using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Live_com.Models;
using Live_com.Models.DataBaseWorkers;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Web.Script.Serialization;
using System.Data.Objects;

namespace Live_com.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
       // GET: /About
       /* public ActionResult About()
        {
            var app_worker = new AppsWorker();
            var jsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string json = jsonSerializer.Serialize(app_worker.ReadApps());
            return new ObjectResult<Apps>(new Apps { 
                ClientId = 1,
                ClientSecret = "dfgadfgszfgb",
                RedirectUri = "www.ya.ru",
                State = "sf"
            });
        }*/
    }
}
