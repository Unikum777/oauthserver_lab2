using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Live_com.Models
{
    public class SkyDriveStorage
    {
        int UserId = 1;
        DatabaseConnector data; 
        public SkyDriveStorage()
        {
            Init();
        }
        public Dictionary<string, string> Get(string UserId)
        {
            data = new DatabaseConnector();
            return data.GetFilesByUser(Convert.ToInt32(UserId));
        }
        public Dictionary<string, string> Get(int UserId)
        {
            data = new DatabaseConnector();
            return data.GetFilesByUser(UserId);
        }

        public File Add(File f)
        {
            data = new DatabaseConnector();
            return data.Add(f);
        }

       /* public bool Delete(int id)
        {
            return files.Remove(id);
        }

        public bool Update(SharedFile f)
        {
            bool update = files.ContainsKey(f.ID);
            files[f.ID] = f;
            return update;
        }*/
        public void Init()
        {
           
        }
    }
}