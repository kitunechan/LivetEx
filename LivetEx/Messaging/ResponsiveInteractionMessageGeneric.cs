using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LivetEx.Messaging {

	/// <summary>
	/// 戻り値のある相互作用メッセージの基底クラスです。
	/// </summary>
	public abstract class ResponsiveInteractionMessageOneParameter : ResponsiveInteractionMessage {

		public ResponsiveInteractionMessageOneParameter( string messageKey )
			: base( messageKey ) {
		}

		public ResponsiveInteractionMessageOneParameter( string messageKey, object value )
			: this( messageKey ) {
			Value = value;
		}


		public object Value {
			get { return (object)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( "Value", typeof( object ), typeof( ResponsiveInteractionMessageOneParameter ), new PropertyMetadata( default( object ) ) );

	}

	/// <summary>
	/// 戻り値のある相互作用メッセージの基底クラスです。
	/// </summary>
	public class ResponsiveInteractionMessage<TValue, TResponse> : ResponsiveInteractionMessageOneParameter {

		public ResponsiveInteractionMessage( string messageKey )
			: base( messageKey ) {
		}

		public ResponsiveInteractionMessage( string messageKey, TValue value )
			: this( messageKey ) {
			Value = value;
		}


		new public TValue Value {
			get { return (TValue)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		/// <summary>
		/// 戻り値情報
		/// </summary>
		public new TResponse Response {
			get { return (TResponse)base.Response; }
			set { base.Response = value; }
		}

		protected override Freezable CreateInstanceCore() {
			return new ResponsiveInteractionMessage<TValue, TResponse>( MessageKey, Value );
		}

	}
}
