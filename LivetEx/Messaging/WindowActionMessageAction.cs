using System.Windows;
using LivetEx.Messaging;

namespace LivetEx.Messaging {
	/// <summary>
	/// Windowの最小化・最大化・閉じる・通常化・ダイアログ結果を行うアクションです。WindowActionMessageに対応します。
	/// </summary>
	public class WindowActionMessageAction : MessageAction<FrameworkElement> {
		protected override void InvokeAction( Message message ) {
			if( message is WindowActionMessage windowMessage){
				Action( this.AssociatedObject, windowMessage );
			}
		}

		public static void Action( FrameworkElement element, WindowActionMessage message ) {

			var window = Window.GetWindow( element );
			if( window != null ) {
				switch( message.Action ) {
					case WindowAction.Close:
					window.Close();
					break;

					case WindowAction.Maximize:
					window.WindowState = WindowState.Maximized;
					break;

					case WindowAction.Minimize:
					window.WindowState = WindowState.Minimized;
					break;

					case WindowAction.Normal:
					window.WindowState = WindowState.Normal;
					break;

					case WindowAction.Active:
					window.Activate();
					break;

					case WindowAction.ResultOK:
					window.DialogResult = true;
					break;

					case WindowAction.ResultCancel:
					window.DialogResult = false;
					break;

					default:
					break;
				}
			}
		}
	}
}
