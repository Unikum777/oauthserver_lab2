using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Live_com.Models;

namespace Live_com.Models.DataBaseWorkers
{
    public class FilesWorker
    {
        DataContext DataBase = new DataContext(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Никита\Source\Repos\Live_com\Live_com\App_Data\Database2.mdf;Integrated Security=True");
        public File CreateFile(string file_name, int size, string external_id, int user_id, int folder_id)
        {
            File f = new File
            {
                Access = 0,
                Size = size,
                FileName = file_name,
                ExternalId = external_id,
                UserId = user_id,
                FolderId = folder_id,
            };
            Table<File> Files = DataBase.GetTable<File>();
            Files.InsertOnSubmit(f);
            DataBase.SubmitChanges();
            return f;
        }
        public List<File> ReadFiles (int user_id)
        {
            Table<File> Files = DataBase.GetTable<File>();

            IQueryable<File> custQuery =
            from cust in Files
            where cust.UserId == user_id
            select cust;

            return custQuery.ToList<File>();
        }
        public List<File> ReadFilesByFolder(int user_id, int folder_id)
        {
            Table<File> Files = DataBase.GetTable<File>();
            try
            {
                IQueryable<File> custQuery =
                from cust in Files
                where (cust.UserId == user_id) && (cust.FolderId == folder_id)
                select cust;

                return custQuery.ToList<File>();
            }
            catch
            {
                return null;
            }
        }
        public File ReadFile (int file_id, int user_id)
        {
            Table<File> Files = DataBase.GetTable<File>();
            try
            {
                File custQuery =
                (from cust in Files
                 where (cust.Id == file_id) && (cust.UserId == user_id)
                 select cust).First();

                return custQuery;
            }
            catch
            {
                return null;
            }
        }
        public File UpdateFile(File file)
        {
            DataBase.SubmitChanges();
            return file;
        }
        public File DeleteFile(int file_id, string file_name)
        {
            Table<File> Files = DataBase.GetTable<File>();

            File custQuery =
            (from cust in Files
             where (cust.Id == file_id) || (cust.FileName == file_name)
             select cust).First();
            Files.DeleteOnSubmit(custQuery);

            DataBase.SubmitChanges();
            return custQuery;
        }
    }
}