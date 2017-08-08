using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// ファイルを開く・ファイルを保存するアクション用の共通相互作用メッセージ基底抽象クラスです。<br/>
	/// ファイルを開くアクションをViewに行わせたい場合は、<see cref="FileDialogMessage"/>を使用してください。<br/>
	/// ファイルを保存するアクションをViewに行わせたい場合は、<see cref="SaveFileDialogMessage"/>を使用してください。
	/// </summary>
	public abstract class FileDialogMessage : ResponsiveInteractionMessage<string[]> {
		protected FileDialogMessage() {
		}

		protected FileDialogMessage( string messageKey )
			: base( messageKey ) {
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected abstract override Freezable CreateInstanceCore();


		/// <summary>
		/// ダイアログタイトルを指定、または取得します。
		/// </summary>
		public string Title {
			get { return (string)GetValue( TitleProperty ); }
			set { SetValue( TitleProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register( "Title", typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( null ) );

		/// <summary>
		/// ファイルの拡張子Filterを指定、または取得します。
		/// </summary>
		public string Filter {
			get { return (string)GetValue( FilterProperty ); }
			set { SetValue( FilterProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register( "Filter", typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( null ) );


		/// <summary>
		/// ファイル ダイアログで現在選択されているフィルターのインデックスを取得または設定します。 既定値は1です。
		/// </summary>
		#region Register FilterIndex
		public int FilterIndex {
			get { return (int)GetValue( FilterIndexProperty ); }
			set { SetValue( FilterIndexProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for FilterIndex.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FilterIndexProperty =
			DependencyProperty.Register( "FilterIndex", typeof( int ), typeof( FileDialogMessage ), new PropertyMetadata( 1 ) );
		#endregion



		/// <summary>
		/// 拡張子を指定しなかった場合、自動で拡張子を追加するかどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool AddExtension {
			get { return (bool)GetValue( AddExtensionProperty ); }
			set { SetValue( AddExtensionProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for AddExtension.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AddExtensionProperty =
			DependencyProperty.Register( "AddExtension", typeof( bool ), typeof( FileDialogMessage ), new PropertyMetadata( true ) );

		/// <summary>
		/// ダイアログに表示される初期ディレクトリを指定、または取得します。
		/// </summary>
		public string InitialDirectory {
			get { return (string)GetValue( InitialDirectoryProperty ); }
			set { SetValue( InitialDirectoryProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for InitialDirectory.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty InitialDirectoryProperty =
			DependencyProperty.Register( "InitialDirectory", typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( null ) );


		/// <summary>
		/// ファイルダイアログで指定されたファイルのパスを含む文字列を指定、または取得します。
		/// </summary>
		public string FileName {
			get { return (string)GetValue( FileNameProperty ); }
			set { SetValue( FileNameProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FileNameProperty =
			DependencyProperty.Register( "FileName", typeof( string ), typeof( FileDialogMessage ), new PropertyMetadata( null ) );




	}
}