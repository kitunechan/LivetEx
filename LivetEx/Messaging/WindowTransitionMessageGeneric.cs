using System.Windows;
using System;

namespace LivetEx.Messaging {
	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "ViewModel" )]
	public class WindowTransitionMessage<T>: WindowTransitionMessage where T: ViewModel{
		public WindowTransitionMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public WindowTransitionMessage( string messageKey, WindowTransitionMode mode )
			: this( messageKey, null, null, mode ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( string messageKey, T ViewModel )
			: this( messageKey, null, ViewModel, WindowTransitionMode.UnKnown ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelと画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( T ViewModel, WindowTransitionMode mode, string messageKey )
			: this( messageKey, null, ViewModel, mode ) { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="viewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowTransitionMessage( string messageKey, Type windowType, T viewModel, WindowTransitionMode mode )
			: base( messageKey ) {
			Mode = mode;
			ViewModel = viewModel;

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
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		public WindowTransitionMessage( T ViewModel )
			: this( null, null, ViewModel, WindowTransitionMode.UnKnown) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelと画面遷移モードを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public WindowTransitionMessage( T ViewModel, WindowTransitionMode mode )
			: this( null, null, ViewModel, mode ) { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public WindowTransitionMessage( Type windowType, T ViewModel, WindowTransitionMode mode )
			: this( null, windowType, ViewModel, mode ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		new public T ViewModel {
			get { return (T)GetValue( ViewModelProperty ); }
			set { SetValue( ViewModelProperty, value ); }
		}


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowTransitionMessage<T>( MessageKey, ViewModel );
		}
	}
}
