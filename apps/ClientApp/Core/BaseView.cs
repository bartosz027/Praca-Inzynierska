using System;

using System.Text;
using System.Text.RegularExpressions;

using System.Windows.Controls;
using System.Windows.Input;

namespace ClientApp.Core {

    public abstract class BaseView : UserControl {
        protected void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }

}