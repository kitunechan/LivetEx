using System.Windows;
using System.Windows.Input;
using LivetEx.Triggers;

namespace LivetEx.Messaging {
	/// <summary>
	/// Viewから直接相互作用メッセージを定義する際に使用します。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "Message" )]
	public class DirectMessage : Freezable {
		private MethodBinderWithArgument _callbackMethod = new MethodBinderWithArgument();

		/// <summary>
		/// 相互作用メッセージ(各種<see cref="Messaging.Message"/>)を指定、または取得します。
		/// </summary>
		public Message Message {
			get { return (Message)GetValue( MessageProperty ); }
			set { SetValue( MessageProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register( "Message", typeof( Message ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );

		/// <summary>
		/// アクション実行後に実行するコマンドを指定、または取得します<br/>
		/// このプロパティが設定されていた場合、アクションの実行後アクションの実行に使用した相互作用メッセージをパラメータとしてコマンドを呼び出します。
		/// </summary>
		public ICommand CallbackCommand {
			get { return (ICommand)GetValue( CallbackCommandProperty ); }
			set { SetValue( CallbackCommandProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for CallbackCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CallbackCommandProperty =
			DependencyProperty.Register( "CallbackCommand", typeof( ICommand ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );

		/// <summary>
		/// アクション実行後に実行するメソッドを持つオブジェクトを指定、または取得します<br/>
		/// このプロパティとCallbackMethodNameが設定されていた場合、アクションの実行後アクションの実行に使用した相互作用メッセージをパラメータとしてメソッドを呼び出します。
		/// </summary>
		public object CallbackMethodTarget {
			get { return GetValue( CallbackMethodTargetProperty ); }
			set { SetValue( CallbackMethodTargetProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for CallbackMethodTarget.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CallbackMethodTargetProperty =
			DependencyProperty.Register( "CallbackMethodTarget", typeof( object ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );

		/// <summary>
		/// アクション実行後に実行するメソッドの名前を指定、または取得します<br/>
		/// このプロパティとCallbackMethodTargetが設定されていた場合、アクションの実行後アクションの実行に使用した相互作用メッセージをパラメータとしてメソッドを呼び出します。
		/// </summary>
		public string CallbackMethodName {
			get { return (string)GetValue( CallbackMethodNameProperty ); }
			set { SetValue( CallbackMethodNameProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CallbackMethodNameProperty =
			DependencyProperty.Register( "CallbackMethodName", typeof( string ), typeof( DirectMessage ), new UIPropertyMetadata( null ) );

		internal void InvokeCallbacks( Message message ) {
			if( CallbackCommand != null ) {
				if( CallbackCommand.CanExecute( message ) ) {
					CallbackCommand.Execute( message );
				}
			}
			if( CallbackMethodTarget != null && CallbackMethodName != null ) {
				_callbackMethod.Invoke( CallbackMethodTarget, CallbackMethodName, message.GetType(), message );
			}
		}

		protected override Freezable CreateInstanceCore() {
			return new DirectMessage();
		}
	}
}
