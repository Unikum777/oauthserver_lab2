using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Live_com.Models;

namespace Live_com.Models.DataBaseWorkers
{
    
         
    public class FoldersWorker
    {        
        
        DataContext DataBase = new DataContext(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Никита\Source\Repos\Live_com\Live_com\App_Data\Database2.mdf;Integrated Security=True");

        public Folder CreateFolder(string folder_name, string external_id, int user_id)
        {
            Folder f = new Folder
            {
                FolderName = folder_name,
                ExternalId = external_id,
                UserId = user_id
            };
            Table<Folder> Folders = DataBase.GetTable<Folder>();
            Folders.InsertOnSubmit(f);
            DataBase.SubmitChanges();
            return f;
        }
        public List<Folder> ReadFolders(int user_id)
        {
            Table<Folder> Folders = DataBase.GetTable<Folder>();
            try
            {
                IQueryable<Folder> custQuery =
                from cust in Folders
                where cust.UserId == user_id
                select cust;

                return custQuery.ToList<Folder>();
            }
            catch
            {
                return null;
            }
        }
        public Folder ReadFolder(int folder_id, int user_id)
        {
            Table<Folder> Folders = DataBase.GetTable<Folder>();
            try
            {
                Folder custQuery =
                (from cust in Folders
                 where (cust.Id == folder_id) && (cust.UserId == user_id)
                 select cust).First();

                return custQuery;
            }
            catch
            {
                return null;
            }
        }
        public Folder UpdateFolder(Folder folder)
        {
            DataBase.SubmitChanges();
            return folder;
        }
        public Folder DeleteFolder(int folder_id, string folder_name)
        {
            Table<Folder> Folders = DataBase.GetTable<Folder>();

            Folder custQuery =
            (from cust in Folders
             where (cust.Id == folder_id) || (cust.FolderName == folder_name)
             select cust).First();
            Folders.DeleteOnSubmit(custQuery);

            DataBase.SubmitChanges();
            return custQuery;
        }
    }
}