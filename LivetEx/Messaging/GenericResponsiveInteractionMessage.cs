using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LivetEx.Messaging {
	public class GenericResponsiveInteractionMessage<TValue, TResponse>: ResponsiveInteractionMessage<TResponse> {

		public GenericResponsiveInteractionMessage( string messageKey )
			: base( messageKey ){
		}

		public GenericResponsiveInteractionMessage( string messageKey, TValue value )
			: this( messageKey ) {
			Value = value;
		}


		public TValue Value {
			get { return (TValue)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( "Value", typeof( TValue ), typeof( GenericResponsiveInteractionMessage<TValue, TResponse> ), new PropertyMetadata( defaultValue : default( TValue ) ) );

		protected override Freezable CreateInstanceCore() {
			return new GenericResponsiveInteractionMessage<TValue, TResponse>( MessageKey, Value );
		}

	}
}
