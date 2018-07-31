﻿using System.Collections.Concurrent;
using System.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LivetEx.WeakEventListeners;

namespace LivetEx.Messaging {
	public sealed class MessageListener : IDisposable, IEnumerable<KeyValuePair<string, ConcurrentBag<Action<InteractionMessage>>>> {
		private LivetWeakEventListener<EventHandler<InteractionMessageRaisedEventArgs>, InteractionMessageRaisedEventArgs> _listener;
		private WeakReference<InteractionMessenger> _source;
		private ConcurrentDictionary<string, ConcurrentBag<Action<InteractionMessage>>> _actionDictionary = new ConcurrentDictionary<string, ConcurrentBag<Action<InteractionMessage>>>();


		public MessageListener( InteractionMessenger sendMessenger, InteractionMessenger receiveMessenger )
			: this( sendMessenger ) {
			RegisterAction( message=> {
				receiveMessenger.Raise( message );
			} );
		}

		public MessageListener( InteractionMessenger messenger ) {
			Dispatcher = Dispatcher.CurrentDispatcher;
			_source = new WeakReference<InteractionMessenger>( messenger );
			_listener = new LivetWeakEventListener<EventHandler<InteractionMessageRaisedEventArgs>, InteractionMessageRaisedEventArgs>
				(
					h => h,
					h => messenger.Raised += h,
					h => messenger.Raised -= h,
					MessageReceived
				);
		}

		public MessageListener( InteractionMessenger messenger, string messageKey, Action<InteractionMessage> action )
			: this( messenger ) {
			if( messageKey == null ) {
				messageKey = string.Empty;
			}

			RegisterAction( messageKey, action );
		}

		public MessageListener( InteractionMessenger messenger, Action<InteractionMessage> action )
			: this( messenger, null, action ) {
		}

		public void RegisterAction( Action<InteractionMessage> action ) {
			ThrowExceptionIfDisposed();
			_actionDictionary.GetOrAdd( string.Empty, _ => new ConcurrentBag<Action<InteractionMessage>>() ).Add( action );
		}

		public void RegisterAction( string messageKey, Action<InteractionMessage> action ) {
			ThrowExceptionIfDisposed();
			_actionDictionary.GetOrAdd( messageKey, _ => new ConcurrentBag<Action<InteractionMessage>>() ).Add( action );
		}

		private void MessageReceived( object sender, InteractionMessageRaisedEventArgs e ) {
			if( _disposed ) return;

			var message = e.Message;

			var cloneMessage = (InteractionMessage)message.Clone();

			cloneMessage.Freeze();

			DoActionOnDispatcher( () => {
				GetValue( e, cloneMessage );
			} );

			if( message is IResponsiveInteractionMessage responsiveMessage ) {
				responsiveMessage.Response = ( (IResponsiveInteractionMessage)cloneMessage ).Response;
			}
		}

		private void GetValue( InteractionMessageRaisedEventArgs e, InteractionMessage cloneMessage ) {
			if( _source.TryGetTarget( out var _ ) ) {
				if( e.Message.MessageKey != null ) {
					_actionDictionary.TryGetValue( e.Message.MessageKey, out var list );

					if( list != null ) {
						foreach( var action in list ) {
							action( cloneMessage );
						}
					}
				}

				_actionDictionary.TryGetValue( string.Empty, out var allList );
				if( allList != null ) {
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

		IEnumerator<KeyValuePair<string, ConcurrentBag<Action<InteractionMessage>>>> IEnumerable<KeyValuePair<string, ConcurrentBag<Action<InteractionMessage>>>>.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		public Dispatcher Dispatcher { get; set; }

		public void Add( Action<InteractionMessage> action ) {
			RegisterAction( action );
		}

		public void Add( string messageKey, Action<InteractionMessage> action ) {
			RegisterAction( messageKey, action );
		}


		public void Add( string messageKey, params Action<InteractionMessage>[] actions ) {
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
