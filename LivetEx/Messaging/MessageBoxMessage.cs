using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// メッセージボックスを表示ズルメッセージです。
	/// </summary>
	public class MessageBoxMessage : ResponsiveMessage<MessageBoxResult> {

		public MessageBoxMessage() { }

		/// <summary>
		/// メッセージキーを指定して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public MessageBoxMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new MessageBoxMessage();
		}

		/// <summary>
		/// メッセージボックスがアクションの親ウインドウに所有されるかを設定します。
		/// </summary>
		#region Register IsOwnedProperty
		public bool? IsOwned {
			get { return (bool?)GetValue( IsOwnedProperty ); }
			set { SetValue( IsOwnedProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for IsOwnedProperty.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsOwnedProperty =
			DependencyProperty.Register( "IsOwnedProperty", typeof( bool? ), typeof( MessageBoxMessage ), new PropertyMetadata( null ) );
		#endregion


		/// <summary>
		/// 表示するメッセージを指定、または取得します。
		/// </summary>
		public string Text {
			get { return (string)GetValue( TextProperty ); }
			set { SetValue( TextProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register( "Text", typeof( string ), typeof( MessageBoxMessage ), new PropertyMetadata( null ) );


		/// <summary>
		/// キャプション（タイトル部分）を指定、または取得します。
		/// </summary>
		public string Caption {
			get { return (string)GetValue( CaptionProperty ); }
			set { SetValue( CaptionProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register( "Caption", typeof( string ), typeof( MessageBoxMessage ), new PropertyMetadata( null ) );



		/// <summary>
		/// メッセージボックスイメージを指定、または取得します。
		/// </summary>
		public MessageBoxImage Image {
			get { return (MessageBoxImage)GetValue( ImageProperty ); }
			set { SetValue( ImageProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register( "Image", typeof( MessageBoxImage ), typeof( MessageBoxMessage ), new PropertyMetadata() );


		/// <summary>
		/// メッセージボックスボタンを指定、または取得します。
		/// </summary>
		public MessageBoxButton Button {
			get { return (MessageBoxButton)GetValue( ButtonProperty ); }
			set { SetValue( ButtonProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Button.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ButtonProperty =
			DependencyProperty.Register( "Button", typeof( MessageBoxButton ), typeof( MessageBoxMessage ), new PropertyMetadata( MessageBoxButton.OK ) );

		/// <summary>
		/// メッセージボックスの既定の結果を指定、または取得します。
		/// </summary>
		public MessageBoxResult DefaultResult {
			get { return (MessageBoxResult)GetValue( DefaultResultProperty ); }
			set { SetValue( DefaultResultProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for DefaultResult.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DefaultResultProperty =
			DependencyProperty.Register( "DefaultResult", typeof( MessageBoxResult ), typeof( MessageBoxMessage ), new PropertyMetadata( MessageBoxResult.OK ) );
	}
}