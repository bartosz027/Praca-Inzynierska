﻿using System.Windows;
using System.Windows.Input;
using ClientApp.MVVM.View.PIWindowView;
using ClientApp.MVVM.ViewModel.PIWindowViewModel;
using Network.Client;
using Network.Shared.DataTransfer.Model.Account.Login;
using Network.Shared.DataTransfer.Base;
using Network.Client.DataProcessing;
using System;

namespace ClientApp.MVVM.View.LoginWindowView
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Client.Instance.ResponseReceived += OnResponseReceived;
        }

        private void ResizeWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Click
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow RegisterWindow = new RegisterWindow();
            RegisterWindow.Show();

            // Close current window
            Client.Instance.ResponseReceived -= OnResponseReceived;
            Application.Current.Windows[0].Close();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordWindow ForgotPasswordWindow = new ForgotPasswordWindow();
            ForgotPasswordWindow.Show();

            // Close current window
            Client.Instance.ResponseReceived -= OnResponseReceived;
            Application.Current.Windows[0].Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Client.Instance.SendRequest(new LoginRequest() { 
                Email = EmailBox.Text, 
                Password = PasswordBox.Password 
            });
        }

        // Response Event Handling
        private void OnResponseReceived(object sender, Response response)
        {
            var dispatcher = new ResponseDispatcher(response);
            dispatcher.Dispatch<LoginResponse>(OnLoginResponse);
        }

        private void OnLoginResponse(LoginResponse response)
        {
            switch (response.Status)
            {
                case STATUS.SUCCESS:
                {
                    App.Current.Dispatcher.Invoke(delegate {
                        PIWindow PIWindow = new PIWindow();
                        PIWindowViewModel PIWindowViewModel = new PIWindowViewModel();

                        PIWindow.DataContext = PIWindowViewModel;
                        PIWindow.Show();

                        Client.Instance.ResponseReceived -= OnResponseReceived;
                        Application.Current.Windows[0].Close();
                    });
                    break;
                }
                case STATUS.FAILURE: 
                {
                    MessageBox.Show("TODO: LUX powiadomienie");
                    break;
                }
            }
        }
    }
}