using System;

using System.Windows;
using System.Windows.Input;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;

namespace ClientApp.Core {

    public abstract class BaseWindow : Window {
        public BaseWindow() {
            Client.Instance.ResponseReceived += OnResponseReceived;
        }

        public void Dispose() {
            Client.Instance.ResponseReceived -= OnResponseReceived;
        }

        // Response events
        protected void OnResponseReceived(object sender, Response response) {
            App.Current.Dispatcher.Invoke(delegate {
                var dispatcher = new ResponseDispatcher(response);

                if (response.Result == Result.None) {
                    throw new NotImplementedException();
                }

                ResponseReceived(dispatcher);
            });
        }

        protected virtual void ResponseReceived(ResponseDispatcher dispatcher) { 
        
        }

        // Window events
        protected virtual void ResizeWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        protected virtual void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        protected virtual void MinimizeWindowButton_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        protected virtual void MaximizeWindowButton_Click(object sender, RoutedEventArgs e) {
            if (WindowState != WindowState.Maximized) {
                ResizeMode = ResizeMode.NoResize;
                WindowState = WindowState.Maximized;
            }
            else {
                ResizeMode = ResizeMode.CanResizeWithGrip;
                WindowState = WindowState.Normal;
            }
        }
    }

}