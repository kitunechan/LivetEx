using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LivetEx.Triggers;

namespace LivetEx.Messaging {
	/// <summary>
	/// メッセージボックスを表示するアクションです。<see cref="MessageBoxMessage"/>に対応します。
	/// </summary>
	public class CallMethodInteractionMessageAction : InteractionMessageAction<FrameworkElement> {
		protected override void InvokeAction( InteractionMessage message ) {
			if( message is CallMethodMessage callMethodMessage ) {
				var clone = (CallMethodMessage)callMethodMessage.Clone();

				Action( this.AssociatedObject, clone );
			} else if( message is CallMethodMessageGeneric responsiveInteractionMessage ) {
				var clone = (CallMethodMessageGeneric)responsiveInteractionMessage.Clone();

				Action( this.AssociatedObject, clone );

				responsiveInteractionMessage.Response = clone.Response;
			}
		}


		static LivetCallMethodAction LivetCallMethodAction = new LivetCallMethodAction();

		

		public static void Action( FrameworkElement element, CallMethodMessageGeneric message ) {
			LivetCallMethodAction.MethodTarget = Window.GetWindow( element );

			LivetCallMethodAction.MethodName = message.MethodName;
			LivetCallMethodAction.MethodParameter = message.MethodParameter;

			LivetCallMethodAction._Invoke( message );
		}

		public static void Action( FrameworkElement element, CallMethodMessage message ) {
			LivetCallMethodAction.MethodTarget = Window.GetWindow( element );

			LivetCallMethodAction.MethodName = message.MethodName;
			LivetCallMethodAction.MethodParameter = message.MethodParameter;

			LivetCallMethodAction._Invoke( message );

		}
	}
}
