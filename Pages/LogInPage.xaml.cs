﻿using I2P_Project.Classes.Data_Managers;
using I2P_Project.Classes.UserSystem;
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
using System.Windows.Shapes;

namespace I2P_Project.Pages
{
    /// <summary>
    /// Логика взаимодействия для LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Window
    {
        public LogInPage()
        {
            DataBaseManager.Initialize();
            InitializeComponent();
        }

        private void LogInClick(object sender, RoutedEventArgs e)
        {
            if (DataBaseManager.CheckEmail(EMailTB.Text))
            {
                if (DataBaseManager.CheckPassword(EMailTB.Text, PasswordTB.Password))
                    LogIn();
                else
                    WrongLabel.Content = "Wrong password"; // TODO String constants
            }
            else
                WrongLabel.Content = "User not found"; // TODO String constants
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            RegisterPage Register = new RegisterPage();
            Register.Show();
            Close();
        }

        private void LogIn()
        {
            SetCurrentUser();
            UserHomePage HomePage = new UserHomePage();
            HomePage.Show();
            Close();
        }

        private void SetCurrentUser()
        {
            int userType = DataBaseManager.GetUserType(EMailTB.Text);
            switch (userType)
            {
                case 0:
                    SystemDataManager.CurrentUser = new Student(EMailTB.Text);
                    break;
                case 1:
                    SystemDataManager.CurrentUser = new Faculty(EMailTB.Text);
                    break;
                case 2:
                    SystemDataManager.CurrentUser = new Librarian(EMailTB.Text);
                    break;
                default:
                    throw new Exception("Unhandled user type!");
            }
        }

    }
}
