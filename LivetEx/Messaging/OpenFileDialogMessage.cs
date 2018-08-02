using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// ファイルを開く アクション用の相互作用メッセージです。
	/// </summary>
	public class OpenFileDialogMessage : FileDialogMessage {
		public OpenFileDialogMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public OpenFileDialogMessage( string messageKey )
			: base( messageKey ) {
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new OpenFileDialogMessage();
		}

		/// <summary>
		/// 複数ファイルを選択可能かを取得、または設定します。
		/// </summary>
		public bool MultiSelect {
			get { return (bool)GetValue( MultiSelectProperty ); }
			set { SetValue( MultiSelectProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MultiSelect.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MultiSelectProperty =
			DependencyProperty.Register( "MultiSelect", typeof( bool ), typeof( OpenFileDialogMessage ), new PropertyMetadata( false ) );


		/// <summary>
		/// ファイル ダイアログに表示される初期ディレクトリのグループを取得または設定します。
		/// </summary>
		#region Register InitialDirectoryGroup
		public string InitialDirectoryGroup {
			get { return (string)GetValue( InitialDirectoryGroupProperty ); }
			set { SetValue( InitialDirectoryGroupProperty, value ); }
		}


		// Using a DependencyProperty as the backing store for InitialDirectoryGroup.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty InitialDirectoryGroupProperty =
			DependencyProperty.Register( "InitialDirectoryGroup", typeof( string ), typeof( OpenFileDialogMessage ), new PropertyMetadata( null ) );
		#endregion

	}
}