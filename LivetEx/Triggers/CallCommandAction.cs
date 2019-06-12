﻿using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Input;

namespace LivetEx.Triggers {
	public class CallCommandAction : TriggerAction<DependencyObject> {
		protected override void Invoke( object parameter ) {
			if( Command.CanExecute( parameter ) ) {
				Command.Execute( parameter );
			}
		}

		#region Register Command
		public ICommand Command {
			get => (ICommand)GetValue( CommandProperty );
			set => SetValue( CommandProperty, value );
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register( nameof( Command ), typeof( ICommand ), typeof( CallCommandAction ), new PropertyMetadata( default( ICommand ) ) );
		#endregion

		#region Register CommandParameter
		public object CommandParameter {
			get => (object)GetValue( CommandParameterProperty );
			set => SetValue( CommandParameterProperty, value );
		}

		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register( nameof( CommandParameter ), typeof( object ), typeof( CallCommandAction ), new PropertyMetadata( default( object ) ) );
		#endregion

	}
}
