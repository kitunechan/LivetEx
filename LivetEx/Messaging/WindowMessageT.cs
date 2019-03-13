using System.Windows;
using System;

namespace LivetEx.Messaging {
	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "ViewModel" )]
	public class WindowMessage<VM> : WindowMessage where VM : ViewModel {
		public WindowMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowMessage( string messageKey ) : base( messageKey ) { }


		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowMessage( string messageKey, VM ViewModel )
			: this( messageKey, null, ViewModel ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		public WindowMessage( VM ViewModel )
			: this( null, null, ViewModel ) { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="viewModel">新しいWindowのDataContextに設定するViewModel</param>
		public WindowMessage( string messageKey, Type windowType, VM viewModel )
			: base( messageKey, windowType, viewModel ) {
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
		public new VM ViewModel {
			get { return (VM)GetValue( ViewModelProperty ); }
			set { SetValue( ViewModelProperty, value ); }
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowMessage<VM>();
		}
	}


	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "ViewModel" )]
	public class WindowMessageV<TWindow> : WindowMessage where TWindow : Window {
		public WindowMessageV() {
			this.WindowType = this.GetType().GetGenericArguments()[0];
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowMessageV( string messageKey ) : base( messageKey ) {
			this.WindowType = this.GetType().GetGenericArguments()[0];
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
			return new WindowMessageV<TWindow>();
		}


	}
}
