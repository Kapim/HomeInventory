using HomeInventory.Desktop.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace HomeInventory.Desktop.Wpf.Views
{
    /// <summary>
    /// Interaction logic for LocationTreeView.xaml
    /// </summary>
    public partial class LocationTreeView : UserControl
    {
        public LocationTreeView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is not LocationTreeViewModel vm) return;

            if (e.NewValue is LocationNodeViewModel location)
                vm.SelectedLocation = location;
            else
                return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!location.IsEditing) return;

                var tb = FindVisualChild<TextBox>((DependencyObject)sender);
                tb?.Focus();
                tb?.SelectAll();

            }), System.Windows.Threading.DispatcherPriority.Background);

        }


        private static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t) return t;

                var nested = FindVisualChild<T>(child);
                if (nested != null) return nested;
            }
            return null;
        }
    }
}
