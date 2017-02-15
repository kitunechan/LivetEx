using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// １つだけプロパティを設定できる相互作用メッセージの抽象基底クラスです。
	/// </summary>
	public abstract class GenericInteractionMessage: InteractionMessage {
		public GenericInteractionMessage( string messageKey )
			: base( messageKey ) {
		}


		public GenericInteractionMessage( string messageKey, object value )
			: base( messageKey ) {
			Value = value;
		}


		internal object Value {
			get { return (object)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( "Value", typeof( object ), typeof( GenericInteractionMessage ), new PropertyMetadata( defaultValue : default( object ) ) );
		
	}

	/// <summary>
	/// １つだけプロパティを設定できる相互作用メッセージの基底クラスです。
	/// </summary>
	public class GenericInteractionMessage<T>: GenericInteractionMessage {
		public GenericInteractionMessage( string messageKey )
			: base( messageKey ) {
		}


		public GenericInteractionMessage( string messageKey, T value )
			: base( messageKey ) {
			Value = value;
		}


		public new T Value {
			get { return (T)base.Value; }
			set { base.Value = value; }
		}

		protected override Freezable CreateInstanceCore() {
			return new GenericInteractionMessage<T>( MessageKey, Value );
		}
	}
}
