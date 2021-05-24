using Microsoft.Xaml.Behaviors;
using System.Windows;
using LivetEx.Messaging;
using System.Linq;
using System;

namespace LivetEx.Triggers {

	/// <summary>
	/// 引数を一つだけ持つメソッドに対応したCallMethodActionです。
	/// </summary>
	public class CallMethodAction : TriggerAction<DependencyObject> {
		private static readonly MethodBinder _method = new MethodBinder();
		private static readonly MethodBinderWithArgument _argumentMethod = new MethodBinderWithArgument();

		#region Register MethodTarget
		/// <summary>
		/// メソッドを呼び出すオブジェクトを指定、または取得します。
		/// </summary>
		public object MethodTarget {
			get { return GetValue( MethodTargetProperty ); }
			set { SetValue( MethodTargetProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodTarget.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodTargetProperty =
			DependencyProperty.Register( "MethodTarget", typeof( object ), typeof( CallMethodAction ), new PropertyMetadata( null ) );
		#endregion

		#region Register MethodName
		/// <summary>
		/// 呼び出すメソッドの名前を指定、または取得します。
		/// </summary>
		public string MethodName {
			get { return (string)GetValue( MethodNameProperty ); }
			set { SetValue( MethodNameProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( "MethodName", typeof( string ), typeof( CallMethodAction ), new PropertyMetadata( null ) );
		#endregion


		#region Register MethodParameterType
		public Type MethodParameterType {
			get => (Type)GetValue( MethodParameterTypeProperty );
			set => SetValue( MethodParameterTypeProperty, value );
		}

		public static readonly DependencyProperty MethodParameterTypeProperty =
			DependencyProperty.Register( nameof( MethodParameterType ), typeof( Type ), typeof( CallMethodAction ), new PropertyMetadata( default( Type ) ) );
		#endregion

		#region Register MethodParameter
		/// <summary>
		/// 呼び出すメソッドに渡す引数を指定、または取得します。
		/// </summary>
		public object MethodParameter {
			get { return GetValue( MethodParameterProperty ); }
			set { SetValue( MethodParameterProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodParameter.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( "MethodParameter", typeof( object ), typeof( CallMethodAction ), new PropertyMetadata( null ) );
		#endregion



		protected override void Invoke( object parameter ) {
			Invoke( MethodTarget ?? this.AssociatedObject, MethodName, MethodParameterType, MethodParameter, parameter );
		}

		static void Invoke( object methodTarget, string methodName, Type methodParameterType, object methodParameter, object parameter ) {
			if( parameter is ICallMethodMessage callMethodMessage ) {
				if( callMethodMessage.MethodTarget != null && callMethodMessage.MethodTarget != methodTarget.GetType() ) {
					System.Diagnostics.Debug.WriteLine( $"CallMethodAction({methodTarget.GetType().FullName}): {callMethodMessage.MethodTarget.FullName}" );
					return;
				}

				methodName = callMethodMessage.MethodName ?? methodName;
			}

			if( parameter is ICallOneParameterMethodMessage callMethodMessageOneParameter ) {
				methodParameter = callMethodMessageOneParameter.MethodParameter ?? methodParameter;
			}

			var t = parameter.GetType();
			if( t.IsGenericType ) {
				var type = t.GetGenericTypeDefinition();
				if( type == typeof( CallActionMessage<> ) || type == typeof( CallFuncMessage<,> ) ) {
					methodParameterType = t.GenericTypeArguments.First();
				}
			}

			if( methodParameterType == null && methodParameter == null ) {
				if( parameter is IResponsiveMessage responsiveMessage ) {
					var r = _method.Invoke( methodTarget, methodName );
					responsiveMessage.Response = r;
				} else {
					_method.Invoke( methodTarget, methodName );
				}

			} else {
				if( parameter is IResponsiveMessage responsiveMessage ) {
					var r = _argumentMethod.Invoke( methodTarget, methodName, methodParameter?.GetType() ?? methodParameterType, methodParameter );
					responsiveMessage.Response = r;
				} else {
					_argumentMethod.Invoke( methodTarget, methodName, methodParameter?.GetType() ?? methodParameterType, methodParameter );
				}
			}

			if( parameter is Message message ) {
				message.IsHandled = true;
			}
		}


		public static void Action( object methodTarget, ICallMethodMessage message ) {
			Invoke( methodTarget, null, null, null, message );
		}
	}
}
