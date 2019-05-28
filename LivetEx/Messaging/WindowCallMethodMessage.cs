using System.Windows;

namespace LivetEx.Messaging {
	/// <summary>
	/// ウインドウのメソッドを呼び出すメッセージです。
	/// </summary>
	public class WindowCallMethodMessage<T> : Message<T> {

		public WindowCallMethodMessage() { }

		public WindowCallMethodMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowCallMethodMessage<T>();
		}


		#region Register MethodName
		public string MethodName {
			get => (string)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( WindowCallMethodMessage<T> ), new PropertyMetadata( default( string ) ) );
		#endregion

		#region Register MethodParameter
		public T MethodParameter {
			get => Value;
			set {
				SetValue( MethodParameterProperty, value );
				Value = value;
			}
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( WindowCallMethodMessage<T> ), new PropertyMetadata( default( object ) ) );
		#endregion

	}

	/// <summary>
	/// ウインドウの返り値のあるメソッドを呼び出すメッセージです。
	/// </summary>
	public class WindowCallResponseMethodMessage<TValue, TResponse> : ResponsiveMessage<TValue, TResponse> {
		public WindowCallResponseMethodMessage() { }

		/// <summary>
		/// メッセージキーを指定して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowCallResponseMethodMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowCallResponseMethodMessage<TValue, TResponse>();
		}

		#region Register MethodName
		public string MethodName {
			get => (string)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( WindowCallResponseMethodMessage<TValue, TResponse> ), new PropertyMetadata( default( string ) ) );
		#endregion

		#region Register MethodParameter
		public TValue MethodParameter {
			get => Value;
			set {
				SetValue( MethodParameterProperty, value );
				Value = value;
			}
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( WindowCallResponseMethodMessage<TValue, TResponse> ), new PropertyMetadata( default( object ) ) );
		#endregion

	}

	/// <summary>
	/// ウインドウの返り値のあるメソッドを呼び出すメッセージです。
	/// </summary>
	public class WindowCallResponseMethodMessage<TResponse> : ResponsiveMessage<TResponse> {
		public WindowCallResponseMethodMessage() { }

		/// <summary>
		/// メッセージキーを指定して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowCallResponseMethodMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new WindowCallResponseMethodMessage<TResponse>();
		}

		#region Register MethodName
		public string MethodName {
			get => (string)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( WindowCallResponseMethodMessage<TResponse> ), new PropertyMetadata( default( string ) ) );
		#endregion

	}

}