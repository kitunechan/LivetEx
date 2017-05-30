using System.Windows.Interactivity;
using System.Windows;
using LivetEx.Messaging;
using System.Linq;

namespace LivetEx.Behaviors {
	/// <summary>
	/// 引数を一つだけ持つメソッドに対応したCallMethodActionです。
	/// </summary>
	public class LivetCallMethodAction: TriggerAction<DependencyObject> {
		private MethodBinder _method = new MethodBinder();
		private MethodBinderWithArgument _callbackMethod = new MethodBinderWithArgument();

		private bool _parameterSet;

		/// <summary>
		/// メソッドを呼び出すオブジェクトを指定、または取得します。
		/// </summary>
		public object MethodTarget {
			get { return GetValue( MethodTargetProperty ); }
			set { SetValue( MethodTargetProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodTarget.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodTargetProperty =
			DependencyProperty.Register( "MethodTarget", typeof( object ), typeof( LivetCallMethodAction ), new PropertyMetadata( null ) );

		/// <summary>
		/// 呼び出すメソッドの名前を指定、または取得します。
		/// </summary>
		public string MethodName {
			get { return (string)GetValue( MethodNameProperty ); }
			set { SetValue( MethodNameProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( "MethodName", typeof( string ), typeof( LivetCallMethodAction ), new PropertyMetadata( null ) );


		/// <summary>
		/// 呼び出すメソッドに渡す引数を指定、または取得します。
		/// </summary>
		public object MethodParameter {
			get { return GetValue( MethodParameterProperty ); }
			set { SetValue( MethodParameterProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodParameter.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( "MethodParameter", typeof( object ), typeof( LivetCallMethodAction ), new PropertyMetadata( null, OnMethodParameterChanged ) );

		private static void OnMethodParameterChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e ) {
			var thisReference = (LivetCallMethodAction)sender;

			thisReference._parameterSet = true;
		}

		/// <summary>
		/// MethodParameterをメッセージ(GenericInteractionMessage)から与えるかどうかを取得または設定します。
		/// </summary>
		[System.ComponentModel.Description( "MethodParameterをメッセージ(GenericInteractionMessage)から与えるかどうかを取得または設定します。" )]
		#region Register IsParameterToGenericInteractionMessage
		public bool IsParameterToGenericInteractionMessage {
			get { return (bool)GetValue( IsParameterToGenericInteractionMessageProperty ); }
			set { SetValue( IsParameterToGenericInteractionMessageProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for IsParameterToGenericInteractionMessage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsParameterToGenericInteractionMessageProperty =
			DependencyProperty.Register( "IsParameterToGenericInteractionMessage", typeof( bool ), typeof( LivetCallMethodAction ), new PropertyMetadata( false ) );
		#endregion

		
		protected override void Invoke( object parameter ) {
			if( MethodTarget == null ) return;
			if( MethodName == null ) return;

			if( IsParameterToGenericInteractionMessage ) {
				_callbackMethod.Invoke( MethodTarget, MethodName, ((GenericInteractionMessage)parameter).Value );
			}else if( parameter is ResponsiveInteractionMessage ){
				var responsive = (ResponsiveInteractionMessage)parameter;
				responsive.Response = _method.Invoke( MethodTarget, MethodName, responsive.GetType().GenericTypeArguments.FirstOrDefault() ?? typeof( object ) );
			} else if( !_parameterSet ) {
				_method.Invoke( MethodTarget, MethodName );
			} else {
				_callbackMethod.Invoke( MethodTarget, MethodName, MethodParameter );
			}
		}
	}
}
