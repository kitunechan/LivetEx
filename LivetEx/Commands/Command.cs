using System;
using System.Collections.Generic;
using System.Linq;

namespace LivetEx.Commands {
	public abstract class Command {
		private List<WeakReference<EventHandler>> _canExecuteChangedHandlers = new List<WeakReference<EventHandler>>();

		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に発生します。
		/// </summary>
		public event EventHandler CanExecuteChanged {
			add {
				_canExecuteChangedHandlers.Add( new WeakReference<EventHandler>( value ) );
			}
			remove {
				var list = _canExecuteChangedHandlers.Where( r => r.TryGetTarget( out var result ) && result == value ).ToArray();
				foreach( var weakReference in list ) {
					_canExecuteChangedHandlers.Remove( weakReference );
				}
			}
		}

		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に呼び出されます。
		/// </summary>
		protected void OnCanExecuteChanged() {
			foreach( var handlerWeakReference in _canExecuteChangedHandlers.ToArray() ) {
				if( handlerWeakReference.TryGetTarget( out var result ) ) {
					LivetDispatcherHelper.UIDispatcher.InvokeAsync( () => result( this, EventArgs.Empty ) );
				} else {
					_canExecuteChangedHandlers.Remove( handlerWeakReference );
				}
			}
		}
	}
}
