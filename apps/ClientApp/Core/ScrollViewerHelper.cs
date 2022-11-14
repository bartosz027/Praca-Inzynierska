using Hangfire.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ClientApp.Core
{
    public static class ScrollViewerHelper
    {
        public static readonly DependencyProperty ScrollsHorizontallyProperty
            = DependencyProperty.RegisterAttached("WheelScrollsHorizontally",
                typeof(bool),
                typeof(ScrollViewerHelper),
                new PropertyMetadata(false, UseHorizontalScrollingChangedCallback));

        private static void UseHorizontalScrollingChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as UIElement;

            if (element == null)
                throw new Exception("Attached property must be used with UIElement.");

            if ((bool)e.NewValue)
                element.PreviewMouseWheel += OnPreviewMouseWheel;
            else
                element.PreviewMouseWheel -= OnPreviewMouseWheel;
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            var scrollViewer = ((UIElement)sender).FindDescendant<ScrollViewer>();

            if (scrollViewer == null)
                return;

            if (args.Delta < 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    scrollViewer.LineRight();
                }
               
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    scrollViewer.LineLeft();
                }
            }

            args.Handled = true;
        }

        public static void SetWheelScrollsHorizontally(UIElement element, bool value) => element.SetValue(ScrollsHorizontallyProperty, value);
        public static bool GetWheelScrollsHorizontally(UIElement element) => (bool)element.GetValue(ScrollsHorizontallyProperty);

        [CanBeNull]
        private static T FindDescendant<T>([CanBeNull] this DependencyObject d) where T : DependencyObject
        {
            if (d == null)
                return null;

            var childCount = VisualTreeHelper.GetChildrenCount(d);

            for (var i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);

                var result = child as T ?? FindDescendant<T>(child);

                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
