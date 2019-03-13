using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LivetEx.Messaging {

	/// <summary>
	/// １つだけプロパティを設定できる相互作用メッセージの基底クラスです。
	/// </summary>
	public class Message<T> : MessageOneParameter {
		public Message() {

		}

		public Message( string messageKey ) : base( messageKey ) {
		}


		public Message( string messageKey, T value ) : base( messageKey ) {
			Value = value;
		}


		public new T Value {
			get { return (T)GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}


		protected override Freezable CreateInstanceCore() {
			return new Message<T>();
		}
	}
}
