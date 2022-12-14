using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// Windowの最小化・最大化・閉じる・通常化・ダイアログ結果を行うアクションです。WindowActionMessageに対応します。
	/// </summary>
	public class WindowActionMessageAction : MessageAction<FrameworkElement, WindowActionMessage> {
		protected override void InvokeAction( WindowActionMessage message ) {
			Action( this.AssociatedObject, message );
		}

		public static void Action( FrameworkElement element, WindowActionMessage message ) {
			var window = Window.GetWindow( element );
			if( window != null ) {
				message.IsHandled = true;
				switch( message.Action ) {
					case WindowAction.Close: {
						window.Close();
						break;
					}
					case WindowAction.Maximize: {
						window.WindowState = WindowState.Maximized;
						break;
					}
					case WindowAction.Minimize: {
						window.WindowState = WindowState.Minimized;
						break;
					}
					case WindowAction.Normal: {
						window.WindowState = WindowState.Normal;
						break;
					}
					case WindowAction.Active: {
						window.Activate();
						break;
					}
					case WindowAction.ResultOK: {
						window.DialogResult = true;
						break;
					}
					case WindowAction.ResultCancel: {
						window.DialogResult = false;
						break;
					}

					default: {
						break;
					}
				}
			}
		}
	}
}
