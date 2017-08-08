using System.Windows.Interactivity;
using System.Windows;
using LivetEx.Messaging;
using System.Linq;

namespace LivetEx.Triggers {
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
		/// MethodParameterをメッセージから与えるかどうかを取得または設定します。
		/// </summary>
		[System.ComponentModel.Description( "MethodParameterをメッセージから与えるかどうかを取得または設定します。" )]
		#region Register IsParameterToGenericInteractionMessage
		public bool IsParameterFromMessage {
			get { return (bool)GetValue( IsParameterFromMessageProperty ); }
			set { SetValue( IsParameterFromMessageProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for IsParameterFromMessage.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsParameterFromMessageProperty =
			DependencyProperty.Register( "IsParameterFromMessage", typeof( bool ), typeof( LivetCallMethodAction ), new PropertyMetadata( false ) );
		#endregion

		
		protected override void Invoke( object parameter ) {
			if( MethodTarget == null ) return;
			if( MethodName == null ) return;

			if( IsParameterFromMessage ) {
				if( parameter is InteractionMessageOneParameter interaction ) {
					_callbackMethod.Invoke( MethodTarget, MethodName, interaction.Value );
				}else if( parameter is ResponsiveInteractionMessageOneParameter responsive ) {
					responsive.Response = _callbackMethod.Invoke( MethodTarget, MethodName, responsive.Value, responsive.GetType().GenericTypeArguments.FirstOrDefault() ?? typeof( object ) );
				}
			}else if( parameter is ResponsiveInteractionMessage responsive ) {
				responsive.Response = _method.Invoke( MethodTarget, MethodName, responsive.GetType().GenericTypeArguments.FirstOrDefault() ?? typeof( object ) );

			} else if( !_parameterSet ) {
				_method.Invoke( MethodTarget, MethodName );
			} else {
				_callbackMethod.Invoke( MethodTarget, MethodName, MethodParameter );
			}
		}
	}
}
