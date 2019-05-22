using System.Windows;
using System;

namespace LivetEx.Messaging {
	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "ViewModel" )]
	public class WindowMessage<TWindow> : WindowMessage where TWindow : Window {
		public WindowMessage() : base() {
			this.WindowType = typeof( TWindow );
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowMessage( string messageKey ) : base( messageKey ) {
			this.WindowType = typeof( TWindow );
		}

		/// <summary>
		/// ウインドウの設定を行う関数
		/// </summary>
		public new Action<TWindow> WindowSettingAction {
			get => base.WindowSettingAction;
			set => base.WindowSettingAction = window => value?.Invoke( (TWindow)window );
		}

		/// <summary>
		/// ウインドウコンテンツがレンダリングされた後に実行する関数
		/// </summary>
		public new Action<TWindow> InitializeAction {
			get => base.InitializeAction;
			set => base.InitializeAction = window => value?.Invoke( (TWindow)window );
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowMessage<TWindow>();
		}


	}

	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "ViewModel" )]
	public class WindowMessage<TWindow, TViewModel> : WindowMessage<TWindow> where TWindow : Window where TViewModel : ViewModel {
		public WindowMessage() : base() {

		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowMessage( string messageKey ) : base( messageKey ) { }


		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="viewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowMessage( string messageKey, TViewModel viewModel ) : base( messageKey ) {
			this.ViewModel = viewModel;
		}

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		public WindowMessage( TViewModel viewModel ) : this() {
			this.ViewModel = viewModel;
		}


		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		public new TViewModel ViewModel {
			get { return (TViewModel)GetValue( ViewModelProperty ); }
			set { SetValue( ViewModelProperty, value ); }
		}


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowMessage<TWindow, TViewModel>();
		}
	}

	public static class WindowMessageGenerator<TWindow> where TWindow : Window {
		public static WindowMessage<TWindow, TViewModel> Create<TViewModel>( TViewModel viewModel ) where TViewModel : ViewModel {
			return new WindowMessage<TWindow, TViewModel>( viewModel ) {
				WindowType = typeof( TWindow )

			};
		}

		public static WindowMessage<TWindow, TViewModel> Create<TViewModel>( string messageKey, TViewModel viewModel ) where TViewModel : ViewModel {
			return new WindowMessage<TWindow, TViewModel>( messageKey, viewModel ) {
				WindowType = typeof( TWindow )

			};
		}
	}
}
