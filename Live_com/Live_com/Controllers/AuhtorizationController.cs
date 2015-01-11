using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Live_com.Models;
using System.Text;
using System.Web.Mvc;
using Live_com.Models.DataBaseWorkers;


namespace Live_com.Controllers
{
    public class AuhtorizationController : Controller
    {
        //
        // GET: /Auhtorization/
       // User CurrentUser;
        public ActionResult Index(string parameters)
        {
            return View();
        }
        public ActionResult CreateUser(string parameters)
        {
            return View();
        }
        /* 
            * Test string: http://localhost:32596/Auhtorization/oauth?client_id=5&client_secret=SuperPuperSecretKey&redirect_uri=www.yandex.ru&state=desctop
        */
        public ActionResult Oauth(int client_id, string client_secret, string redirect_uri, string state)
        {
            var appworker = new AppsWorker();
            try
            {
                var current_app = appworker.ReadApp(client_id, client_secret);
                if (current_app == null)
                {
                    throw (new Exception("Invalid cliend Id or client Secret"));
                }
                else
                {
                    Session["RedirectUri"] = redirect_uri;
                    return View("Oauth");
                }
            }
            catch (Exception ex)
            {
                return View("OauthErrorPage", ex);
            }
        }
        
        public ActionResult LoginAuthorization()
        {
            string login = Request["txtAmount"].ToString();
            string password = Request["txtRate"].ToString();
            var user_worker = new UsersWorker();
            var current_user = user_worker.ReadUser(login);
            current_user.Password = current_user.Password.Trim();
            if (current_user == null)
            {
                return View("User not found");
            }
            else if (current_user.Password != password)
            {
                return View("Invalid password");
            }
            else
            {
                Session["Login"] = current_user.Login;
                return View("LoginAuthorization", current_user);
            }
        }

        public ActionResult CreateNewUser()
        {
            string login = Request["txtLogin"].ToString();
            string password = Request["txtPassword"].ToString();
            string email = Request["txtEmail"].ToString();
            string phone = Request["txtPhone"].ToString();
            try
            {
                var user_worker = new UsersWorker();
                var current_user = user_worker.CreateUser(login, password, email, phone);
                return View("CreateNewUser");
            }
            catch (Exception ex)
            {
                return View("CreateUserError", "Creating error " + ex.Message);
            }
         }
        public ActionResult Accepted()
        {
            var userworker = new UsersWorker();
            var current_user = userworker.ReadUser(Session["Login"].ToString());
            current_user.AuthorizationCode = current_user.GenerateRandomString(20);
            current_user.RedirectUri = Session["RedirectUri"].ToString();
            current_user.Permissions = 1;
            var dict = new Dictionary<string, string>();
            dict.Add("authentication_code", current_user.AuthorizationCode);

            Session["AuthCode"] = current_user.AuthorizationCode;
            userworker.UpdateUser(current_user);
            Response.Redirect("http://" + current_user.RedirectUri + "?auth_code=" + current_user.AuthorizationCode);
            return Json(dict, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DontAccepted()
        {
            var userworker = new UsersWorker();
            var current_user = userworker.ReadUser(Session["Login"].ToString());
            current_user.RedirectUri = Session["RedirectUri"].ToString();
            current_user.Permissions = 0;
            return View();
        }
        /*public string GenerateRandomString(int length)
        {
            var str = new Char[length];
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                if (rnd.Next(2)==0)
                {
                    str[i] = Convert.ToChar(rnd.Next(26) + 'A');
                }
                else
                {
                    str[i] = Convert.ToChar(rnd.Next(26) + 'a');
                }
                
            }
            return new string (str);
        }*/

    }
}
