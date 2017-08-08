using System;
using System.Windows.Interactivity;
using System.Windows;

namespace LivetEx.Triggers {
	/// <summary>
	/// アタッチしたオブジェクトのDataContextがIDisposableである場合、Disposeします。
	/// </summary>
	public class DataContextDisposeAction: TriggerAction<FrameworkElement> {
		protected override void Invoke( object parameter ) {
			if( AssociatedObject.DataContext is IDisposable disposable ) {
				disposable.Dispose();
			}
		}
	}
}
