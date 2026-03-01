using HomeInventory.Desktop.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }
    }
}
