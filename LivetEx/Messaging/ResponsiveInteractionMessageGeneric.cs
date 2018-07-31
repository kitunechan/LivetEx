using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LivetEx.Messaging {

	/// <summary>
	/// 戻り値のある相互作用メッセージの基底クラスです。
	/// </summary>
	public interface IResponsiveInteractionMessageOneParameter : IResponsiveInteractionMessage {
		object Value { get; set; }
	}

	/// <summary>
	/// 戻り値のある相互作用メッセージの基底クラスです。
	/// </summary>
	public class ResponsiveInteractionMessage<TValue, TResponse> : InteractionMessage, IResponsiveInteractionMessageOneParameter {
		public ResponsiveInteractionMessage() {

		}

		public ResponsiveInteractionMessage( string messageKey ) : base( messageKey ) {
		}

		public ResponsiveInteractionMessage( string messageKey, TValue value ) : this( messageKey ) {
			Value = value;
		}

		protected override Freezable CreateInstanceCore() {
			return new ResponsiveInteractionMessage<TValue, TResponse>();
		}

		public TValue Value { get; set; }
		object IResponsiveInteractionMessageOneParameter.Value {
			get => this.Value;
			set => this.Value = (TValue)value;
		}


		/// <summary>
		/// 戻り値情報
		/// </summary>
		public TResponse Response { get; set; }

		object IResponsiveInteractionMessage.Response {
			get => this.Response;
			set => this.Response = (TResponse)value;
		}

	}
}
