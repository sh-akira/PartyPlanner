using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// MemberEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MemberEditDialog : Window
    {
        public MemberEditDialog()
        {
            InitializeComponent();
        }

        public Member CurrentMember;
        public ObservableCollection<Role> NewRoles;
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text) || NewRoles.Any() == false)
            {
                MessageBox.Show("名前か役職が空です", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentMember.Name = NameTextBox.Text;
            CurrentMember.Roles = new List<Role>(NewRoles);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentMember == null)
            {
                CurrentMember = new Member();
                CurrentMember.IsChecked = true;
                NewRoles = new ObservableCollection<Role>();
            }
            else
            {
                NameTextBox.Text = CurrentMember.Name;
                NewRoles = new ObservableCollection<Role>(CurrentMember.Roles);
            }
            RoleListBox.ItemsSource = NewRoles;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RoleSelectDialog();
            if (dialog.ShowDialog() == true)
            {
                if (NewRoles.Contains(dialog.SelectedRole) == false)
                {
                    NewRoles.Add(dialog.SelectedRole);
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoleListBox.SelectedItem == null) return;
            NewRoles.Remove(RoleListBox.SelectedItem as Role);
        }
    }
}
