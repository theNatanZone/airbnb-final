using AirbnbProj2.DAL;
using System.Diagnostics;

namespace AirbnbProj2.Models
{
    public class User
    {
        private string? firstName;
        private string? familyName;
        private string email;
        private string password;
        private bool isActive = true;
        private bool isAdmin = false;

        public User() { }

        public User(string? firstName, string? familyName, string email, string password, bool isActive, bool isAdmin)
        {
            FirstName = firstName;
            FamilyName = familyName;
            Email = email;
            Password = password;
            IsActive = isActive;
            IsAdmin = isAdmin;
        }

        public string? FirstName { get => firstName; set => firstName = value; }
        public string? FamilyName { get => familyName; set => familyName = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public bool IsActive { get => isActive; internal set => isActive = value;}
        public bool IsAdmin { get => isAdmin; internal set => isAdmin = value; }

    }
}
