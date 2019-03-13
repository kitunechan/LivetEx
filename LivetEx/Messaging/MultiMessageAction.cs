using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LivetEx.Messaging;

namespace LivetEx.Messaging {
	public class MultiMessageAction : MessageAction<FrameworkElement> {
		protected override void InvokeAction( Message message ) {
			switch( message ) {

				case WindowCallResponseMethodMessage _message:{
					WindowCallMethodMessageAction.Action( this.AssociatedObject, _message );
					break;
				}
				case WindowCallMethodMessage _message: {
					WindowCallMethodMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case MessageBoxMessage _message: {
					MessageBoxMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case WindowActionMessage _message: {
					WindowActionMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case WindowMessage _message: {
					WindowMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case OpenFileDialogMessage _message: {
					OpenFileDialogMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case SaveFileDialogMessage _message: {
					SaveFileDialogMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				default:
					break;
			}

		}
	}
}
