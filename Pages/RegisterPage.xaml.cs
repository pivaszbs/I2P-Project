﻿using I2P_Project.Classes.Data_Managers;
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

namespace I2P_Project.Pages
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Window
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void OnRegisterClick(object sender, RoutedEventArgs e)
        {
            // TODO Implement some serial numbers to check librarians
            bool isLibrarian = false;
            DataBaseManager.RegisterUser
                (
                    EMailTB.Text,
                    PasswordTB.Text,
                    NameTB.Text,
                    AdressTB.Text,
                    PhoneNumberTB.Text,
                    isLibrarian
                );
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            LogInPage LogIn = new LogInPage();
            LogIn.Show();
            Close();
        }

    }
}