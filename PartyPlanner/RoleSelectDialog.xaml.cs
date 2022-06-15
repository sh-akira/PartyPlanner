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
    /// RoleSelectDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class RoleSelectDialog : Window
    {
        public RoleSelectDialog()
        {
            InitializeComponent();
            RoleListBox.ItemsSource = Settings.Instance.Roles;
        }

        public Role SelectedRole => RoleListBox.SelectedItem as Role;

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
