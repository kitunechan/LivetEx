using System.Windows;
using System;

namespace LivetEx.Messaging {
	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	[System.Windows.Markup.ContentProperty( "TransitionViewModel" )]
	public class TransitionMessage<T>: TransitionMessage where T: ViewModel{
		public TransitionMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public TransitionMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public TransitionMessage( string messageKey, TransitionMode mode )
			: this( messageKey, null, null, mode ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="messageKey">メッセージキー</param>
		public TransitionMessage( string messageKey, T transitionViewModel )
			: this( messageKey, null, transitionViewModel, TransitionMode.UnKnown ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelと画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		/// <param name="messageKey">メッセージキー</param>
		public TransitionMessage( T transitionViewModel, TransitionMode mode, string messageKey )
			: this( messageKey, null, transitionViewModel, mode ) { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		/// <param name="messageKey">メッセージキー</param>
		public TransitionMessage( string messageKey, Type windowType, T transitionViewModel, TransitionMode mode )
			: base( messageKey ) {
			Mode = mode;
			TransitionViewModel = transitionViewModel;

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
		public TransitionMessage( T transitionViewModel )
			: this( null, null, transitionViewModel, TransitionMode.UnKnown) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelと画面遷移モードを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public TransitionMessage( T transitionViewModel, TransitionMode mode )
			: this( null, null, transitionViewModel, mode ) { }

		/// <summary>
		/// 新しいWindowの型と新しいWindowに設定するViewModel、画面遷移モードとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="transitionViewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="mode">画面遷移の方法を決定するTransitionMode列挙体。初期値はUnKnownです。</param>
		public TransitionMessage( Type windowType, T transitionViewModel, TransitionMode mode )
			: this( null, windowType, transitionViewModel, mode ) { }

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		new public T TransitionViewModel {
			get { return (T)GetValue( TransitionViewModelProperty ); }
			set { SetValue( TransitionViewModelProperty, value ); }
		}


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new TransitionMessage<T>( MessageKey, TransitionViewModel );
		}
	}
}
