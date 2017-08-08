using System;
using System.Linq;
using System.Windows;
using LivetEx.Messaging;

namespace LivetEx.Messaging {
	/// <summary>
	/// 画面遷移(Window)を行うアクションです。<see cref="WindowTransitionMessage"/>に対応します。
	/// </summary>
	public class WindowTransitionInteractionMessageAction : InteractionMessageAction<FrameworkElement> {
		/// <summary>
		/// 遷移するウインドウの型を指定、または取得します。
		/// </summary>
		public Type WindowType {
			get { return (Type)GetValue( WindowTypeProperty ); }
			set { SetValue( WindowTypeProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for WindowType.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WindowTypeProperty =
			DependencyProperty.Register( "WindowType", typeof( Type ), typeof( WindowTransitionInteractionMessageAction ), new PropertyMetadata() );

		private static bool IsValidWindowType( Type value ) {
			if( value != null ) {
				if( value.IsSubclassOf( typeof( Window ) ) ) {
					return value.GetConstructor( Type.EmptyTypes ) != null;
				}
			}

			return false;
		}

		/// <summary>
		/// 画面遷移の種類を指定するTransitionMode列挙体を指定、または取得します。<br/>
		/// TransitionMessageでModeがUnKnown以外に指定されていた場合、そちらが優先されます。
		/// </summary>
		public WindowTransitionMode Mode {
			get { return (WindowTransitionMode)GetValue( ModeProperty ); }
			set { SetValue( ModeProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register( "Mode", typeof( WindowTransitionMode ), typeof( WindowTransitionInteractionMessageAction ), new PropertyMetadata( WindowTransitionMode.UnKnown ) );

		/// <summary>
		/// 遷移先ウィンドウがこのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool IsOwned {
			get { return (bool)GetValue( OwnedFromThisProperty ); }
			set { SetValue( OwnedFromThisProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for OwnedFromThis.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OwnedFromThisProperty =
			DependencyProperty.Register( "IsOwned", typeof( bool ), typeof( WindowTransitionInteractionMessageAction ), new PropertyMetadata( true ) );


		protected override void InvokeAction( InteractionMessage message ) {
			if( message is WindowTransitionMessage transitionMessage ) {
				var clone = (WindowTransitionMessage)transitionMessage.Clone();
				{
					clone.WindowType = transitionMessage.WindowType ?? WindowType;
					clone.Mode = ( transitionMessage.Mode != WindowTransitionMode.UnKnown ) ? transitionMessage.Mode : Mode;
					clone.IsOwned = transitionMessage.IsOwned ?? IsOwned;

					clone.Freeze();
				}
				Action( this.AssociatedObject, clone );

				transitionMessage.Response = clone.Response;
			}
		}


		public static void Action( FrameworkElement element, WindowTransitionMessage message ) {
			var targetType = message.WindowType;

			if( !IsValidWindowType( targetType ) ) {
				return;
			}

			var defaultConstructor = targetType.GetConstructor( Type.EmptyTypes );



			var mode = message.Mode;
			if( mode == WindowTransitionMode.UnKnown ) {
				mode = WindowTransitionMode.Modal;
			}

			switch( mode ) {
				case WindowTransitionMode.Modeless:
				case WindowTransitionMode.Modal: {
					var targetWindow = (Window)defaultConstructor.Invoke( null );
					if( message.ViewModel != null ) {
						targetWindow.DataContext = message.ViewModel;
					}

					if( message.IsOwned == true ) {
						targetWindow.Owner = Window.GetWindow( element );
					}

					if( message.WindowStartupLocation.HasValue ) {
						targetWindow.WindowStartupLocation = message.WindowStartupLocation.Value;
					}

					if( mode == WindowTransitionMode.Modeless ) {
						targetWindow.Show();
						message.Response = null;
					} else {
						message.Response = targetWindow.ShowDialog();
					}

					break;
				}

				case WindowTransitionMode.NewOrActive: {
					var window = Application.Current.Windows
						.OfType<Window>()
						.FirstOrDefault( w => w.GetType() == targetType );

					if( window == null ) {
						window = (Window)defaultConstructor.Invoke( null );

						if( message.ViewModel != null ) {
							window.DataContext = message.ViewModel;
						}
						if( message.WindowStartupLocation.HasValue ) {
							window.WindowStartupLocation = message.WindowStartupLocation.Value;
						}
						if( message.IsOwned == true ) {
							window.Owner = Window.GetWindow( element );
						}
						window.Show();
						message.Response = null;
					} else {
						if( message.ViewModel != null ) {
							window.DataContext = message.ViewModel;
						}
						if( message.WindowStartupLocation.HasValue ) {
							window.WindowStartupLocation = message.WindowStartupLocation.Value;
						}
						if( message.IsOwned == true ) {
							window.Owner = Window.GetWindow( element );
						}
						window.Activate();
						// 最小化中なら戻す
						if( window.WindowState == WindowState.Minimized ) {
							window.WindowState = WindowState.Normal;
						}
						message.Response = null;
					}

					break;
				}
			}

		}

	}
}
