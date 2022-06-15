using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartyPlanner
{
    public class Settings
    {
        public static Settings Instance = new Settings();


        public ObservableCollection<Role> Roles = new ObservableCollection<Role>();

        public ObservableCollection<Member> Members = new ObservableCollection<Member>();

        public ObservableCollection<Role> SelectedRoles = new ObservableCollection<Role>();

        public void Apply(Settings NewSettings)
        {
            ObservableApply(NewSettings.Roles, Roles);
            ObservableApply(NewSettings.Members, Members);
            ObservableApply(NewSettings.SelectedRoles, SelectedRoles);
        }

        private void ObservableApply<T>(ObservableCollection<T> source, ObservableCollection<T> target)
        {
            target.Clear();
            foreach (var data in source)
            {
                target.Add(data);
            }
        }
    }

    public class Role
    {
        public string Name { get; set; }

        public static bool operator ==(Role r1, Role r2) => r1.Name == r2.Name;
        public static bool operator !=(Role r1, Role r2) => r1.Name != r2.Name;
        public override bool Equals(object obj) => obj is Role r ? this == r : false;
        public override int GetHashCode() => Name.GetHashCode();
    }

    public class Member
    {
        public string Name { get; set; }
        public List<Role> Roles { get; set; }
        public bool IsChecked { get; set; }
        public string RolesString => string.Join(", ", Roles.Select(d => d.Name));
    }
}
