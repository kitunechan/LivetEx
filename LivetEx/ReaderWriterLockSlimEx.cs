using System;
using System.Threading;

namespace LivetEx {
	public class ReaderWriterLockSlimEx : IDisposable {
		ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		public IDisposable ReadLock() {
			_lock.EnterReadLock();

			return new DisposableAction( () => {
				_lock.ExitReadLock();
			} );
		}

		public IDisposable WriteLock() {
			_lock.EnterWriteLock();

			return new DisposableAction( () => {
				_lock.ExitWriteLock();
			} );
		}

		public IDisposable UpgradeableReadLock() {
			_lock.EnterUpgradeableReadLock();

			return new DisposableAction( () => {
				_lock.ExitUpgradeableReadLock();
			} );
		}


		public void ReadWithLockAction( Action readAction ) {
			if( _lock.IsReadLockHeld ) {
				readAction();
			} else {
				using( ReadLock() ) {
					readAction();
				}
			}
		}

		public TResult ReadWithLockAction<TResult>( Func<TResult> readAction ) {
			if( _lock.IsReadLockHeld ) {
				return readAction();
			} else {
				using( ReadLock() ) {
					return readAction();
				}
			}
		}


		public void ReadAndWriteWithLockAction( Action writeAction, Action readAfterWriteAction ) {
			using( UpgradeableReadLock() ) {
				using( WriteLock() ) {
					writeAction();
				}

				using( ReadLock() ) {
					readAfterWriteAction();
				}
			}
		}

		public TResult ReadAndWriteWithLockAction<TResult>( Func<TResult> writeAction, Action<TResult> readAfterWriteAction ) {
			using( UpgradeableReadLock() ) {

				TResult result;
				using( WriteLock() ) {
					result = writeAction();
				}

				using( ReadLock() ) {
					readAfterWriteAction( result );
				}

				return result;
			}
		}

		public void WriteReadWithLockAction<TResult>( Func<TResult> readBeforeWriteAction, Action<TResult> writeAction, Action<TResult> readAfterWriteAction ) {
			using( UpgradeableReadLock() ) {
				var result = readBeforeWriteAction();

				using( WriteLock() ) {
					writeAction( result );
				}

				using( ReadLock() ) {
					readAfterWriteAction( result );
				}
			}
		}

		#region Dispose
		private bool disposed = false;
		protected virtual void Dispose( bool disposing ) {
			if( !disposed ) {
				disposed = true;

				if( disposing ) {
					_lock.Dispose();
				}
			}
		}

		public void Dispose() {
			Dispose( true );
		}

		#endregion
	}
}
