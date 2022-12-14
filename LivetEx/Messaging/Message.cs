using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// 相互作用メッセージの基底クラスです。<br/>
	/// Viewからのアクション実行後、戻り値情報が必要ない相互作用メッセージを作成する場合はこのクラスを継承して相互作用メッセージを作成します。
	/// </summary>
	public class Message : Freezable {
		public Message() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public Message( string messageKey ) {
			MessageKey = messageKey;
		}

		/// <summary>
		/// メッセージキーを指定、または取得します。
		/// </summary>
		public string MessageKey {
			get => (string)GetValue( MessageKeyProperty );
			set => SetValue( MessageKeyProperty, value );
		}

		// Using a DependencyProperty as the backing store for MessageKey.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MessageKeyProperty =
			DependencyProperty.Register( nameof( MessageKey ), typeof( string ), typeof( Message ), new PropertyMetadata( null ) );

		public bool IsHandled { get; set; }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new Message();
		}

		protected override void CloneCore( Freezable sourceFreezable ) {
			this.IsHandled = ((Message)sourceFreezable).IsHandled;

			base.CloneCore( sourceFreezable );
		}
	}
}