using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// １つだけプロパティを設定できる相互作用メッセージの抽象基底クラスです。
	/// </summary>
	public abstract class MessageOneParameter : Message {
		public MessageOneParameter() {

		}

		public MessageOneParameter( string messageKey ) : base( messageKey ) {
		}


		public MessageOneParameter( string messageKey, object value ) : base( messageKey ) {
			Value = value;
		}


		public object Value {
			get { return (object)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register( "Value", typeof( object ), typeof( MessageOneParameter ), new PropertyMetadata( defaultValue: default( object ) ) );

	}
}
