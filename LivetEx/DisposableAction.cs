using System;

namespace LivetEx {
	public sealed class DisposableAction : IDisposable {
		public DisposableAction( Action action ) {
			this._action = action;
		}

		Action _action;

		public void Dispose() {
			_action.Invoke();
		}
	}
}
