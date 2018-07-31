using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// 確認相互作用メッセージを表します。
	/// </summary>
	public class CallMethodMessage : InteractionMessage {

		public CallMethodMessage() { }

		/// <summary>
		/// メッセージキーを指定して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public CallMethodMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new CallMethodMessage();
		}


		#region Register MethodName
		public string MethodName {
			get => (string)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallMethodMessage ), new PropertyMetadata( default( string ) ) );
		#endregion



		#region Register MethodParameter
		public object MethodParameter {
			get => (object)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( CallMethodMessage ), new PropertyMetadata( default( object ) ) );
		#endregion

	}

	public class CallMethodMessageGeneric<T> : CallMethodMessageGeneric {



	}

	public class CallMethodMessageGeneric : InteractionMessage, IResponsiveInteractionMessage {
		public CallMethodMessageGeneric() { }

		/// <summary>
		/// メッセージキーを指定して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public CallMethodMessageGeneric( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new CallMethodMessage();
		}


		#region Register MethodName
		public string MethodName {
			get => (string)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallMethodMessage ), new PropertyMetadata( default( string ) ) );
		#endregion



		#region Register MethodParameter
		public object MethodParameter {
			get => (object)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( CallMethodMessage ), new PropertyMetadata( default( object ) ) );
		#endregion

		public object Response { get; set; }
	}


}