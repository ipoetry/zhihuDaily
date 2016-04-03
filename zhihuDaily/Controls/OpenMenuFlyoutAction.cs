using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace zhihuDaily
{
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);

            flyoutBase.ShowAt(senderElement);

            return null;
            //Windows.UI.Xaml.Controls.MenuFlyout mf = (Windows.UI.Xaml.Controls.MenuFlyout)FlyoutBase.GetAttachedFlyout(senderElement);
            ////FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            //var e = (Windows.UI.Xaml.Input.TappedRoutedEventArgs)parameter;
            //mf.ShowAt(senderElement, e.GetPosition(null));
            //return null;
        }
    }
}
