using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// メッセージボックスを表示するアクションです。<see cref="MessageBoxMessage"/>に対応します。
	/// </summary>
	public class MessageBoxInteractionMessageAction : InteractionMessageAction<FrameworkElement> {
		protected override void InvokeAction( InteractionMessage message ) {
			if( message is MessageBoxMessage messageBoxMessage ) {
				var clone = (MessageBoxMessage)messageBoxMessage.Clone();
				{
					clone.IsOwned = messageBoxMessage.IsOwned ?? IsOwned ?? true;
				}


				Action( this.AssociatedObject, clone );

				messageBoxMessage.Response = clone.Response;
			}
		}

		/// <summary>
		/// メッセージボックスがこのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool? IsOwned {
			get { return (bool?)GetValue( OwnedFromThisProperty ); }
			set { SetValue( OwnedFromThisProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for OwnedFromThis.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OwnedFromThisProperty =
			DependencyProperty.Register( "IsOwned", typeof( bool? ), typeof( MessageBoxInteractionMessageAction ), new PropertyMetadata( null ) );


		public static void Action( FrameworkElement element, MessageBoxMessage message ) {
			if( message.IsOwned == false ) {
				message.Response = MessageBox.Show(
					message.Text,
					message.Caption,
					message.Button,
					message.Image,
					message.DefaultResult
					);
			} else {
				var window = Window.GetWindow( element );
				message.Response = MessageBox.Show(
					window,
					message.Text,
					message.Caption,
					message.Button,
					message.Image,
					message.DefaultResult
					);
			}

		}
	}
}
