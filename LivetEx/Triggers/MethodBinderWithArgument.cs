using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace LivetEx.Triggers {


	/// <summary>
	/// ビヘイビア・トリガー・アクションでのメソッド直接バインディングを可能にするためのクラスです。
	/// 引数が一つだけ存在するメソッドを実行します。メソッドの実行は最大限キャッシュされます。
	/// </summary>
	public class MethodBinderWithArgument {
		private static ConcurrentDictionary<Type, ConcurrentDictionary<string, ConcurrentDictionary<Type, Action<object, object>>>> _actionCacheDictionary
			= new ConcurrentDictionary<Type, ConcurrentDictionary<string, ConcurrentDictionary<Type, Action<object, object>>>>();
		private static ConcurrentDictionary<Type, ConcurrentDictionary<string, ConcurrentDictionary<Type, Func<object, object, object>>>> _funcCacheDictionary
			= new ConcurrentDictionary<Type, ConcurrentDictionary<string, ConcurrentDictionary<Type, Func<object, object, object>>>>();
		private static readonly List<Task> taskList = new List<Task>();

		public static IEnumerable<Task> Tasks => taskList;

		private static bool TryGetCacheFromActionCacheDictionary( Type targetType, string methodName, Type argmentType, out Action<object, object> action ) {
			if( _actionCacheDictionary.TryGetValue( targetType, out var argmentTypeDictionary ) ) {
				if( argmentTypeDictionary.TryGetValue( methodName, out var actionDictionary ) ) {
					return actionDictionary.TryGetValue( argmentType, out action );
				}
			}
			action = null;
			return false;
		}

		private static bool TryGetCacheFromFuncCacheDictionary( Type targetType, string methodName, Type argmentType, out Func<object, object, object> func ) {
			if( _funcCacheDictionary.TryGetValue( targetType, out var argmentTypeDictionary ) ) {
				if( argmentTypeDictionary.TryGetValue( methodName, out var funcDictionary ) ) {
					return funcDictionary.TryGetValue( argmentType, out func );
				}
			}
			func = null;
			return false;
		}

		private Type _targetTypeCache;
		private string _methodNameCache;
		private Type _argumentTypeCache;

		private Action<object, object> _actionCache;
		private Func<object, object, object> _funcCache;
		private MethodInfo _methodInfoCache;



		public object Invoke( object target, string methodName, Type argumentType, object argument ) {
			if( target == null ) throw new ArgumentNullException( "targetObject" );
			if( methodName == null ) throw new ArgumentNullException( "methodName" );
			if( argumentType == null ) throw new ArgumentNullException( "argumentType" );

			var targetType = target.GetType();

			if( targetType == _targetTypeCache && methodName == _methodNameCache && argumentType == _argumentTypeCache ) {

				if( _actionCache != null ) {
					_actionCache( target, argument );
					return null;
				}
				if( _funcCache != null ) {
					return _funcCache( target, argument );
				}

				if( TryGetCacheFromActionCacheDictionary( targetType, methodName, argumentType, out _actionCache ) ) {
					_actionCache( target, argument );
					return null;
				}
				if( TryGetCacheFromFuncCacheDictionary( targetType, methodName, argumentType, out _funcCache ) ) {
					return _funcCache( target, argument );
				}

				if( _methodInfoCache != null ) {
					return _methodInfoCache.Invoke( target, new[] { argument } );
				}

			} else {
				_targetTypeCache = targetType;
				_methodNameCache = methodName;
				_argumentTypeCache = argumentType;
				_actionCache = null;
				_funcCache = null;

				if( TryGetCacheFromActionCacheDictionary( targetType, methodName, argumentType, out _actionCache ) ) {
					_actionCache( target, argument );
					return null;
				}
				if( TryGetCacheFromFuncCacheDictionary( targetType, methodName, argumentType, out _funcCache ) ) {
					return _funcCache( target, argument );
				}
			}

			var methods = _targetTypeCache.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
							.Where( method => method.Name == methodName )
							.ToArray();

			_methodInfoCache = methods
									.FirstOrDefault( method => {
										var parameters = method.GetParameters();
										return parameters.Length == 1 && parameters[0].ParameterType == argumentType;
									} );

			if( _methodInfoCache == null ) {
				_methodInfoCache = methods
									.FirstOrDefault( method => {
										var parameters = method.GetParameters();
										return parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom( argumentType );
									} );
			}

			if( _methodInfoCache == null ) {
				throw new ArgumentException( $"{targetType.Name} 型に {argumentType.Name} 型の引数を一つだけ持つメソッド {methodName} が見つかりません。" );
			}

			var taskArgs = new { TargetType = targetType, MethodInfo = _methodInfoCache, ParameterType = _methodInfoCache.GetParameters()[0].ParameterType };
			var result = _methodInfoCache.Invoke( target, new[] { argument } );

			if( _methodInfoCache.ReturnType == typeof( void ) ) {
				var t = Task.Run( () => {
					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var paraParameterType = Expression.Parameter( typeof( object ), "argument" );

					var method = Expression.Lambda<Action<object, object>>(
									Expression.Call(
										Expression.Convert( paraTarget, taskArgs.TargetType ),
										taskArgs.MethodInfo,
										Expression.Convert( paraParameterType, taskArgs.ParameterType )
									),
									paraTarget,
									paraParameterType
								).Compile();

					_actionCacheDictionary
						.GetOrAdd( taskArgs.TargetType, _ => new ConcurrentDictionary<string, ConcurrentDictionary<Type, Action<object, object>>>() )
						.GetOrAdd( taskArgs.MethodInfo.Name, _ => new ConcurrentDictionary<Type, Action<object, object>>() )
						.TryAdd( taskArgs.ParameterType, method );
				} );

				taskList.Add( t );
				t.ContinueWith( _ => {
					taskList.Remove( _ );
				} );
			} else {
				var t = Task.Run( () => {
					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var paraParameterType = Expression.Parameter( typeof( object ), "argument" );

					var method = Expression.Lambda<Func<object, object, object>>(
									Expression.Convert(
										Expression.Call(
											Expression.Convert( paraTarget, taskArgs.TargetType ),
											taskArgs.MethodInfo,
											Expression.Convert( paraParameterType, taskArgs.ParameterType )
										),
										typeof( object )
									),
									paraTarget,
									paraParameterType
								).Compile();

					_funcCacheDictionary
						.GetOrAdd( taskArgs.TargetType, _ => new ConcurrentDictionary<string, ConcurrentDictionary<Type, Func<object, object, object>>>() )
						.GetOrAdd( taskArgs.MethodInfo.Name, _ => new ConcurrentDictionary<Type, Func<object, object, object>>() )
						.TryAdd( taskArgs.ParameterType, method );
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
