using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// ファイルを保存する 用の相互作用メッセージです。
	/// </summary>
	public class SaveFileDialogMessage : FileDialogMessage {
		public SaveFileDialogMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public SaveFileDialogMessage( string messageKey )
			: base( messageKey ) {
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new SaveFileDialogMessage( MessageKey );
		}

		/// <summary>
		/// ユーザーが存在しないファイルを指定した場合に、ファイルを作成することを確認するメッセージを表示するかどうかを指定、または取得します。デフォルトはfalseです。
		/// </summary>
		public bool CreatePrompt {
			get { return (bool)GetValue( CreatePromptProperty ); }
			set { SetValue( CreatePromptProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for CreatePrompt.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CreatePromptProperty =
			DependencyProperty.Register( "CreatePrompt", typeof( bool ), typeof( SaveFileDialogMessage ), new PropertyMetadata( false ) );


		/// <summary>
		/// ユーザーが指定したファイルが存在する場合、上書き確認メッセージを表示するかどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool OverwritePrompt {
			get { return (bool)GetValue( OverwritePromptProperty ); }
			set { SetValue( OverwritePromptProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for OverwritePrompt.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OverwritePromptProperty =
			DependencyProperty.Register( "OverwritePrompt", typeof( bool ), typeof( SaveFileDialogMessage ), new PropertyMetadata( true ) );


		#region Register DefaultExt
		public string DefaultExt {
			get { return (string)GetValue( DefaultExtProperty ); }
			set { SetValue( DefaultExtProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for DefaultExt.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DefaultExtProperty =
			DependencyProperty.Register( "DefaultExt", typeof( string ), typeof( SaveFileDialogMessage ), new PropertyMetadata( null ) );
		#endregion


	}
}