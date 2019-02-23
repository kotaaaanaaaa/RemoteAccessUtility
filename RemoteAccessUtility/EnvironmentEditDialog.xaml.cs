using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RemoteAccessUtility
{
    /// <summary>
    /// EnvironmentEditDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EnvironmentEditDialog : UserControl
    {
        private IList<Environment> Environments;

        public EnvironmentEditDialog(IList<Environment> environments)
        {
            Environments = environments;

            InitializeComponent();
            EnvironmentsList.ItemsSource = Environments;
            EnvironmentsList.SelectedIndex = 0;
        }

        private async void EnvironmentsList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedItem = (Environment)EnvironmentsList.SelectedItem;
            if (selectedItem == null) return;
            this.DataContext = selectedItem;
        }

        private async void AddEnvironment_Click(object sender, RoutedEventArgs e)
        {
            var newEnvironment = new Environment();
            Environments.Add(newEnvironment);
            EnvironmentsList.SelectedItem = newEnvironment;
        }

        private async void RemoveEnvironment_Click(object sender, RoutedEventArgs e)
        {
            if (Environments.Count <= 1) return;
            var selectedItem = (Environment)EnvironmentsList.SelectedItem;
            if (selectedItem == null) return;
            Environments.Remove(selectedItem);
            EnvironmentsList.SelectedIndex = 0;
        }
    }
}
