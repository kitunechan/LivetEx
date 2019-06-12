using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LivetEx.Messaging;
using LivetEx.Triggers;

namespace LivetEx.Messaging {
	public class MultiMessageAction : MessageAction<FrameworkElement, Message> {
		protected override void InvokeAction( Message message ) {
			switch( message ) {
				case MessageBoxMessage _message: {
					MessageBoxMessageAction.Action( this.AssociatedObject, _message );
					return;
				}

				case WindowActionMessage _message: {
					WindowActionMessageAction.Action( this.AssociatedObject, _message );
					return;
				}

				case WindowMessage _message: {
					WindowMessageAction.Action( this.AssociatedObject, _message );
					return;
				}

				case OpenFileDialogMessage _message: {
					OpenFileDialogMessageAction.Action( this.AssociatedObject, _message );
					return;
				}

				case SaveFileDialogMessage _message: {
					SaveFileDialogMessageAction.Action( this.AssociatedObject, _message );
					return;
				}

				case ICallMethodMessage _message: {
					CallMethodAction.Action( this.AssociatedObject, _message );
					return;
				}
			}

		}
	}
}
