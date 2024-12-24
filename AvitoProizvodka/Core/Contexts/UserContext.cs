using AvitoProizvodka.Data;
using System;

namespace AvitoProizvodka.Core.Contexts
{
    public class UserContext
    {
        public string SearchText { get; set; }

        public User User { get; set; }
        public User SelectedUser { get; set; }

        public void Cleanup()
        {
            SearchText = string.Empty;
            User = new User();
            SelectedUser = new User();
        }

        public void AddByUserId(int userId)
        {
            AddedByUserId?.Invoke(userId);
        }
        public void RemoveByUserId(int userId)
        {
            RemovedByUserId?.Invoke(userId);
        }

        public event Action<int> AddedByUserId;
        public event Action<int> RemovedByUserId;
    }
}
