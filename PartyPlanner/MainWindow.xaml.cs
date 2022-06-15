using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PartyPlanner
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RoleDataGrid.ItemsSource = Settings.Instance.Roles;
            MemberDataGrid.ItemsSource = Settings.Instance.Members;
            SelectedRolesDataGrid.ItemsSource = Settings.Instance.SelectedRoles;
        }

        private void RoleAddButton_Click(object sender, RoutedEventArgs e)
        {
            var role = InputBox.Show("役職追加", "役職名を入力してください");
            if (role != null)
            {
                Settings.Instance.Roles.Add(new Role { Name = role });
            }
        }

        private void RoleRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoleDataGrid.SelectedItem == null) return;
            Settings.Instance.Roles.Remove(RoleDataGrid.SelectedItem as Role);
        }

        private void MemberAddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MemberEditDialog();
            if (dialog.ShowDialog() == true)
            {
                Settings.Instance.Members.Add(dialog.CurrentMember);
            }
        }

        private void MemberRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberDataGrid.SelectedItem == null) return;
            Settings.Instance.Members.Remove(MemberDataGrid.SelectedItem as Member);
        }

        private void MemberEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemberDataGrid.SelectedItem == null) return;

            var dialog = new MemberEditDialog();
            dialog.CurrentMember = MemberDataGrid.SelectedItem as Member;
            dialog.ShowDialog();
            MemberDataGrid.Items.Refresh();
        }

        private void RoleSelectAddButton_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new RoleSelectDialog();
            if (dialog.ShowDialog() == true)
            {
                Settings.Instance.SelectedRoles.Add(dialog.SelectedRole);
            }
        }

        private void RoleSelectRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedRolesDataGrid.SelectedItem == null) return;
            Settings.Instance.SelectedRoles.RemoveAt(SelectedRolesDataGrid.SelectedIndex);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Title = "設定の読込";
                ofd.Filter = "*.json|*.json";
                if (ofd.ShowDialog() == true)
                {
                    var json = System.IO.File.ReadAllText(ofd.FileName);
                    var settings = Json.Serializer.Deserialize<Settings>(json);
                    Settings.Instance.Apply(settings);
                }
            }
            catch
            {
                MessageBox.Show("ファイルの書き込みに失敗しました", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sfd = new SaveFileDialog();
                sfd.Title = "設定の保存";
                sfd.Filter = "*.json|*.json";
                if (sfd.ShowDialog() == true)
                {
                    var json = Json.Serializer.Serialize(Settings.Instance);
                    var readable = Json.Serializer.ToReadable(json);
                    System.IO.File.WriteAllText(sfd.FileName, readable);
                }
            }
            catch
            {
                MessageBox.Show("ファイルの書き込みに失敗しました", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {

            var members = new List<Member>(Settings.Instance.Members.Where(d => d.IsChecked).OrderBy(d => Guid.NewGuid()));
            var roles = Settings.Instance.SelectedRoles;
            var partyList = new List<List<Member>>();

            while (members.Any())
            {
                var memberList = new List<Member>();
                partyList.Add(memberList);
                foreach (var role in roles)
                {
                    var member = SelectPriorityMember(members, role);
                    if (member != null) members.Remove(member);
                    memberList.Add(member);
                }
            }




            for (int pIndex = partyList.Count - 1; pIndex > 0; pIndex--)
            {
                //手前のパーティーに空席が無ければ終わり
                if (partyList.Take(pIndex).Any(d => d.Contains(null)) == false) break;

                var party = partyList[pIndex];
                //手前のパーティーに空席がある場合一人ずつ移動できる場合移動する
                for (int mIndex = 0; mIndex < party.Count; mIndex++)
                {
                    bool found = false;
                    var member = party[mIndex];
                    if (member == null) continue;
                    var memberRoles = member.Roles;
                    for (int prevEmptyPartyIndex = pIndex - 1; prevEmptyPartyIndex >= 0; prevEmptyPartyIndex--)
                    {
                        var prevEmptyParty = partyList[prevEmptyPartyIndex];
                        //手前のパーティーに空席が無い場合さらに手前にあるか見る
                        if (prevEmptyParty.Contains(null) == false) break;

                        for (int emptyIndex = 0; emptyIndex < partyList[pIndex].Count; emptyIndex++)
                        {
                            //空席を調べる
                            if (prevEmptyParty[emptyIndex] != null) continue;
                            var emptyRole = roles[emptyIndex];

                            //やりたい役職順に手前のパーティーから交代できる人を探す
                            foreach (var role in memberRoles)
                            {
                                var roleIndex = roles.IndexOf(role);
                                for (int prevPartyIndex = prevEmptyPartyIndex; prevPartyIndex >= 0; prevPartyIndex--)
                                {
                                    //手前のパーティーから、手前の空席に人を移動して、空いたところに移動する
                                    var prevParty = partyList[prevPartyIndex];
                                    if (prevParty[roleIndex].Roles.Contains(emptyRole))
                                    {
                                        prevEmptyParty[emptyIndex] = prevParty[roleIndex];
                                        prevParty[roleIndex] = member;
                                        party[mIndex] = null;
                                        found = true;
                                        break;
                                    }
                                }
                                if (found) break;
                            }
                            if (found) break;
                        }
                        if (found) break;
                    }
                }
            }

            for(int pIndex = partyList.Count-1;pIndex >= 0; pIndex--)
            {
                if (partyList[pIndex].Any(d=>d != null) == false)
                {
                    partyList.RemoveAt(pIndex);
                }
            }

            //各パーティーの同じ役職の人をランダムに入れ替える
            for (int rIndex = 0; rIndex < roles.Count; rIndex++)
            {
                var randomMembers = partyList.Select(d => d[rIndex]).OrderBy(d => Guid.NewGuid()).OrderBy(d => d == null).ToArray();
                for (int pIndex = 0; pIndex < partyList.Count; pIndex++)
                {
                    partyList[pIndex][rIndex] = randomMembers[pIndex];
                }
            }

            //出力
            var sb = new StringBuilder();

            foreach (var party in partyList)
            {
                sb.AppendLine("-----------------------");
                for (int index = 0; index < party.Count; index++)
                {
                    sb.Append(roles[index].Name);
                    sb.Append(" : ");
                    sb.AppendLine(party[index] == null ? "" : party[index].Name + "さん");
                }
            }
            sb.AppendLine("-----------------------");

            ResultTextBox.Text = sb.ToString();
        }

        private Member SelectPriorityMember(IEnumerable<Member> Members, Role Role)
        {
            //一つしか役職が出来ない人を先に選択
            var member = Members.Where(d => d.Roles.Contains(Role) && d.Roles.Count == 1)
                          .FirstOrDefault();

            if (member == null)
            {
                member = Members.Where(d => d.Roles.Contains(Role))
                          .OrderBy(d => d.Roles.IndexOf(Role))
                          //.ThenBy(d => d.Roles.Count())
                          .FirstOrDefault();
            }
            return member;
        }
    }
}
