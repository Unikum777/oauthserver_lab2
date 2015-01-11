using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class MeController : ApiController
    {
        int ExpiredDay = 3;
        int FilesOnPage = 3;
        // GET api/me
        public string Get()
        {
            try
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
                else
                {
                    return new JavaScriptSerializer().Serialize(current_user);
                }
            }
            catch
            {
                var notacceptable_response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                notacceptable_response.Headers.Add("Error", "Access error");
                throw new HttpResponseException(notacceptable_response);
            }
        }
    }
}
