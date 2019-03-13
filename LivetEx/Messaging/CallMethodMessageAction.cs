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
			if( message is WindowCallMethodMessage callMethodMessage ) {
				var clone = (WindowCallMethodMessage)callMethodMessage.Clone();

				Action( this.AssociatedObject, clone );
			} else if( message is WindowCallResponseMethodMessage responsiveMessage ) {
				var clone = (WindowCallResponseMethodMessage)responsiveMessage.Clone();

				Action( this.AssociatedObject, clone );

				responsiveMessage.Response = clone.Response;
			}
		}

		static readonly LivetCallMethodAction LivetCallMethodAction = new LivetCallMethodAction();

		public static void Action( FrameworkElement element, WindowCallResponseMethodMessage message ) {
			LivetCallMethodAction.MethodTarget = Window.GetWindow( element );

			LivetCallMethodAction.MethodName = message.MethodName;
			LivetCallMethodAction.MethodParameter = message.MethodParameter;

			LivetCallMethodAction._Invoke( message );
		}

		public static void Action( FrameworkElement element, WindowCallMethodMessage message ) {
			LivetCallMethodAction.MethodTarget = Window.GetWindow( element );

			LivetCallMethodAction.MethodName = message.MethodName;
			LivetCallMethodAction.MethodParameter = message.MethodParameter;

			LivetCallMethodAction._Invoke( message );

		}
	}
}
