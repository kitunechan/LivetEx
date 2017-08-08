using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LivetEx.Triggers {
	/// <summary>
	/// ビヘイビア・トリガー・アクションでのメソッド直接バインディングを可能にするためのクラスです。<br/>
	/// 引数の無いメソッドを実行します。メソッドの実行はキャッシュされます。
	/// </summary>
	public class MethodBinder {
		private static ConcurrentDictionary<Type, ConcurrentDictionary<string, Action<object>>> _ActionCacheDictionary = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Action<object>>>();
		private static ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>> _FuncCacheDictionary = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>>();

		private static bool TryGetCacheFromActionCacheDictionary( Type targetObjectType, string methodName, out Action<object> m ) {
			if( _ActionCacheDictionary.TryGetValue( targetObjectType, out var actionDictionary ) ) {
				return actionDictionary.TryGetValue( methodName, out m );
			}

			m = null;
			return false;
		}

		private static bool TryGetCacheFromFunctionCacheDictionary( Type targetObjectType, string methodName, out Func<object, object> m ) {
			if( _FuncCacheDictionary.TryGetValue( targetObjectType, out var functionDictionary ) ) {
				return functionDictionary.TryGetValue( methodName, out m );
			}

			m = null;
			return false;
		}

		private Func<object, object> _function;
		private Action<object> _action;

		private string _methodName;
		private MethodInfo _methodInfo;
		private Type _targetObjectType;

		public void Invoke( object targetObject, string methodName ) {
			Invoke( targetObject, methodName, typeof( void ) );
		}

		public object Invoke( object targetObject, string methodName, Type resultType ) {
			if( targetObject == null ) throw new ArgumentNullException( "targetObject" );
			if( methodName == null ) throw new ArgumentNullException( "methodName" );

			var newTargetObjectType = targetObject.GetType();

			if( _targetObjectType == newTargetObjectType && _methodName == methodName ) {
				if( resultType == typeof( void ) ) {
					if( _action != null ) {
						_action( targetObject );
						return null;
					}

					if( TryGetCacheFromActionCacheDictionary( _targetObjectType, _methodName, out _action ) ) {
						_action( targetObject );
						return null;
					}
					
				} else {
					if( _function != null ) {
						return _function( targetObject );
					}

					if( TryGetCacheFromFunctionCacheDictionary( _targetObjectType, _methodName, out _function ) ) {
						return _function( targetObject );
					}
				}

				if( _methodInfo != null ) {
					return _methodInfo.Invoke( targetObject, new object[] { } );
				}
			}

			_targetObjectType = newTargetObjectType;
			_methodName = methodName;

			if( resultType == typeof( void ) ) {
				if( TryGetCacheFromActionCacheDictionary( _targetObjectType, _methodName, out _action ) ) {
					_action( targetObject );
					return null;
				}
			} else {
				if( TryGetCacheFromFunctionCacheDictionary( _targetObjectType, _methodName, out _function ) ) {
					return _function( targetObject );
				}
			}
			
			_methodInfo = _targetObjectType.GetMethods()
							.FirstOrDefault( method => method.Name == methodName && !method.GetParameters().Any() && method.ReturnType == resultType );

			if( _methodInfo == null ) {
				throw new ArgumentException( $"{_targetObjectType.Name}型に{resultType}を返す引数の無いメソッド{methodName}が見つかりません。" );
			}

			var result = _methodInfo.Invoke( targetObject, new object[] { } );
			var taskArgument = new Tuple<Type, MethodInfo>( _targetObjectType, _methodInfo );

			if( resultType == typeof( void ) ) {
				Task.Factory.StartNew( arg => {
					var taskArg = (Tuple<Type, MethodInfo>)arg;

					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var method = Expression.Lambda<Action<object>>( Expression.Call( Expression.Convert( paraTarget, taskArg.Item1 ), taskArg.Item2 ), paraTarget ).Compile();

					var dic = _ActionCacheDictionary.GetOrAdd( taskArg.Item1, _ => new ConcurrentDictionary<string, Action<object>>() );
					dic.TryAdd( taskArg.Item2.Name, method );
				}, taskArgument );
			}else if( Nullable.GetUnderlyingType( resultType ) != null ) {
				// Func<object,Nullable>が作れない・・・

			} else {
				Task.Factory.StartNew( arg => {
					var taskArg = (Tuple<Type, MethodInfo>)arg;

					var paraTarget = Expression.Parameter( typeof( object ), "target" );

					var method = Expression.Lambda<Func<object, object>>( Expression.Call( Expression.Convert( paraTarget, taskArg.Item1 ), taskArg.Item2 ), paraTarget ).Compile();

					var dic = _FuncCacheDictionary.GetOrAdd( taskArg.Item1, _ => new ConcurrentDictionary<string, Func<object, object>>() );
					dic.TryAdd( taskArg.Item2.Name, method );
				}, taskArgument );
			}
			return result;
		}


	}
}
