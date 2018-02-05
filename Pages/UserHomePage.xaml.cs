﻿using I2P_Project.Classes.Data_Managers;
using I2P_Project.Classes.UserSystem;
using I2P_Project.DataBases;
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
    /// Interaction logic for UserHomePage.xaml
    /// </summary>
    public partial class UserHomePage : Window
    {
        public UserHomePage()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void UpdateUI()
        {
            while (DocList.Items.Count > 0) DocList.Items.RemoveAt(0);
            WelcomeText.Content = "Welcome, " + SystemDataManager.CurrentUser.Name + "!";
            foreach (documents doc in DataBaseManager.GetAllDocs())
            {
                string availibility = doc.Count == 0 ? "Not availible" : "Availible: " + doc.Count;
                string line = doc.Id + "| " + availibility +  " | " + doc.Title;
                DocList.Items.Add(line);
            }
        }

        private void OnCheckOut(object sender, RoutedEventArgs e)
        {
            if (DocList.SelectedItem == null) InfoText.Content = "Select a document you would like to check out";
            else
            {
                Patron currentPatron = (Patron)SystemDataManager.CurrentUser;
                string s, item = (string)DocList.SelectedItem;
                s = item.Substring(0, item.IndexOf('|'));
                int docID = Convert.ToInt32(s);
                InfoText.Content = currentPatron.CheckOut(docID);
                UpdateUI();
            }
        }

        private void OnMyDocs(object sender, RoutedEventArgs e)
        {
            MyBooks MyDocs = new MyBooks();
            MyDocs.Show();
            Close();
        }
    }
}
