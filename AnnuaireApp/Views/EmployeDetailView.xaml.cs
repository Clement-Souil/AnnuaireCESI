﻿using System;
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
using System.Windows.Shapes;

namespace AnnuaireApp.Views
{
    /// <summary>
    /// Logique d'interaction pour EmployeDetailView.xaml
    /// </summary>
    public partial class EmployeDetailView : Window
    {
        public EmployeDetailView()
        {
            InitializeComponent();
        }
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
