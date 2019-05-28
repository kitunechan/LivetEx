using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LivetEx.Triggers;

namespace LivetEx.Messaging {
	/// <summary>
	/// ウインドウのメソッドを呼び出すアクションです。
	/// </summary>
	public class WindowCallMethodMessageAction : MessageAction<FrameworkElement> {
		protected override void InvokeAction( Message message ) {
			Action( this.AssociatedObject, message );
		}

		static readonly LivetCallMethodAction LivetCallMethodAction = new LivetCallMethodAction();

		public static void Action( FrameworkElement element, Message message ) {
			var t = message.GetType();
			var baseType = t.GetGenericTypeDefinition();

			if( baseType == typeof( WindowCallResponseMethodMessage<> ) ) {
				var m = typeof( WindowCallMethodMessageAction ).GetMethods()
							.Where( x => x.Name == "Action" )
							.Where( x => x.IsGenericMethod && x.GetGenericArguments().Length == 1 )
							.Where( x => {
								var @params = x.GetParameters();
								if( @params.Length == 2 && @params[1].ParameterType.GetGenericTypeDefinition() == typeof( WindowCallResponseMethodMessage<> ) ) {
									return true;
								}
								return false;
							} )
							.First();

				var clone = message.Clone();

				m.MakeGenericMethod( t.GenericTypeArguments )
					.Invoke( null, new object[] { element, clone } );

				var p_Response = typeof( WindowCallResponseMethodMessage<> ).MakeGenericType( t.GetGenericArguments() ).GetProperty( "Response" );
				p_Response.SetValue( message, p_Response.GetValue( clone ) );

			} else if( baseType == typeof( WindowCallResponseMethodMessage<,> ) ) {
				var m = typeof( WindowCallMethodMessageAction ).GetMethods()
							.Where( x => x.Name == "Action" )
							.Where( x => x.IsGenericMethod && x.GetGenericArguments().Length == 2 )
							.First();

				var clone = message.Clone();

				m.MakeGenericMethod( t.GenericTypeArguments )
					.Invoke( null, new object[] { element, clone } );

				var p_Response = typeof( WindowCallResponseMethodMessage<> ).MakeGenericType( t.GetGenericArguments() ).GetProperty( "Response" );
				p_Response.SetValue( message, p_Response.GetValue( clone ) );

			} else if( baseType == typeof( WindowCallMethodMessage<> ) ) {
				var m = typeof( WindowCallMethodMessageAction ).GetMethods()
							.Where( x => x.Name == "Action" )
							.Where( x => x.IsGenericMethod && x.GetGenericArguments().Length == 1 )
							.Where( x => {
								var @params = x.GetParameters();
								if( @params.Length == 2 && @params[1].ParameterType.GetGenericTypeDefinition() == typeof( WindowCallMethodMessage<> ) ) {
									return true;
								}
								return false;
							} )
							.First();

				m.MakeGenericMethod( t.GenericTypeArguments )
					.Invoke( null, new object[] { element, message.Clone() } );
			}
		}

		public static void Action<TValue, TResult>( FrameworkElement element, WindowCallResponseMethodMessage<TValue, TResult> message ) {
			LivetCallMethodAction.MethodTarget = element;

			LivetCallMethodAction.MethodName = message.MethodName;
			LivetCallMethodAction.MethodParameter = message.MethodParameter;

			LivetCallMethodAction._Invoke( message );
		}

		public static void Action<TResult>( FrameworkElement element, WindowCallResponseMethodMessage<TResult> message ) {
			LivetCallMethodAction.MethodName = message.MethodName;
			LivetCallMethodAction.MethodParameter = null;

			LivetCallMethodAction._Invoke( message );
		}

		public static void Action<T>( FrameworkElement element, WindowCallMethodMessage<T> message ) {
			LivetCallMethodAction.MethodTarget = element;

			LivetCallMethodAction.MethodName = message.MethodName;
			LivetCallMethodAction.MethodParameter = message.MethodParameter;

			LivetCallMethodAction._Invoke( message );

		}
	}
}
