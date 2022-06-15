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

namespace PartyPlanner
{
    /// <summary>
    /// InputBox.xaml の相互作用ロジック
    /// </summary>
    public partial class InputBox : Window
    {
        public InputBox()
        {
            InitializeComponent();
        }

        public static string Show(string Title, string Message, string DefaultText = "")
        {
            var inputBox = new InputBox();
            inputBox.Title = Title;
            inputBox.MessageTextBlock.Text = Message;
            inputBox.InputTextBox.Text = DefaultText;
            if (inputBox.ShowDialog() == true)
            {
                return inputBox.InputTextBox.Text;
            }
            return null;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
