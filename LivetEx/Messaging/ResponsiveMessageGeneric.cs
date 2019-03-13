using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LivetEx.Messaging {

	/// <summary>
	/// 戻り値のある相互作用メッセージの基底クラスです。
	/// </summary>
	public interface IResponsiveMessageOneParameter : IResponsiveMessage {
		object Value { get; set; }
	}

	/// <summary>
	/// 戻り値のある相互作用メッセージの基底クラスです。
	/// </summary>
	public class ResponsiveMessage<TValue, TResponse> : Message, IResponsiveMessageOneParameter {
		public ResponsiveMessage() {

		}

		public ResponsiveMessage( string messageKey ) : base( messageKey ) {
		}

		public ResponsiveMessage( string messageKey, TValue value ) : this( messageKey ) {
			Value = value;
		}

		protected override Freezable CreateInstanceCore() {
			return new ResponsiveMessage<TValue, TResponse>();
		}


		#region Register Value
		public TValue Value {
			get => (TValue)GetValue( ValueProperty );
			set => SetValue( ValueProperty, value );
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( nameof( Value ), typeof( TValue ), typeof( ResponsiveMessage<TValue, TResponse> ), new PropertyMetadata( default( TValue ) ) );
		#endregion

		object IResponsiveMessageOneParameter.Value {
			get => this.Value;
			set => this.Value = (TValue)value;
		}


		/// <summary>
		/// 戻り値情報
		/// </summary>
		public TResponse Response { get; set; }

		object IResponsiveMessage.Response {
			get => this.Response;
			set => this.Response = (TResponse)value;
		}

	}
}
