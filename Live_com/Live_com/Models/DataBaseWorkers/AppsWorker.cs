using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Live_com.Models;

namespace Live_com.Models.DataBaseWorkers
{
    public class AppsWorker
    {
        DataContext DataBase = new DataContext(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Никита\Source\Repos\Live_com\Live_com\App_Data\Database2.mdf;Integrated Security=True");
        public Apps CreateApp(string redir_uri, string state)
        {
            Apps NewApplication = new Apps
            {
                RedirectUri = redir_uri,
                State = state
            };
            NewApplication.ClientSecret = NewApplication.GenerateRandomString(20);
            Table<Apps> Applications = DataBase.GetTable<Apps>();
            Applications.InsertOnSubmit(NewApplication);
            DataBase.SubmitChanges();
            return NewApplication;
        }
        public Apps ReadApp(int client_id, string client_secret)
        {
            try
            {
                Table<Apps> Applications = DataBase.GetTable<Apps>();

                Apps custQuery =
                (from cust in Applications
                 where (cust.ClientId == client_id) && (cust.ClientSecret == client_secret)
                 select cust).First();
                return custQuery;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
        public List<Apps> ReadApps()
        {
            Table<Apps> Applications = DataBase.GetTable<Apps>();

            IQueryable<Apps> custQuery =
            from cust in Applications
            select cust;

            return custQuery.ToList<Apps>();
        }
        public Apps DeleteApp(int client_id)
        {
            Table<Apps> Applications = DataBase.GetTable<Apps>();

            Apps custQuery =
            (from cust in Applications
             where (cust.ClientId == client_id)
             select cust).First();
            Applications.DeleteOnSubmit(custQuery);
            return custQuery;
        }
    }
}