using System;
using System.Windows;
using System.ComponentModel;

using System.Windows.Controls;
using System.Windows.Input;

using Network.Client;
using Network.Client.DataProcessing;

using ClientApp.Resources;
using Network.Shared.DataTransfer.Base;

namespace ClientApp.Core {

    public abstract class BaseWindow : Window {
        public void EnableResponseListener() {
            Client.Instance.ResponseReceived += OnResponseReceivedEvent;
        }

        public void DisableResponseListener() {
            Client.Instance.ResponseReceived -= OnResponseReceivedEvent;
        }

        protected virtual void ShowErrorMessage(TextBlock textBlock, string content) {
            textBlock.Text = ResourceManager.GetValue(content);
            textBlock.Visibility = Visibility.Visible;
        }

        protected virtual void ShowErrorMessage(TextBlock textBlock, string content, params object[] values) {
            textBlock.Text = ResourceManager.GetValue(content, values);
            textBlock.Visibility = Visibility.Visible;
        }

        // Window events
        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            DisableResponseListener();
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

        protected virtual void ResizeWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        // Response events
        protected void OnResponseReceivedEvent(object sender, Response response) {
            App.Current.Dispatcher.Invoke(delegate {
                var dispatcher = new ResponseDispatcher(response);

                if (response.Result == ResponseResult.None) {
                    throw new NotImplementedException();
                }

                OnResponseReceived(dispatcher);
            });
        }

        // Template method pattern
        protected virtual void OnResponseReceived(ResponseDispatcher dispatcher) { }
    }

}