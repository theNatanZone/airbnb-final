using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using AirbnbProj2.Models;

namespace AirbnbProj2.DAL
{
    public class UsersService
    {
        private readonly DBService _dbService;

        public UsersService(DBService dbService) 
        {
            _dbService = dbService;
        }

        public bool InsertUser(User user)
        {
            try
            {
                var dbParameters = new DbParameter[]
                {
                    new SqlParameter("@firstName", user.FirstName),
                    new SqlParameter("@familyName", user.FamilyName),
                    new SqlParameter("@email", user.Email.ToLower()),
                    new SqlParameter("@password", user.Password)
                };

                var result = _dbService.Insert("spInsertUser2024", dbParameters);
                return result != 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception thrown while user insertion: {ex}");
                throw;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                var dbParameters = new DbParameter[]
                {
                    new SqlParameter("@firstName", user.FirstName),
                    new SqlParameter("@familyName", user.FamilyName),
                    new SqlParameter("@email", user.Email.ToLower()),
                    new SqlParameter("@password", user.Password),
                    new SqlParameter("@isActive", user.IsActive),
                    new SqlParameter("@isAdmin", user.IsAdmin)
                };
                
                var result = _dbService.Update("spUpdateUser2024", dbParameters);
                return result != 0;

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception thrown while updating user: {e}");
                throw;
            }
        }

        public bool ActivateUser(string email)
        {
            try
            {
                var dbParameters = new DbParameter[]
                {
                    new SqlParameter("@email", email.ToLower()),
                    new SqlParameter("@isActive", true)
                };

                var result = _dbService.Update("spActivateUser2024", dbParameters);
                return result != 0;

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception thrown while activating user: {e}");
                throw;
            }
        }

        public bool DeactivateUser(string email)
        {
            try
            {
                var dbParameters = new DbParameter[]
                {
                    new SqlParameter("@email", email.ToLower()),
                    new SqlParameter("@isActive", false)
                };

                var result = _dbService.Update("spDeactivateUser2024", dbParameters);
                return result != 0;

            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception thrown while deactivating user: {e}");
                throw;
            }
        }

        public List<User> GetUsers() 
        {
            return _dbService.ReadUsers(); 
        }

        public User? GetUserByEmail(string userEmail)
        {
            if(!CheckEmail(userEmail))
                return null;
            
            var user = _dbService.ReadUsers().FirstOrDefault(u => u.Email.ToLower() == userEmail.ToLower());
            return user;
        }

        public void ValidateUserModel(User user)
        {
            if (!CheckEmail(user.Email))
                throw new FormatException("FormatException: Invalid Email address");

            if (!CheckPassword(user.Password))
                throw new ArgumentOutOfRangeException(nameof(user.Password),"ArgumentOutOfRangeException: Password must be between 5 to 64 characters.");

            if (string.IsNullOrEmpty(user.FirstName))
                throw new ArgumentException("ArgumentException: the field 'firstName' is required.");

            if (string.IsNullOrEmpty(user.FamilyName))
                throw new ArgumentException("ArgumentException: the field 'familyName' is required.");
        }

        public bool CheckPassword(string userPassword)
        {
            return userPassword is { Length: >= 5 and <= 64 };
        }

        public bool CheckEmail(string email)
        {
            try
            {
                var m = new MailAddress(email.ToLower());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public User? GetAuthenticatedUser(UserCredentials credentials)
        {
            var existingUser = GetUserByEmail(credentials.Email);
            return existingUser?.Password == credentials.Password ? existingUser : null;
        }


    }
}
