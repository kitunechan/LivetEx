using System.Windows;
using System;

namespace LivetEx.Messaging {
	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "ViewModel" )]
	public class WindowTransitionMessage : ResponsiveInteractionMessage<bool?> {
		/// <summary>
		/// 相互作用メッセージのインスタンスを生成します。
		/// </summary>
		public WindowTransitionMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( string messageKey ) : base( messageKey ) { }


		/// <summary>
		/// Windowの型、Windowに設定するViewModel、メッセージキーを指定して相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="viewModel">新しいWindowのDataContextに設定するViewModel</param>
		public WindowTransitionMessage( string messageKey, Type windowType, ViewModel viewModel )
			: base( messageKey ) {
			ViewModel = viewModel;

			if( windowType != null ) {
				if( !windowType.IsSubclassOf( typeof( Window ) ) ) {
					throw new ArgumentException( "Windowの派生クラスを指定してください。", "windowType" );
				}
			}

			WindowType = windowType;
		}


		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		public ViewModel ViewModel {
			get { return (ViewModel)GetValue( ViewModelProperty ); }
			set { SetValue( ViewModelProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ViewModelProperty =
			DependencyProperty.Register( nameof( ViewModel ), typeof( ViewModel ), typeof( WindowTransitionMessage ), new PropertyMetadata( null ) );


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



		#region Register WindowState
		public WindowState WindowState {
			get { return (WindowState)GetValue( WindowStateProperty ); }
			set { SetValue( WindowStateProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for WindowState.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WindowStateProperty =
			DependencyProperty.Register( nameof( WindowState ), typeof( WindowState ), typeof( WindowTransitionMessage ), new PropertyMetadata( WindowState.Normal ) );
		#endregion


		#region Register WindowStartupLocation
		public WindowStartupLocation? WindowStartupLocation {
			get => (WindowStartupLocation?)GetValue( WindowStartupLocationProperty );
			set => SetValue( WindowStartupLocationProperty, value );
		}

		public static readonly DependencyProperty WindowStartupLocationProperty =
			DependencyProperty.Register( nameof( WindowStartupLocation ), typeof( WindowStartupLocation? ), typeof( WindowTransitionMessage ), new PropertyMetadata( default( WindowStartupLocation? ) ) );
		#endregion


		/// <summary>
		/// ウインドウの設定を行う関数
		/// </summary>
		public Action<Window> WindowSettingAction { get; set; }

		/// <summary>
		/// ウインドウコンテンツがレンダリングされた後に実行する関数
		/// </summary>
		public Action<Window> InitializeAction { get; set; }


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowTransitionMessage();
		}


		/// <summary>
		/// DependencyProperty 以外のものはここでコピー処理を行う
		/// </summary>
		protected override void CloneCore( Freezable sourceFreezable ) {
			base.CloneCore( sourceFreezable );

			var source = (WindowTransitionMessage)sourceFreezable;

			this.InitializeAction = source.InitializeAction;
			this.WindowSettingAction = source.WindowSettingAction;
		}


	}
}
