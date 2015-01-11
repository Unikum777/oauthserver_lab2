using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient; 
using System.Net;

namespace Live_com.Models
{
    public class DatabaseConnector: SkyDriveStorage
    {
        SqlConnection connection1;
        public String Error;
        public DatabaseConnector()
        {
            string dbLocation = System.IO.Path.GetFullPath(@"C:\Users\Никита\Source\Repos\Live_com\Live_com\App_Data\Database1.mdf");

            connection1 = new SqlConnection
                            (@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Никита\Source\Repos\Live_com\Live_com\App_Data\Database1.mdf;Integrated Security=True"
                            );
        }
        public Dictionary<string, string> GetFilesByUser(int UserId)
        {
            connection1.Open();

           string sql = "SELECT * FROM FILES WHERE Userid = " + UserId.ToString();
            SqlCommand command1 = new SqlCommand(sql, connection1);
            SqlDataReader dataReader1 = command1.ExecuteReader();

            // Организуем циклический перебор полученных записей и выводим название каждой планеты на метку
            var result = new Dictionary<string, string>();
            while (dataReader1.Read())
            {
                result.Add(Convert.ToString(dataReader1["FileId"]),Convert.ToString(dataReader1["FileName"]));
            }
            connection1.Close();
            return result;
        }
        public File GetFileByUserAndIndex(int UserId, int index)
        {
            try
            {
                connection1.Open();
                string sql = "SELECT * FROM FILES WHERE Userid = " + UserId.ToString() + " AND id= " + index.ToString();
                SqlCommand command1 = new SqlCommand(sql, connection1);
                SqlDataReader dataReader1 = command1.ExecuteReader();

                // Организуем циклический перебор полученных записей и выводим название каждой планеты на метку
                var result = new File();
                dataReader1.Read();
                result.ID = Convert.ToInt32(dataReader1["Id"]);
                result.FileId = Convert.ToString(dataReader1["FileId"]);
                result.FileName = Convert.ToString(dataReader1["FileName"]);
                result.Size = Convert.ToInt32(dataReader1["Size"]);
                connection1.Close();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public HttpStatusCode CheckUserByPass(string login, string Pass)
        {
            connection1.Open();
            string sql = "SELECT * FROM USERS WHERE Login = '" +  login + "' AND Password = '" + Pass+"'";
            SqlCommand command1 = new SqlCommand(sql, connection1);
            SqlDataReader dataReader1 = command1.ExecuteReader();
            dataReader1.Read();

            if (login == dataReader1["Login"].ToString() && Pass == dataReader1["Password"].ToString())
            {
                connection1.Close();
                return HttpStatusCode.Accepted;
            }
            Error = "Invalid Login or Password";
            return HttpStatusCode.Unauthorized;
        }

        public HttpStatusCode CheckUserByAccessToken(string AccessToken)
        {
            try
            {
                connection1.Open();
                string sql = "SELECT * FROM USERS WHERE Access_token = '" + AccessToken+"'";
                SqlCommand command1 = new SqlCommand(sql, connection1);
                SqlDataReader dataReader1 = command1.ExecuteReader();
                dataReader1.Read();
                if ( AccessToken == dataReader1["Access_token"].ToString() && Convert.ToInt32(dataReader1["Accepted"]) > 0 && DateTime.Now <= Convert.ToDateTime(dataReader1["Expired"]))
                {
                    connection1.Close();
                    return HttpStatusCode.Accepted;
                }
                else if (AccessToken == dataReader1["Access_token"] && DateTime.Now > Convert.ToDateTime(dataReader1["Expired"]))
                {
                    Error = "Access token was expired. Please refresh it";
                    return HttpStatusCode.Unauthorized;
                }
                Error = "Invalid Login or Password";
                return HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.Unauthorized;
            }
        }
        public User GetUserByRefreshToken(string RefreshToken)
        {
            try
            {
                connection1.Open();
                string sql = "SELECT * FROM USERS WHERE Refresh_token = '" + RefreshToken + "'";
                SqlCommand command1 = new SqlCommand(sql, connection1);
                SqlDataReader dataReader1 = command1.ExecuteReader();
                dataReader1.Read();
                if (RefreshToken == dataReader1["Refresh_token"].ToString())
                {
                    var model = new User();
                    model.id = Convert.ToInt32(dataReader1["Id"]);
                    model.login = Convert.ToString(dataReader1["Login"]);
                    model.RedirectUri = Convert.ToString(dataReader1["Redirect_uri"]);
                    connection1.Close();
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public HttpStatusCode AddFile(int UserId, string FileId, string FileName, int size, string Access)
        {
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "INSERT Files (UserId, FileId, FileName, Size, Access)" +
            " VALUES ("+ UserId.ToString() + ", "+ FileId + ", "+ FileName + ", "+ size.ToString() + ", "+ Access + ")";

            connection1.Open();
            cmd.ExecuteNonQuery();
            connection1.Close();
            return HttpStatusCode.OK;
        }
        public HttpStatusCode CreateNewUser(string login, string password, string e_mail)
        {
            try
            {
                connection1.Open();
                string sqlIns = "INSERT INTO Users (Login, Password, Email) VALUES (@login, @pass, @email)";
                SqlCommand cmdIns = new SqlCommand(sqlIns, connection1);
                cmdIns.Parameters.Add("@login", login);
                cmdIns.Parameters.Add("@pass", password);
                cmdIns.Parameters.Add("@email", e_mail);
                cmdIns.ExecuteNonQuery();
                cmdIns.Dispose();
                cmdIns = null;
                //cmd.ExecuteNonQuery();
                connection1.Close();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return HttpStatusCode.InternalServerError;
            }
            return HttpStatusCode.OK;

        }
        public HttpStatusCode SetAvailable(string login, bool value)
        {
            try
            {
                connection1.Open();
                string sqlIns = "UPDATE  Users SET [Accepted]= @accept WHERE Login = @login";
                SqlCommand cmdIns = new SqlCommand(sqlIns, connection1);
                cmdIns.Parameters.Add("@login", login);

                if (value)
                {
                    cmdIns.Parameters.Add("@accept", 1);
                }
                else
                {
                    cmdIns.Parameters.Add("@accept", 0);
                }
                cmdIns.ExecuteNonQuery();
                cmdIns.Dispose();
                cmdIns = null;
                connection1.Close();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return HttpStatusCode.InternalServerError;
            }
            return HttpStatusCode.OK;
        }
        public HttpStatusCode SetTokens(string login, string access_token, string refresh_token, DateTime date )
        {
            try
            {
                connection1.Open();
                string sqlIns = "UPDATE  Users SET [Access_token]= @AccessToken, [Refresh_token] = @RefreshToken, [Expired] = @Expired WHERE Login = @login";
                SqlCommand cmdIns = new SqlCommand(sqlIns, connection1);
                cmdIns.Parameters.Add("@login", login);
                cmdIns.Parameters.Add("@AccessToken", access_token);
                cmdIns.Parameters.Add("@RefreshToken", refresh_token);
                cmdIns.Parameters.Add("@Expired", date);
                cmdIns.ExecuteNonQuery();
                cmdIns.Dispose();
                cmdIns = null;
                connection1.Close();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return HttpStatusCode.InternalServerError;
            }
            return HttpStatusCode.OK;
        }

        public HttpStatusCode SetRedirectUri(string login, string redirect_uri)
        {
            try
            {
                connection1.Open();
                string sqlIns = "UPDATE  Users SET [Redirect_uri]= @Redirect WHERE Login = @login";
                SqlCommand cmdIns = new SqlCommand(sqlIns, connection1);
                cmdIns.Parameters.Add("@login", login);
                cmdIns.Parameters.Add("@Redirect", redirect_uri);
                cmdIns.ExecuteNonQuery();
                cmdIns.Dispose();
                cmdIns = null;
                connection1.Close();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return HttpStatusCode.InternalServerError;
            }
            return HttpStatusCode.OK;
        }
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            connection1.Open();
            string sql = "SELECT * FROM USERS";
            SqlCommand command1 = new SqlCommand(sql, connection1);
            SqlDataReader dataReader1 = command1.ExecuteReader();
            while (dataReader1.Read())
            {
                var temp = new User();
                temp.id = Convert.ToInt32(dataReader1["Id"]);
                temp.login = dataReader1["Login"].ToString();
                if (dataReader1["Email"] != null)
                {
                    temp.email = dataReader1["Email"].ToString();
                }
                else
                {
                    temp.email = "-";
                }
                if (dataReader1["Expired"].ToString() != "")
                {
                    temp.Expired = Convert.ToDateTime(dataReader1["Expired"]);
                }
                else
                {
                    temp.Expired = Convert.ToDateTime("01.01.1900");
                }
                users.Add(temp);
            }
            return users;
        }
        public User GetUserInfo(string access_token)
        {
            var user = new User();
            connection1.Open();
            string sql = "SELECT * FROM USERS WHERE Access_token = @Access";
            SqlCommand command1 = new SqlCommand(sql, connection1);
            command1.Parameters.Add("@Access", access_token);

            SqlDataReader dataReader1 = command1.ExecuteReader();
            if (dataReader1.Read())
            {
                user.id = Convert.ToInt32(dataReader1["Id"]);
                user.login = dataReader1["Login"].ToString();
                if (dataReader1["Email"] != null)
                {
                    user.email = dataReader1["Email"].ToString();
                }
                else
                {
                    user.email = "-";
                }
                if (dataReader1["Password"] != null)
                {
                    user.password = dataReader1["Password"].ToString();
                }
                else
                {
                    user.password = "-";
                }
                if (dataReader1["Redirect_uri"] != null)
                {
                    user.RedirectUri = dataReader1["Redirect_uri"].ToString();
                }
                else
                {
                    user.RedirectUri = "-";
                }

                if (dataReader1["Expired"].ToString() != "")
                {
                    user.Expired = Convert.ToDateTime(dataReader1["Expired"]);
                }
                else
                {
                    user.Expired = Convert.ToDateTime("01.01.1900");
                }
            }
            connection1.Close();
            return user;
        }
        public User GetUserInfoByLogin(string login)
        {
            var user = new User();
            connection1.Open();
            string sql = "SELECT * FROM USERS WHERE Login = @Login";
            SqlCommand command1 = new SqlCommand(sql, connection1);
            command1.Parameters.Add("@Login", login);

            SqlDataReader dataReader1 = command1.ExecuteReader();
            if (dataReader1.Read())
            {
                user.id = Convert.ToInt32(dataReader1["Id"]);
                user.login = dataReader1["Login"].ToString();
                if (dataReader1["Email"] != null)
                {
                    user.email = dataReader1["Email"].ToString();
                }
                else
                {
                    user.email = "-";
                }
                if (dataReader1["Password"] != null)
                {
                    user.password = dataReader1["Password"].ToString();
                }
                else
                {
                    user.password = "-";
                }
                if (dataReader1["Redirect_uri"] != null)
                {
                    user.RedirectUri = dataReader1["Redirect_uri"].ToString();
                }
                else
                {
                    user.RedirectUri = "-";
                }

                if (dataReader1["Expired"].ToString() != "")
                {
                    user.Expired = Convert.ToDateTime(dataReader1["Expired"]);
                }
                else
                {
                    user.Expired = Convert.ToDateTime("01.01.1900");
                }
            }
            connection1.Close();
            return user;
        }
        public HttpStatusCode CheckApp(string client_id, string client_secret)
        {
            try
            {
                connection1.Open();
                 string sql = "SELECT * FROM Apps WHERE ClientId = @Id  AND ClientSecret = @Secret";
                //string sql = "SELECT * FROM Apps WHERE ClientId = @Id";
                SqlCommand command1 = new SqlCommand(sql, connection1);
                command1.Parameters.AddWithValue("@Id", client_id);
                command1.Parameters.AddWithValue("@Secret", client_secret);
                SqlDataReader dataReader1 = command1.ExecuteReader();
                dataReader1.Read();

                if(client_id == dataReader1["ClientId"].ToString() && client_secret == dataReader1["ClientSecret"].ToString())
                {
                    connection1.Close();
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return HttpStatusCode.InternalServerError;
            }
        }

        public bool SetAuthCode(string login, string auth_code)
        {
            try
            {
                connection1.Open();
                string sqlIns = "INSERT INTO Authentication_code (code, login) VALUES (@code, @login)";
                SqlCommand cmdIns = new SqlCommand(sqlIns, connection1);
                cmdIns.Parameters.Add("@code", auth_code);
                cmdIns.Parameters.Add("@login", login);
                cmdIns.ExecuteNonQuery();
                cmdIns.Dispose();
                cmdIns = null;
                //cmd.ExecuteNonQuery();
                connection1.Close();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return false;
            }
            return true;
        }

        public string GetAuthCode(string auth_code)
        {
            try
            {
                connection1.Open();
                string sql = "SELECT * FROM Authentication_code WHERE Code = @Id";
                SqlCommand command1 = new SqlCommand(sql, connection1);
                command1.Parameters.AddWithValue("@Id", auth_code);
                SqlDataReader dataReader1 = command1.ExecuteReader();
                dataReader1.Read();
                string code = dataReader1["Code"].ToString();
                string login = dataReader1["Login"].ToString();
                connection1.Close();

                if  (code== auth_code)
                {
                    return login;
                }
                connection1.Close();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return null;
            }
            return null;
        }

        public HttpStatusCode UpdateAppInfo(string client_id, string client_secret, string redirect, string state)
        {
            try
            {
                if (CheckApp(client_id, client_secret) == HttpStatusCode.OK)
                {
                    connection1.Open();
                    string sqlIns = "UPDATE Apps SET [RedirectUri] = @Redirect,  [State] = @State WHERE [ClientId] = @Client";
                    SqlCommand cmdIns = new SqlCommand(sqlIns, connection1);

                    cmdIns.Parameters.AddWithValue("@Redirect", redirect);
                    cmdIns.Parameters.AddWithValue("@State", state);
                    cmdIns.Parameters.AddWithValue("@Client", client_id);
                    cmdIns.ExecuteNonQuery();
                    cmdIns.Dispose();
                    connection1.Close();
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return HttpStatusCode.InternalServerError;
            }

        }
    }
}