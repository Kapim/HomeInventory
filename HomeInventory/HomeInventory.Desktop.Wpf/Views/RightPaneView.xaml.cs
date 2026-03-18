using HomeInventory.Desktop.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace HomeInventory.Desktop.Wpf.Views
{
    /// <summary>
    /// Interaction logic for RightPane.xaml
    /// </summary>
    public partial class RightPaneView : UserControl
    {
        public RightPaneView()
        {
            InitializeComponent();
        }
    

        private static DataGridCell? GetCell(DataGrid grid, DataGridRow row, int columnIndex)
        {
            if (row == null) return null;

            var presenter = FindVisualChild<DataGridCellsPresenter>(row);
            if (presenter == null) return null;

            var cell = (DataGridCell)presenter.ItemContainerGenerator
                .ContainerFromIndex(columnIndex);

            return cell;
        }

        private static T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                if (child is T t)
                    return t;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs r)
        {
            if (sender is DataGrid grid)
                grid.SelectionChanged += Grid_SelectionChanged;
        }

        private void Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not DataGrid grid) return;
            if (grid.SelectedItem is null) return;

            if (grid.DataContext is RightPaneViewModel vm)
                // only begin edit of new item
                if (vm.SelectedItem == null || !vm.SelectedItem.IsNew)
                    return;
            

            grid.Dispatcher.BeginInvoke(() =>
            {
                grid.UpdateLayout();
                grid.ScrollIntoView(grid.SelectedItem);

                var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);

                if (row is null) return;

                var cell = GetCell(grid, row, 1);
                if (cell is null) return;

                cell.Focus();
                grid.BeginEdit();
            });
        }
    }
}