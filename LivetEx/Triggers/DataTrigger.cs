using System.Windows;

namespace LivetEx.Triggers {
	/// <summary>
	/// 初期値に対応したDataTriggerです。
	/// </summary>
	public class DataTrigger : Microsoft.Expression.Interactivity.Core.DataTrigger {
		protected override void OnAttached() {
			base.OnAttached();

			base.EvaluateBindingChange( new DependencyPropertyChangedEventArgs( ValueProperty, null, Value ) );
		}
	}
}
