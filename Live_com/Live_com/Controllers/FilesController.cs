using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Specialized;
using Live_com.Models;
using Live_com.Models.DataBaseWorkers;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;


namespace Live_com.Controllers
{
    public class FilesController : ApiController
    {
        int ExpiredDay = 3;
        int FilesOnPage = 3;

        // GET api/files
        //Invoke-RestMethod http://localhost:32596/api/files -Headers @{"access_token"="tzIziWegXJcAdiuKAaUZ"}  
        public string Get()
        {
            var arr1 = Request.Headers.First(p => p.Key == "access_token").Value.ToList<string>();
            string access_token = Convert.ToString(arr1[0]);
            var user_worker = new UsersWorker();
            var current_user = user_worker.ReadUserByAccessToken(access_token);

            if (current_user == null)
            {
                var unauthorized_response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                unauthorized_response.Headers.Add("Error", "Invalid access token");
                throw new HttpResponseException(unauthorized_response);
            }
            else if (Convert.ToDateTime(current_user.ExpiredDateTime).AddDays(ExpiredDay) < DateTime.Now)
            {
                var notacceptable_response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                notacceptable_response.Headers.Add("Error", "Access token expired exception. Refresh token");
                throw new HttpResponseException(notacceptable_response);   
            }
            else if (current_user.Permissions == 0)
            {
                var notacceptable_response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                notacceptable_response.Headers.Add("Error", "Permission denied");
                throw new HttpResponseException(notacceptable_response);
            }
            else
            {
                var files_worker = new FilesWorker();
                var arr = files_worker.ReadFiles(current_user.Id);
                var short_arr = new List<ShortFileInfo>();
                foreach(var item in arr)
                {
                    short_arr.Add(new ShortFileInfo { FileName = item.FileName, Id = item.Id, Size = item.Size });
                }

                return new JavaScriptSerializer().Serialize(short_arr);
            }
        }

        // GET api/files/5
        public Object Get(int id)
        {
            var arr1 = Request.Headers.First(p => p.Key == "access_token").Value.ToList<string>();
            string access_token = Convert.ToString(arr1[0]);
            var user_worker = new UsersWorker();
            var current_user = user_worker.ReadUserByAccessToken(access_token);

            if (current_user == null)
            {
                var unauthorized_response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                unauthorized_response.Headers.Add("Error", "Invalid access token");
                throw new HttpResponseException(unauthorized_response);
            }
            else if (Convert.ToDateTime(current_user.ExpiredDateTime).AddDays(ExpiredDay) < DateTime.Now)
            {
                var notacceptable_response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                notacceptable_response.Headers.Add("Error", "Access token expired exception. Refresh token");
                throw new HttpResponseException(notacceptable_response);
            }
            else if (current_user.Permissions == 0)
            {
                var notacceptable_response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                notacceptable_response.Headers.Add("Error", "Permission denied");
                throw new HttpResponseException(notacceptable_response);
            }
            else if (id < 1)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NoContent);
                response.Headers.Add("Error", "Page number < 1");
                throw new HttpResponseException(response);
            }
            else
            {
                var files_worker = new FilesWorker();
                var arr = files_worker.ReadFiles(current_user.Id);
                var result = new Dictionary<string, Object>();
                result.Add("Page", id);
                var short_arr = new List<ShortFileInfo>();
                for (int i = id * FilesOnPage - FilesOnPage; i < id * FilesOnPage; i++ )
                {
                    if (arr.Count > i)
                    {
                        short_arr.Add(new ShortFileInfo { FileName = arr[i].FileName, Id = arr[i].Id, Size = arr[i].Size });
                    }
                }
                result.Add("Data", short_arr);
                return new JavaScriptSerializer().Serialize(result);
            }
        }

        // POST api/files

        /*
         Invoke-RestMethod -Method Post -Uri http://localhost:32596/api/files -Headers @{"client_id"="5";"client_secret"="SuperPuperSecretKey";"redirect_uri"="www.yandex.ru";"Auth_code"="Value";"refresh_token"="Value";"grand_type"="Value"}
         */
       // public Dictionary<string, string> PostFile([FromBody]string value)
        public string PostFile([FromBody]string value)
        {
            try
            {
                var arr1 = Request.Headers.First(p => p.Key == "client_id").Value.ToList<string>();
                int client_id = Convert.ToInt32(arr1[0]);

                arr1 = Request.Headers.First(p => p.Key == "client_secret").Value.ToList<string>();
                string client_secret = Convert.ToString(arr1[0]);

                arr1 = Request.Headers.First(p => p.Key == "redirect_uri").Value.ToList<string>();
                string redirect_uri = Convert.ToString(arr1[0]);

                arr1 = Request.Headers.First(p => p.Key == "grand_type").Value.ToList<string>();
                string grand_type = Convert.ToString(arr1[0]);

                if (grand_type == "authorization")
                {
                    arr1 = Request.Headers.First(p => p.Key == "Auth_code").Value.ToList<string>();
                    string Auth_code = Convert.ToString(arr1[0]);


                    var user_worker = new UsersWorker();
                    var current_user = user_worker.ReadUserByAuthCode(Auth_code);
                    if (current_user == null)
                    {
                        throw (new Exception("Не найден authorization code " + Auth_code));
                    }
                    current_user = user_worker.AppendAccessToken(current_user);

                    var dict = new Dictionary<string, string>();
                    dict.Add("access_token", current_user.AccessToken);
                    dict.Add("refresh_token", current_user.RefreshToken);
                    dict.Add("expired", current_user.ExpiredDateTime.ToString());
                    return new JavaScriptSerializer().Serialize(dict);
                   // return dict;
                }
                else if (grand_type == "refresh")
                {
                    arr1 = Request.Headers.First(p => p.Key == "refresh_token").Value.ToList<string>();
                    string refresh_token = Convert.ToString(arr1[0]);

                    var user_worker = new UsersWorker();
                    var current_user = user_worker.ReadUserByRefreshToken(refresh_token);
                    if (current_user == null)
                    {
                        throw (new Exception("Не найден refresh_token " + refresh_token));
                    }
                    current_user = user_worker.AppendAccessToken(current_user);

                    var dict = new Dictionary<string, string>();
                    dict.Add("access_token", current_user.AccessToken);
                    dict.Add("refresh_token", current_user.RefreshToken);
                    dict.Add("expired", current_user.ExpiredDateTime.ToString());
                     return new JavaScriptSerializer().Serialize(dict);
                    //return dict;
                }
                else
                {
                    throw (new Exception("Неверно задан grand_type"));
                }
            }
            catch (Exception ex)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("Error", "Invalid POST operation: " + ex.Message);
                return new JavaScriptSerializer().Serialize(dict);
                //return dict;
            }
        }
       

        // PUT api/files/5
        public void Put(int id, [FromBody]string value)
        {
        }
        public string GenerateRandomString(int length)
        {
            var str = new Char[length];
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < length; i++)
            {
                if (rnd.Next(2) == 0)
                {
                    str[i] = Convert.ToChar(rnd.Next(26) + 'A');
                }
                else
                {
                    str[i] = Convert.ToChar(rnd.Next(26) + 'a');
                }
            }
            return new string(str);
        }
    }
}