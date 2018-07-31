﻿using System;
using System.Windows.Interactivity;
using System.Windows;
using LivetEx.WeakEventListeners;

namespace LivetEx.Messaging {
	/// <summary>
	/// ViewModelからの相互作用メッセージを受信し、アクションを実行します。
	/// </summary>
	public class InteractionMessageTrigger : TriggerBase<FrameworkElement>, IDisposable {
		private LivetWeakEventListener<EventHandler<InteractionMessageRaisedEventArgs>, InteractionMessageRaisedEventArgs> _listener;
		
		private bool _loaded = true;

		/// <summary>
		/// ViewModelのMessengerを指定、または取得します。
		/// </summary>
		public InteractionMessenger Messenger {
			get { return (InteractionMessenger)GetValue( MessengerProperty ); }
			set { SetValue( MessengerProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Messenger.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessengerProperty =
			DependencyProperty.Register( "Messenger", typeof( InteractionMessenger ), typeof( InteractionMessageTrigger ),
										new PropertyMetadata( MessengerChanged ) );


		/// <summary>
		/// アタッチされたオブジェクトがロードされている期間(Loaded~Unloaded)だけActionを実行するかどうかを指定、または取得します。デフォルトはfalseです。
		/// </summary>
		public bool InvokeActionsOnlyWhileAttatchedObjectLoaded {
			get { return (bool)GetValue( InvokeActionsOnlyWhileAttatchedObjectLoadedProperty ); }
			set { SetValue( InvokeActionsOnlyWhileAttatchedObjectLoadedProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for FireActionsOnlyWhileAttatchedObjectLoading.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty InvokeActionsOnlyWhileAttatchedObjectLoadedProperty =
			DependencyProperty.Register( "InvokeActionsOnlyWhileAttatchedObjectLoaded", typeof( bool ), typeof( InteractionMessageTrigger ), new PropertyMetadata( false ) );



		/// <summary>
		/// このトリガーが有効かどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool IsEnable {
			get { return (bool)GetValue( IsEnableProperty ); }
			set { SetValue( IsEnableProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for IsEnable.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsEnableProperty =
			DependencyProperty.Register( "IsEnable", typeof( bool ), typeof( InteractionMessageTrigger ), new PropertyMetadata( true ) );



		/// <summary>
		/// このトリガーが反応する相互作用メッセージのメッセージキーを指定、または取得します。<br/>
		/// このプロパティが指定されていない場合、このトリガーは全ての相互作用メッセージに反応します。
		/// </summary>
		public string MessageKey { get; set; }

		private static void MessengerChanged( DependencyObject obj, DependencyPropertyChangedEventArgs e ) {
			var sender = (InteractionMessageTrigger)obj;

			if( e.OldValue == e.NewValue ) {
				return;
			}

			if( e.OldValue != null ) {
				sender._listener?.Dispose();
			}

			if( e.NewValue != null ) {
				var newMessenger = (InteractionMessenger)e.NewValue;

				sender._listener = new LivetWeakEventListener<EventHandler<InteractionMessageRaisedEventArgs>, InteractionMessageRaisedEventArgs>(
					h => h,
					h => newMessenger.Raised += h,
					h => newMessenger.Raised -= h,
					sender.MessageReceived
				);
			}
		}

		private void MessageReceived( object sender, InteractionMessageRaisedEventArgs e ) {
			var message = e.Message;

			var cloneMessage = (InteractionMessage)message.Clone();
				cloneMessage.Freeze();

			var checkResult = false;
			DoActionOnDispatcher( () => {
				if( !IsEnable ) {
					return;
				}

				if( InvokeActionsOnlyWhileAttatchedObjectLoaded && ( !_loaded ) ) {
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

			if( message is IResponsiveInteractionMessage responsiveMessage ) {
				responsiveMessage.Response = ( (IResponsiveInteractionMessage)cloneMessage ).Response;
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
			_loaded = true;
		}

		void AssociatedObjectUnloaded( object sender, RoutedEventArgs e ) {
			_loaded = false;
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