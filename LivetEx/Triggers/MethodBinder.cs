using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace LivetEx.Triggers {
	/// <summary>
	/// ビヘイビア・トリガー・アクションでのメソッド直接バインディングを可能にするためのクラスです。<br/>
	/// 引数の無いメソッドを実行します。メソッドの実行はキャッシュされます。
	/// </summary>
	public class MethodBinder {
		private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Action<object>>> _ActionCacheDictionary = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Action<object>>>();
		private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>> _FuncCacheDictionary = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>>();

		private static readonly List<Task> taskList = new List<Task>();

		public static IEnumerable<Task> Tasks => taskList;

		private static bool TryGetCacheFromActionCacheDictionary( Type targetType, string methodName, out Action<object> action ) {
			if( _ActionCacheDictionary.TryGetValue( targetType, out var actionDictionary ) ) {
				return actionDictionary.TryGetValue( methodName, out action );
			}
			action = null;
			return false;
		}

		private static bool TryGetCacheFromFunctionCacheDictionary( Type targetType, string methodName, out Func<object, object> func ) {
			if( _FuncCacheDictionary.TryGetValue( targetType, out var functionDictionary ) ) {
				return functionDictionary.TryGetValue( methodName, out func );
			}
			func = null;
			return false;
		}

		private Type _targetTypeCache;
		private string _methodNameCache;

		private Func<object, object> _functionCache;
		private Action<object> _actionCache;
		private MethodInfo _methodInfoCache;

		public object Invoke( object target, string methodName ) {
			if( target == null ) throw new ArgumentNullException( "targetObject" );
			if( methodName == null ) throw new ArgumentNullException( "methodName" );

			var targetType = target.GetType();

			if( targetType == _targetTypeCache && methodName == _methodNameCache ) {
				if( _actionCache != null ) {
					_actionCache( target );
					return null;
				}
				if( _functionCache != null ) {
					return _functionCache( target );
				}

				if( TryGetCacheFromActionCacheDictionary( targetType, methodName, out _actionCache ) ) {
					_actionCache( target );
					return null;
				}
				if( TryGetCacheFromFunctionCacheDictionary( targetType, methodName, out _functionCache ) ) {
					return _functionCache( target );
				}

				if( _methodInfoCache != null ) {
					return _methodInfoCache.Invoke( target, null );
				}

				throw new Exception( "Cache Error" );

			} else {
				_targetTypeCache = targetType;
				_methodNameCache = methodName;
				_actionCache = null;
				_functionCache = null;

				if( TryGetCacheFromActionCacheDictionary( targetType, methodName, out _actionCache ) ) {
					_actionCache( target );
					return null;
				}
				if( TryGetCacheFromFunctionCacheDictionary( targetType, methodName, out _functionCache ) ) {
					return _functionCache( target );
				}
			}

			_methodInfoCache = targetType.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
									.FirstOrDefault( method => method.Name == methodName && !method.GetParameters().Any() );

			if( _methodInfoCache == null ) {
				throw new ArgumentException( $"{targetType.Name}型に引数の無いメソッド{methodName}が見つかりません。" );
			}

			var taskArgs = new { TargetType= targetType, MethodInfo = _methodInfoCache };
			var result = _methodInfoCache.Invoke( target, null );

			// メソッドのキャッシュ処理
			if( taskArgs.MethodInfo.ReturnType == typeof( void ) ) {
				var t = Task.Run( () => {

					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var method = Expression.Lambda<Action<object>>(
									Expression.Call(
										Expression.Convert( paraTarget, taskArgs.TargetType ),
										taskArgs.MethodInfo
									),
									paraTarget
								).Compile();

					_ActionCacheDictionary
						.GetOrAdd( taskArgs.TargetType, _ => new ConcurrentDictionary<string, Action<object>>() )
						.TryAdd( taskArgs.MethodInfo.Name, method );
				} );

				taskList.Add( t );
				t.ContinueWith( _ => {
					taskList.Remove( _ );
				} );

			} else {
				var t = Task.Run( () => {
					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var method = Expression.Lambda<Func<object, object>>(
									Expression.Convert(
										Expression.Call( Expression.Convert( paraTarget, taskArgs.TargetType ), taskArgs.MethodInfo ),
										typeof( object )
									),
									paraTarget
								).Compile();

					_FuncCacheDictionary
						.GetOrAdd( taskArgs.TargetType, _ => new ConcurrentDictionary<string, Func<object, object>>() )
						.TryAdd( taskArgs.MethodInfo.Name, method );
				} );

				taskList.Add( t );
				t.ContinueWith( _ => {
					taskList.Remove( _ );
				} );
			}

			return result;
		}


	}
}
