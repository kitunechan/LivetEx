using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LivetEx.Messaging;

namespace LivetEx.Messaging {
	public class MultiInteractionMessageAction : InteractionMessageAction<FrameworkElement> {
		protected override void InvokeAction( InteractionMessage message ) {
			switch( message ) {

				case CallMethodMessageGeneric _message:{
					CallMethodInteractionMessageAction.Action( this.AssociatedObject, _message );
					break;
				}
				case CallMethodMessage _message: {
					CallMethodInteractionMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case MessageBoxMessage _message: {
					MessageBoxInteractionMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case WindowActionMessage _message: {
					WindowActionInteractionMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case WindowTransitionMessage _message: {
					WindowTransitionInteractionMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case OpenFileDialogMessage _message: {
					OpenFileDialogInteractionMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				case SaveFileDialogMessage _message: {
					SaveFileDialogInteractionMessageAction.Action( this.AssociatedObject, _message );
					break;
				}

				default:
					break;
			}

		}
	}
}
