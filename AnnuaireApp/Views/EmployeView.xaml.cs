using AnnuaireApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnnuaireApp.Views
{
    /// <summary>
    /// Logique d'interaction pour EmployeView.xaml
    /// </summary>
    public partial class EmployeView : Page
    {
        public EmployeView()
        {
            InitializeComponent();
            DataContext = new EmployeViewModel();
            SearchBox.TextChanged += SearchBox_TextChanged;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Récupère le TextBlock frère dans le Grid
                if (textBox.Parent is Grid grid && grid.Children[1] is TextBlock placeholder)
                {
                    placeholder.Visibility = string.IsNullOrWhiteSpace(textBox.Text) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
    }
}
