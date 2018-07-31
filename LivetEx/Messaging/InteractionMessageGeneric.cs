using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// １つだけプロパティを設定できる相互作用メッセージの抽象基底クラスです。
	/// </summary>
	public abstract class InteractionMessageOneParameter : InteractionMessage {
		public InteractionMessageOneParameter() {

		}

		public InteractionMessageOneParameter( string messageKey ) : base( messageKey ) {
		}


		public InteractionMessageOneParameter( string messageKey, object value ) : base( messageKey ) {
			Value = value;
		}


		public object Value {
			get { return (object)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( "Value", typeof( object ), typeof( InteractionMessageOneParameter ), new PropertyMetadata( defaultValue: default( object ) ) );

	}

	/// <summary>
	/// １つだけプロパティを設定できる相互作用メッセージの基底クラスです。
	/// </summary>
	public class InteractionMessage<T> : InteractionMessageOneParameter {
		public InteractionMessage() {

		}

		public InteractionMessage( string messageKey ) : base( messageKey ) {
		}


		public InteractionMessage( string messageKey, T value ) : base( messageKey ) {
			Value = value;
		}


		public new T Value {
			get { return (T)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}


		protected override Freezable CreateInstanceCore() {
			return new InteractionMessage<T>();
		}
	}
}
