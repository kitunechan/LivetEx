using System.Windows;
using System;

namespace LivetEx.Messaging {
	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "TransitionViewModel" )]
	public class WindowTransitionMessage: ResponsiveInteractionMessage<bool?> {
		public WindowTransitionMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// メッセージキーと画面遷移モードを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public WindowTransitionMessage( string messageKey, WindowTransitionMode mode )
			: this( null, null, mode, messageKey ) { }


		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( ViewModel transitionViewModel, string messageKey )
			: this( null, transitionViewModel, WindowTransitionMode.UnKnown, messageKey ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelと画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( ViewModel transitionViewModel, WindowTransitionMode mode, string messageKey )
			: this( null, transitionViewModel, mode, messageKey ) { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( Type windowType, ViewModel transitionViewModel, WindowTransitionMode mode, string messageKey )
			: base( messageKey ) {
			Mode = mode;
			ViewModel = transitionViewModel;

			if( windowType != null ) {
				if( !windowType.IsSubclassOf( typeof( Window ) ) ) {
					throw new ArgumentException( "Windowの派生クラスを指定してください。", "windowType" );
				}
			}

			WindowType = windowType;
		}

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		public WindowTransitionMessage( ViewModel transitionViewModel )
			: this( null, transitionViewModel, WindowTransitionMode.UnKnown, null ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelと画面遷移モードを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public WindowTransitionMessage( ViewModel transitionViewModel, WindowTransitionMode mode )
			: this( null, transitionViewModel, mode, null ) { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public WindowTransitionMessage( Type windowType, ViewModel transitionViewModel, WindowTransitionMode mode )
			: this( windowType, transitionViewModel, mode, null ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		public ViewModel ViewModel {
			get { return (ViewModel)GetValue( ViewModelProperty ); }
			set { SetValue( ViewModelProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register( "ViewModel", typeof( ViewModel ), typeof( WindowTransitionMessage ), new PropertyMetadata( null ) );


		/// <summary>
		/// 新しいWindowの表示方法を決定するTransitionModeを指定、または取得します。<br/>
		/// 初期値はUnKnownです。
		/// </summary>
		public WindowTransitionMode Mode {
			get { return (WindowTransitionMode)GetValue( ModeProperty ); }
			set { SetValue( ModeProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register( "Mode", typeof( WindowTransitionMode ), typeof( WindowTransitionMessage ), new PropertyMetadata( WindowTransitionMode.UnKnown ) );


		/// <summary>
		/// 新しいWindowの型を指定、または取得します。
		/// </summary>
		public Type WindowType {
			get { return (Type)GetValue( WindowTypeProperty ); }
			set { SetValue( WindowTypeProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for WindowType.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WindowTypeProperty =
			DependencyProperty.Register( "WindowType", typeof( Type ), typeof( WindowTransitionMessage ), new PropertyMetadata( null ) );

		/// <summary>
		/// 遷移先ウィンドウがアクションのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool? IsOwned {
			get { return (bool?)GetValue( OwnedFromThisProperty ); }
			set { SetValue( OwnedFromThisProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for OwnedFromThis.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OwnedFromThisProperty =
			DependencyProperty.Register( "IsOwned", typeof( bool? ), typeof( WindowTransitionMessage ), new PropertyMetadata( true ) );


		public WindowStartupLocation? WindowStartupLocation { get; set; }


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowTransitionMessage( ViewModel, MessageKey );
		}
	}
}
