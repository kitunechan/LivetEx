using System.Windows.Interactivity;
using System.Windows;

namespace LivetEx.Behaviors
{
    /// <summary>
    /// アタッチしているコントロールにフォーカスを試みます。
    /// </summary>
    public class SetFocusAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            AssociatedObject.Focus();
        }
    }
}
