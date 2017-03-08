using LivetEx.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LivetEx.Behaviors.Messaging {
	/// <summary>
	/// メッセージボックスを表示するアクションです。<see cref="MessageBoxMessage"/>に対応します。
	/// </summary>
	public class MessageBoxInteractionMessageAction: InteractionMessageAction<FrameworkElement> {
		protected override void InvokeAction( InteractionMessage message ) {
			var messageBoxMessage = message as MessageBoxMessage;

			var window = Window.GetWindow( this.AssociatedObject );

			if( messageBoxMessage != null ) {
				messageBoxMessage.Response = MessageBox.Show(
					window,
					messageBoxMessage.Text,
					messageBoxMessage.Caption,
					messageBoxMessage.Button,
					messageBoxMessage.Image,
					messageBoxMessage.DefaultResult
					);
			}

		}
	}
}
