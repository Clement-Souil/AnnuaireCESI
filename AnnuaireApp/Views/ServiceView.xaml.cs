﻿using AnnuaireApp.ViewModels;
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
    /// Logique d'interaction pour ServiceView.xaml
    /// </summary>
    public partial class ServiceView : Page
    {
        public ServiceView()
        {
            InitializeComponent();
            DataContext = new ServiceViewModel();
            UpdateAdminUI(); // Met à jour l'interface après connexion
        }

        public void UpdateAdminUI()
        {
            AdminButtonsPanel.Visibility = AdminManager.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
