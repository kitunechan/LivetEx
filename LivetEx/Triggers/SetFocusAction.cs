using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace LivetEx.Triggers {
    /// <summary>
    /// アタッチしているコントロールにフォーカスを試みます。
    /// </summary>
    public class SetFocusAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            this.AssociatedObject.Focus();
        }
    }
}
