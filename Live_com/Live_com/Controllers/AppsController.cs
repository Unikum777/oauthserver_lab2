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
    public class AppsController : ApiController
    {
        // GET api/apps
        public string Get()
        {
            var app_worker = new AppsWorker();
            var result = app_worker.ReadApps();
            return new JavaScriptSerializer().Serialize(result);
        }
    }
}
