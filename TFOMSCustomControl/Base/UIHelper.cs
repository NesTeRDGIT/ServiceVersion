using System.Windows;
using System.Windows.Media;

namespace TFOMSCustomControl.Base
{
    public class UIHelper
    {
        public static T GetChild<T>(UIElement element) where T:DependencyObject
        {
            if (element == null) return default;

            T child = default;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && child ==null; i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is T)
                {
                    child = VisualTreeHelper.GetChild(element, i) as T;
                }
                else
                {
                    child = GetChild<T>(VisualTreeHelper.GetChild(element, i) as UIElement);
                }
            }
            return child;
        }
    }
}
