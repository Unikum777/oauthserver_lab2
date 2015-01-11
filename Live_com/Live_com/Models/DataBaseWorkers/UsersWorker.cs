using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Live_com.Models;

namespace Live_com.Models.DataBaseWorkers
{
    public class UsersWorker
    {
        DataContext DataBase = new DataContext(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Никита\Source\Repos\Live_com\Live_com\App_Data\Database2.mdf;Integrated Security=True");
        public User CreateUser(string login, string pass, string email, string phone)
        {
            
            Table<User> Users = DataBase.GetTable<User>();
            User NewUser = new User
            {
                Login = login,
                Password = pass,
                Email = email,
                Phone = phone,
                Permissions = 0,
                Id = Users.Count()+1
            };
            
            Users.InsertOnSubmit(NewUser);
            DataBase.SubmitChanges();
            return NewUser;
        }
        public User AppendAuthCode(string login)
        {
            Table<User> Users = DataBase.GetTable<User>();
            var existingUser =
            (from c in Users
             where c.Login == login
             select c)
            .First();

            existingUser.AuthorizationCode = existingUser.GenerateRandomString(20);
            DataBase.SubmitChanges();
            return existingUser;
        }
        public User AppendRedirectUri(string login, string redirect_uri)
        {
            Table<User> Users = DataBase.GetTable<User>();
            var existingUser =
            (from c in Users
             where c.Login == login
             select c)
            .First();

            existingUser.RedirectUri = redirect_uri;
            DataBase.SubmitChanges();
            return existingUser;
        }
        public User AppendAccessToken(User current_user)
        {
            current_user.AccessToken = current_user.GenerateRandomString(20);
            current_user.AuthorizationCode = "";
            if (current_user.RefreshToken == null)
            {
                current_user.RefreshToken = current_user.GenerateRandomString(20);
            }
            current_user.ExpiredDateTime = DateTime.Now;
            DataBase.SubmitChanges();
            return current_user;
        }
        public User UpdateUser(User user)
        {
            DataBase.SubmitChanges();
            return user;
        }
        public User AcceptPermissions(string login)
        {
            Table<User> Users = DataBase.GetTable<User>();
            var existingUser =
            (from c in Users
             where c.Login == login
             select c)
            .First();

            existingUser.Permissions = 1;
            DataBase.SubmitChanges();
            return existingUser;
        }
        public User DecceptPermissions(string login)
        {
            Table<User> Users = DataBase.GetTable<User>();
            var existingUser =
            (from c in Users
             where c.Login == login
             select c)
            .First();

            existingUser.Permissions = 0;
            DataBase.SubmitChanges();
            return existingUser;
        }
        public User ReadUser(string login)
        {
            try
            {
                Table<User> Users = DataBase.GetTable<User>();
                var existingUser =
                (from c in Users
                 where c.Login == login
                 select c)
                .First();
                return existingUser;
            }
            catch
            {
                return null;
            }
            
        }
        public User ReadUserByAuthCode(string authorization_code)
        {
            try
            {
                Table<User> Users = DataBase.GetTable<User>();
                var existingUser =
                (from c in Users
                 where c.AuthorizationCode == authorization_code
                 select c)
                .First();
                return existingUser;
            }
            catch
            {
                return null;
            }
        }
        public User ReadUserByAccessToken(string access_token)
        {
            try
            {
                Table<User> Users = DataBase.GetTable<User>();
                var existingUser =
                (from c in Users
                 where c.AccessToken == access_token
                 select c)
                .First();
                return existingUser;
            }
            catch
            {
                return null;
            }
        }
        public User ReadUserByRefreshToken(string refresh_token)
        {
            try
            {
                Table<User> Users = DataBase.GetTable<User>();
                var existingUser =
                (from c in Users
                 where c.RefreshToken == refresh_token
                 select c)
                .First();
                return existingUser;
            }
            catch
            {
                return null;
            }
        }
        public User DeleteUser(string login)
        {
            Table<User> Users = DataBase.GetTable<User>();
            var existingUser =
            (from c in Users
             where c.Login == login
             select c)
            .First();
            Users.DeleteOnSubmit(existingUser);
            DataBase.SubmitChanges();
            return existingUser;
        }
    }
}