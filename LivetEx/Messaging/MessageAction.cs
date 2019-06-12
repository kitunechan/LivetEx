using System.Windows;
using System.Windows.Interactivity;

using System.ComponentModel;

namespace LivetEx.Messaging {
	/// <summary>
	/// ViewModelからの相互作用メッセージに対応するアクションの基底抽象クラスです<br/>
	/// 独自のアクションを定義する場合はこのクラスを継承してください。
	/// </summary>
	/// <typeparam name="T">このアクションがアタッチ可能な型を示します。</typeparam>
	[System.Windows.Markup.ContentProperty( "DirectMessage" )]
	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes" )]
	public abstract class MessageAction<T, TMessage> : TriggerAction<T> where T : DependencyObject where TMessage : Message {
		protected override sealed void Invoke( object parameter ) {
			if( (bool)( DesignerProperties.IsInDesignModeProperty.GetMetadata( typeof( DependencyObject ) ).DefaultValue ) ) return;

			var window = Window.GetWindow( this.AssociatedObject );
			if( window == null ) {
				return;
			}

			if( ( !window.IsActive ) && InvokeActionOnlyWhenWindowIsActive ) {
				return;
			}

			if( ( DirectMessage?.Message ?? parameter ) is TMessage message ) {
				InvokeAction( message );

				if( DirectMessage != null ) {
					DirectMessage.InvokeCallbacks( message );
				}
			}
		}

		protected abstract void InvokeAction( TMessage message );

		/// <summary>
		/// Viewで直接相互作用メッセージを定義する場合に使用する、<see cref="Messaging.DirectMessage"/>を指定、または取得します。
		/// </summary>
		public DirectMessage DirectMessage {
			get { return (DirectMessage)GetValue( DirectMessageProperty ); }
			set { SetValue( DirectMessageProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for DirectMessage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DirectMessageProperty =
			DependencyProperty.Register( nameof( DirectMessage ), typeof( DirectMessage ), typeof( MessageAction<T, TMessage> ), new PropertyMetadata() );


		/// <summary>
		/// Windowがアクティブな時のみアクションを実行するかどうかを指定、または取得します。初期値はfalseです。
		/// </summary>
		public bool InvokeActionOnlyWhenWindowIsActive {
			get { return (bool)GetValue( InvokeActionOnlyWhenWindowIsActiveProperty ); }
			set { SetValue( InvokeActionOnlyWhenWindowIsActiveProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for InvokeActionOnlyWhenWindowIsActive.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty InvokeActionOnlyWhenWindowIsActiveProperty =
			DependencyProperty.Register( nameof( InvokeActionOnlyWhenWindowIsActive ), typeof( bool ), typeof( MessageAction<T, TMessage> ), new PropertyMetadata( false ) );


	}
}
