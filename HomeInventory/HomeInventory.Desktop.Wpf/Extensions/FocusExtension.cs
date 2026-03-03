
using System.Windows;
using System.Windows.Controls;

namespace HomeInventory.Desktop.Wpf.Extensions
{
    public static class FocusExtensions
    {
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused",
                typeof(bool),
                typeof(FocusExtensions),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsFocusedChanged));

        public static bool GetIsFocused(DependencyObject obj) => (bool)obj.GetValue(IsFocusedProperty);
        public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement fe) return;
            if (e.NewValue is not bool isFocused || !isFocused) return;

            fe.Dispatcher.BeginInvoke(() =>
            {
                fe.Focus();
                if (fe is TextBox tb) tb.SelectAll();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
