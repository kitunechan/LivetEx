using System;
using System.Windows;

namespace LivetEx.Messaging {


	/// <summary>
	/// メソッドの名称と引数にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallOneParameterMethodMessage : ICallMethodMessage {
		object MethodParameter { get; }
	}

	/// <summary>
	/// メソッドの名称にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallMethodMessage {
		Type MethodTarget { get; }
		string MethodName { get; }
	}

	/// <summary>
	/// メソッドの返り値にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface ICallFuncMessage : ICallMethodMessage {
		object Result { get; set; }
	}


	/// <summary>
	/// 引数の無いメソッドを呼び出すメッセージです。
	/// </summary>
	public class CallActionMessage : Message, ICallMethodMessage {
		public CallActionMessage() { }


		public CallActionMessage( Type methodTarget, string methodName ) {
			this.MethodTarget = methodTarget;
			this.MethodName = methodName;
		}

		public CallActionMessage( string methodName ) {
			this.MethodName = methodName;
		}


		#region Register MethodTarget
		public Type MethodTarget {
			get => (Type)GetValue( MethodTargetProperty );
			set => SetValue( MethodTargetProperty, value );
		}

		public static readonly DependencyProperty MethodTargetProperty =
			DependencyProperty.Register( nameof( MethodTarget ), typeof( Type ), typeof( CallActionMessage ), new PropertyMetadata( default( Type ) ) );
		#endregion

		#region Register MethodName
		public string MethodName {
			get => (string)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallActionMessage ), new PropertyMetadata( default( string ) ) );
		#endregion



		protected override Freezable CreateInstanceCore() {
			return new CallActionMessage();
		}
	}

	/// <summary>
	/// 引数のあるメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TParameter">引数の型</typeparam>
	public class CallActionMessage<TParameter> : CallActionMessage, ICallOneParameterMethodMessage {

		public CallActionMessage() { }

		public CallActionMessage( Type methodTarget, string methodName, TParameter methodParameter ) : base( methodTarget, methodName ) {
			this.MethodParameter = methodParameter;
		}

		public CallActionMessage( string methodName, TParameter methodParameter ) : base( methodName ) {
			this.MethodParameter = methodParameter;
		}

		#region Register MethodParameter
		public TParameter MethodParameter {
			get => (TParameter)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( object ), typeof( CallActionMessage<TParameter> ), new PropertyMetadata( default( object ) ) );
		#endregion

		object ICallOneParameterMethodMessage.MethodParameter => MethodParameter;


		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new CallActionMessage<TParameter>();
		}
	}


	/// <summary>
	/// 引数がなく、返り値のあるメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TResult">返り値の型</typeparam>
	public class CallFuncMessage<TResult> : Message, ICallFuncMessage, IResponsiveMessage {
		public CallFuncMessage() { }

		public CallFuncMessage( string methodName ) {
			this.MethodName = methodName;
		}

		public CallFuncMessage( Type methodTarget, string methodName ) {
			this.MethodTarget = methodTarget;
			this.MethodName = methodName;
		}
		
		#region Register MethodTarget
		public Type MethodTarget {
			get => (Type)GetValue( MethodTargetProperty );
			set => SetValue( MethodTargetProperty, value );
		}

		public static readonly DependencyProperty MethodTargetProperty =
			DependencyProperty.Register( nameof( MethodTarget ), typeof( Type ), typeof( CallFuncMessage<TResult> ), new PropertyMetadata( default( Type ) ) );
		#endregion

		#region Register MethodName
		public string MethodName {
			get => (string)GetValue( MethodNameProperty );
			set => SetValue( MethodNameProperty, value );
		}

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( nameof( MethodName ), typeof( string ), typeof( CallFuncMessage<TResult> ), new PropertyMetadata( default( string ) ) );
		#endregion


		public TResult Result { get; set; }

		object IResponsiveMessage.Response {
			get => Result;
			set => Result = (TResult)value;
		}
		object ICallFuncMessage.Result {
			get => Result;
			set => Result = (TResult)value;
		}

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new CallFuncMessage<TResult>();
		}
	}

	/// <summary>
	/// 引数と、返り値のあるメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TParameter">引数の型</typeparam>
	/// <typeparam name="TResult">返り値の方</typeparam>
	public class CallFuncMessage<TParameter, TResult> : CallFuncMessage<TResult>, ICallOneParameterMethodMessage {
		public CallFuncMessage() { }


		public CallFuncMessage( TParameter methodParameter ) : base() {
			this.MethodParameter = methodParameter;
		}

		public CallFuncMessage( string methodName, TParameter methodParameter ) : base( methodName ) {
			this.MethodParameter = methodParameter;
		}

		public CallFuncMessage( Type methodTarget, string methodName, TParameter methodParameter ) : base( methodTarget, methodName ) {
			this.MethodParameter = methodParameter;
		}

		#region Register MethodParameter
		public TParameter MethodParameter {
			get => (TParameter)GetValue( MethodParameterProperty );
			set => SetValue( MethodParameterProperty, value );
		}

		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( nameof( MethodParameter ), typeof( TParameter ), typeof( CallFuncMessage<TParameter, TResult> ), new PropertyMetadata( default( TParameter ) ) );
		#endregion

		object ICallOneParameterMethodMessage.MethodParameter => MethodParameter;

		/// <summary>
		/// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
		/// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
		/// </summary>
		/// <returns>自身の新しいインスタンス</returns>
		protected override Freezable CreateInstanceCore() {
			return new CallFuncMessage<TParameter, TResult>();
		}
	}


}