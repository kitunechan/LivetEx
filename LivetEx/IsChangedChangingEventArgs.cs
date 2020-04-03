using System;

namespace LivetEx {
	public class IsChangedChangingEventArgs : EventArgs {
		public IsChangedChangingEventArgs( bool oldValue, bool newValue ) {
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		public bool NewValue { get; private set; }
		public bool OldValue { get; private set; }
	}
}
