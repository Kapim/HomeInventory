using HomeInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeInventory.Domain
{
    public class User
    {
        public User()
        {

        }

        public User(string userName, string passwordHash, UserRole userRole)
        {
            Rename(userName);
            ChangePassword(passwordHash);
            ChangeRole(userRole);
        }


        public Guid Id { get; private set; } = Guid.NewGuid();
        public string UserName { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRole UserRole { get; private set; }

        public void ChangePassword(string newPassword)
        {
            PasswordHash = newPassword;
        }

        public void ChangeRole(UserRole newRole)
        {
            UserRole = newRole;
        }

        public void Rename(string newName)
        {
            UserName = newName.Trim();
        }
    }
}
