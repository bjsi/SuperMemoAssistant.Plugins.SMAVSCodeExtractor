using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.OrmLite.Sqlite;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace SMACodeExtracts
{
    public static class DataAccess
    {
        public static string DBPath = @"C:\Users\james\Desktop\VSExtracts.db";
        public static OrmLiteConnectionFactory dbFactory = new OrmLiteConnectionFactory(DBPath, SqliteDialect.Provider);
        public static async Task CreateExtractAsync(Extract extract)
        {
            try
            {

                using (var db = dbFactory.Open())
                {
                    db.CreateTableIfNotExists<Extract>();
                    await db.InsertAsync(extract);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to insert extract with exception {e}");
            }
        }
    }
}
