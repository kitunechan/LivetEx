using System;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using LivetEx.WeakEventListeners;

namespace LivetEx.Messaging {
	/// <summary>
	/// ViewModelからの相互作用メッセージを受信し、アクションを実行します。
	/// </summary>
	public class MessageTrigger : TriggerBase<FrameworkElement>, IDisposable {
		public MessageTrigger() {

		}
		public MessageTrigger( Messenger messenger ) {
			this.Messenger = messenger;
		}

		public MessageTrigger( Messenger messenger, string messageKey ) {
			this.Messenger = messenger;
			this.MessageKey = messageKey;
		}

		private LivetWeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs> _listener;
		
		private bool _IsLoaded;

		#region Register Messenger
		/// <summary>
		/// ViewModelのMessengerを指定、または取得します。
		/// </summary>
		public Messenger Messenger {
			get { return (Messenger)GetValue( MessengerProperty ); }
			set { SetValue( MessengerProperty, value ); }
		}

		public static readonly DependencyProperty MessengerProperty =
			DependencyProperty.Register( "Messenger", typeof( Messenger ), typeof( MessageTrigger ),
										new PropertyMetadata( MessengerChanged ) );
		#endregion

		#region Register IsActionLoadedOnly
		/// <summary>
		/// アタッチされたオブジェクトがロードされている期間(Loaded~Unloaded)だけActionを実行するかどうかを指定、または取得します。デフォルトはfalseです。
		/// </summary>
		public bool IsActionLoadedOnly {
			get { return (bool)GetValue( IsActionLoadedOnlyProperty ); }
			set { SetValue( IsActionLoadedOnlyProperty, value ); }
		}

		public static readonly DependencyProperty IsActionLoadedOnlyProperty =
			DependencyProperty.Register( "IsActionLoadedOnly", typeof( bool ), typeof( MessageTrigger ), new PropertyMetadata( false ) );
		#endregion



		/// <summary>
		/// このトリガーが反応する相互作用メッセージのメッセージキーを指定、または取得します。<br/>
		/// このプロパティが指定されていない場合、このトリガーは全ての相互作用メッセージに反応します。
		/// </summary>
		public string MessageKey { get; set; }

		private static void MessengerChanged( DependencyObject obj, DependencyPropertyChangedEventArgs e ) {
			var sender = (MessageTrigger)obj;

			if( e.OldValue == e.NewValue ) {
				return;
			}

			if( e.OldValue != null ) {
				sender._listener?.Dispose();
			}

			if( e.NewValue != null ) {
				var newMessenger = (Messenger)e.NewValue;

				sender._listener = new LivetWeakEventListener<EventHandler<MessageRaisedEventArgs>, MessageRaisedEventArgs>(
					h => h,
					h => newMessenger.Raised += h,
					h => newMessenger.Raised -= h,
					sender.MessageReceived
				);
			}
		}

		private void MessageReceived( object sender, MessageRaisedEventArgs e ) {
			var message = e.Message;
			if( !message.IsHandled ) {
				var cloneMessage = (Message)message.Clone();
				cloneMessage.Freeze();

				var checkResult = false;
				DoActionOnDispatcher( () => {
					if( IsActionLoadedOnly && !_IsLoaded ) {
						return;
					}

					if( string.IsNullOrEmpty( MessageKey ) && string.IsNullOrEmpty( cloneMessage.MessageKey ) ) {
						checkResult = true;
						return;
					}

					if( MessageKey != cloneMessage.MessageKey ) {
						return;
					}

					checkResult = true;
				} );

				if( !checkResult ) {
					return;
				}

				DoActionOnDispatcher( () => InvokeActions( cloneMessage ) );

				message.IsHandled = cloneMessage.IsHandled;

				if( message is IResponsiveMessage responsiveMessage ) {
					responsiveMessage.Response = ( (IResponsiveMessage)cloneMessage ).Response;
				}
				if( message is ShowWindowMessage showWindowMessage ) {
					showWindowMessage.ViewModel = ( (ShowWindowMessage)cloneMessage ).ViewModel;
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

		protected override void OnAttached() {
			base.OnAttached();

			if( AssociatedObject != null ) {
				AssociatedObject.Loaded += AssociatedObjectLoaded;
				AssociatedObject.Unloaded += AssociatedObjectUnloaded;
			}
		}

		void AssociatedObjectLoaded( object sender, RoutedEventArgs e ) {
			_IsLoaded = true;
		}

		void AssociatedObjectUnloaded( object sender, RoutedEventArgs e ) {
			_IsLoaded = false;
		}

		protected override void OnDetaching() {
			if( Messenger != null ) {
				_listener?.Dispose();
			}

			if( AssociatedObject != null ) {
				AssociatedObject.Loaded -= AssociatedObjectLoaded;
				AssociatedObject.Unloaded -= AssociatedObjectUnloaded;
			}

			base.OnDetaching();
		}


		private bool _disposed;

		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;

			_listener?.Dispose();
			_disposed = true;
		}
	}
}