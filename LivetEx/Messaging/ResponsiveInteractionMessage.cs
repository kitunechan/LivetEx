using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// 戻り値のある相互作用メッセージの抽象基底クラスです。
	/// </summary>
	public interface IResponsiveInteractionMessage {
		object Response { get; set; }

	}

	/// <summary>
	/// 戻り値のある相互作用メッセージの基底クラスです。
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ResponsiveInteractionMessage<T> : InteractionMessage, IResponsiveInteractionMessage {
		public ResponsiveInteractionMessage() {
		}

		/// <summary>
		/// メッセージキーを使用して、戻り値のある新しい相互作用メッセージのインスタンスを生成します
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public ResponsiveInteractionMessage( string messageKey )
			: base( messageKey ) {
		}


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new ResponsiveInteractionMessage<T>();
		}

		/// <summary>
		/// 戻り値情報
		/// </summary>
		public T Response { get; set; }

		object IResponsiveInteractionMessage.Response {
			get => this.Response;
			set => this.Response = (T)value;
		}
	}
}