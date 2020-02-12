using System.Collections.Concurrent;
using System.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LivetEx.WeakEventListeners;

namespace LivetEx.Messaging {
	public sealed class MessageListener : IDisposable, IEnumerable<KeyValuePair<string, ConcurrentBag<Action<Message>>>> {
		private LivetWeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs> _listener;
		private WeakReference<Messenger> _source;
		private ConcurrentDictionary<string, ConcurrentBag<Action<Message>>> _actionDictionary = new ConcurrentDictionary<string, ConcurrentBag<Action<Message>>>();

		public MessageListener( Messenger messenger ) {
			Dispatcher = Dispatcher.CurrentDispatcher;
			_source = new WeakReference<Messenger>( messenger );
			_listener = new LivetWeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs>
				(
					h => h,
					h => messenger.RaisedLater += h,
					h => messenger.RaisedLater -= h,
					MessageReceived
				);
		}
		public MessageListener( Messenger messenger, Action<Message> action ) : this( messenger, null, action ) {
		}

		public MessageListener( Messenger sendMessenger, Messenger receiveMessenger ) : this( sendMessenger ) {
			RegisterAction( message => {
				receiveMessenger.Raise( message );
			} );
		}

		public MessageListener( Messenger messenger, string messageKey, Action<Message> action ) : this( messenger ) {
			if( messageKey == null ) {
				messageKey = string.Empty;
			}

			RegisterAction( messageKey, action );
		}



		public void RegisterAction( Action<Message> action ) {
			ThrowExceptionIfDisposed();
			_actionDictionary.GetOrAdd( string.Empty, _ => new ConcurrentBag<Action<Message>>() ).Add( action );
		}

		public void RegisterAction( string messageKey, Action<Message> action ) {
			ThrowExceptionIfDisposed();
			_actionDictionary.GetOrAdd( messageKey, _ => new ConcurrentBag<Action<Message>>() ).Add( action );
		}

		private void MessageReceived( object sender, MessageRaisedEventArgs e ) {
			if( _disposed ) return;

			var message = e.Message;
			if( !message.IsHandled ) {
				var cloneMessage = (Message)message.Clone();

				cloneMessage.Freeze();

				DoActionOnDispatcher( () => {
					GetValue( e, cloneMessage );
				} );

				if( message is IResponsiveMessage responsiveMessage ) {
					responsiveMessage.Response = ( (IResponsiveMessage)cloneMessage ).Response;
				}

				if( message is ShowWindowMessage showWindowMessage ) {
					showWindowMessage.ViewModel = ( (ShowWindowMessage)cloneMessage ).ViewModel;
				}
			}
		}

		private void GetValue( MessageRaisedEventArgs e, Message cloneMessage ) {
			if( _source.TryGetTarget( out var _ ) ) {
				if( e.Message.MessageKey != null ) {
					if( _actionDictionary.TryGetValue( e.Message.MessageKey, out var list ) ) {
						foreach( var action in list ) {
							action( cloneMessage );
						}
					}
				}

				if( _actionDictionary.TryGetValue( string.Empty, out var allList ) ) {
					foreach( var action in allList ) {
						action( cloneMessage );
					}
				}
			}
		}

		private void DoActionOnDispatcher( Action action ) {
			if( Dispatcher.CheckAccess() ) {
				action();
			} else {
				Dispatcher.Invoke( action );
			}
		}

		IEnumerator<KeyValuePair<string, ConcurrentBag<Action<Message>>>> IEnumerable<KeyValuePair<string, ConcurrentBag<Action<Message>>>>.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		public Dispatcher Dispatcher { get; set; }

		public void Add( Action<Message> action ) {
			RegisterAction( action );
		}

		public void Add( string messageKey, Action<Message> action ) {
			RegisterAction( messageKey, action );
		}


		public void Add( string messageKey, params Action<Message>[] actions ) {
			foreach( var action in actions ) {
				RegisterAction( messageKey, action );
			}
		}

		private void ThrowExceptionIfDisposed() {
			if( _disposed ) {
				throw new ObjectDisposedException( "EventListener" );
			}
		}

		private bool _disposed;

		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private void Dispose( bool disposing ) {
			if( _disposed ) return;

			if( disposing ) {
				_listener.Dispose();
			}
			_disposed = true;
		}
	}
}
