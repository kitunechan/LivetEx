using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LivetEx.Triggers {
	/// <summary>
	/// ビヘイビア・トリガー・アクションでのメソッド直接バインディングを可能にするためのクラスです。
	/// 引数が一つだけ存在するメソッドを実行します。メソッドの実行は最大限キャッシュされます。
	/// </summary>
	public class MethodBinderWithArgument {
		private static ConcurrentDictionary<Type, ConcurrentDictionary<string, Action<object, object>>> _actionCacheDictionary = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Action<object, object>>>();
		private static ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object, object>>> _funcCacheDictionary = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object, object>>>();

		private static bool TryGetCacheFromActionCacheDictionary( Type targetObjectType, string methodName, out Action<object, object> m ) {
			if( _actionCacheDictionary.TryGetValue( targetObjectType, out var actionDictionary ) ) {
				return actionDictionary.TryGetValue( methodName, out m );
			}

			m = null;
			return false;
		}

		private static bool TryGetCacheFromFuncCacheDictionary( Type targetObjectType, string methodName, out Func<object, object, object> m ) {
			if( _funcCacheDictionary.TryGetValue( targetObjectType, out var funcDictionary ) ) {
				return funcDictionary.TryGetValue( methodName, out m );
			}

			m = null;
			return false;
		}

		private string _methodName;

		private Type _targetObjectType;
		private Type _argumentType;

		private MethodInfo _methodInfo;
		private Action<object, object> _action;
		private Func<object, object, object> _func;

		public void Invoke( object targetObject, string methodName, Type argumentType, object argument ) {
			Invoke( targetObject, methodName, argumentType, argument, typeof( void ) );
		}

		public object Invoke( object targetObject, string methodName, Type argumentType, object argument, Type resultType ) {
			if( targetObject == null ) throw new ArgumentNullException( "targetObject" );
			if( methodName == null ) throw new ArgumentNullException( "methodName" );

			var newTargetObjectType = targetObject.GetType();
			var newArgumentType = argument?.GetType() ?? argumentType;

			if( _targetObjectType == newTargetObjectType && _methodName == methodName && _argumentType == newArgumentType ) {
				if( resultType == typeof( void ) ) {
					if( _action != null ) {
						_action( targetObject, argument );
						return null;
					}

					if( TryGetCacheFromActionCacheDictionary( _targetObjectType, _methodName, out _action ) ) {
						_action( targetObject, argument );
						return null;
					}
				} else {
					if( _func != null ) {
						_func( targetObject, argument );
						return null;
					}

					if( TryGetCacheFromFuncCacheDictionary( _targetObjectType, _methodName, out _func ) ) {
						_func( targetObject, argument );
						return null;
					}
				}

				if( _methodInfo != null ) {
					return _methodInfo.Invoke( targetObject, new[] { argument } );
				}
			}

			_targetObjectType = newTargetObjectType;
			_argumentType = newArgumentType;
			_methodName = methodName;


			if( resultType == typeof( void ) ) {
				if( TryGetCacheFromActionCacheDictionary( _targetObjectType, _methodName, out _action ) ) {
					_action( targetObject, argument );
					return null;
				}
			} else {
				if( TryGetCacheFromFuncCacheDictionary( _targetObjectType, _methodName, out _func ) ) {
					return _func( targetObject, argument );
				}
			}


			_methodInfo = _targetObjectType.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance )
				.FirstOrDefault( method => {
					if( method.Name != methodName ) {
						return false;
					}

					var parameters = method.GetParameters();

					if( parameters.Length != 1 ) {
						return false;
					}

					if( parameters[0].ParameterType.IsInterface ) {
						if( !newArgumentType.GetInterfaces().Contains( parameters[0].ParameterType ) ) {
							return false;
						}
					} else {
						if( ( !_argumentType.IsSubclassOf( parameters[0].ParameterType ) ) && ( _argumentType != parameters[0].ParameterType ) ) {
							return false;
						}
					}

					if( method.ReturnType != resultType ) {
						return false;
					}

					return true;
				} );

			if( _methodInfo == null ) {
				throw new ArgumentException( $"{_targetObjectType.Name} 型に {_argumentType.Name} 型の引数を一つだけ持つメソッド {methodName} が見つかりません。" );
			}

			var result = _methodInfo.Invoke( targetObject, new[] { argument } );
			var taskArgument = new Tuple<Type, MethodInfo, Type>( _targetObjectType, _methodInfo, _methodInfo.GetParameters()[0].ParameterType );

			if( resultType == typeof( void ) ) {
				Task.Factory.StartNew( arg => {
					var taskArg = (Tuple<Type, MethodInfo, Type>)arg;

					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var paraMessage = Expression.Parameter( typeof( object ), "argument" );

					var method = Expression.Lambda<Action<object, object>>(
								Expression.Call( Expression.Convert( paraTarget, taskArg.Item1 ), taskArg.Item2, Expression.Convert( paraMessage, taskArg.Item3 ) ),
								paraTarget,
								paraMessage
							).Compile();

					var dic = _actionCacheDictionary.GetOrAdd( taskArg.Item1, _ => new ConcurrentDictionary<string, Action<object, object>>() );
					dic.TryAdd( taskArg.Item2.Name, method );
				}, taskArgument );

			} else if( Nullable.GetUnderlyingType( resultType ) != null ) {
				// Func<object,Nullable>が作れない・・・

			} else {
				Task.Factory.StartNew( arg => {
					var taskArg = (Tuple<Type, MethodInfo, Type>)arg;

					var paraTarget = Expression.Parameter( typeof( object ), "target" );
					var paraMessage = Expression.Parameter( typeof( object ), "argument" );

					var method = Expression.Lambda<Func<object, object, object>>(
								Expression.Call( Expression.Convert( paraTarget, taskArg.Item1 ), taskArg.Item2, Expression.Convert( paraMessage, taskArg.Item3 ) ),
								paraTarget,
								paraMessage
							).Compile();

					var dic = _funcCacheDictionary.GetOrAdd( taskArg.Item1, _ => new ConcurrentDictionary<string, Func<object, object, object>>() );
					dic.TryAdd( taskArg.Item2.Name, method );
				}, taskArgument );
			}
			return result;
		}

	}
}
