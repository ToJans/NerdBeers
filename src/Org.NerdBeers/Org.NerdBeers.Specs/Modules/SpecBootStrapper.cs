using System;
using System.IO;
using System.Reflection;
using Nancy.Authentication.Forms;
using Org.NerdBeers.Web.Services;
using System.Diagnostics;
using Simple.Data.Sqlite;

namespace Org.NerdBeers.Specs.Modules
{
    class SpecBootStrapper : NerdBeers.Web.Bootstrapper
    {
        protected static SQLiteDBFactory dbfact = new SQLiteDBFactory();

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<IDBFactory>(dbfact);
            container.Register<IUsernameMapper, UsernameMapper>();
        }

        public dynamic DB
        {
            get
            {
                return dbfact.DB();
            }
        }
    }

    class SQLiteDBFactory : IDBFactory
    {
        [ThreadStatic]
        static string InitSqlDb = null;
        [ThreadStatic]
        static string fname = 
            //GetFullPath(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()+ ".db"); //file
            ":memory:"; // inmem
        [ThreadStatic]
        static string constring = string.Format("Data Source = {0}; Version = 3;", fname);

        [ThreadStatic]
        static DateTime RefDate = DateTime.Now.AddMonths(1);
        [ThreadStatic]
        static System.Data.IDbConnection connection;
        [ThreadStatic]
        static dynamic db;

        static string GetFullPath(string fn)
        {
            return Path.Combine(Path.GetDirectoryName(typeof(SQLiteDBFactory).Assembly.Location), fn);
        }


        public static void ResetDB()
        {

            if (fname == ":memory:")
            {
                if (connection != null)
                {
                    ((SqliteInMemoryDbConnection)connection).KillDashNine();
                    connection = null;
                    db = null;
                }
            }
            else
            {
                if (File.Exists(fname))
                    File.Delete(fname);
            }


            InitSqlDb = InitSqlDb ??
                File.ReadAllText(GetFullPath("BuildNerdDb.SQLite.txt"));

            // prepare DB schema
            connection = Simple.Data.Ado.ProviderHelper.GetProviderByConnectionString(constring).CreateConnection();
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = InitSqlDb;
            cmd.ExecuteNonQuery();

            // insert testdata
            db = Simple.Data.Database.OpenConnection(constring);
            int NerdId = (int)db.Nerds.Insert(Guid: "xxx", Name: "Tom").Id;
            int EventId = (int)db.BeerEvents.Insert(Name: "First nerdbeer event", EventDate: RefDate, Location: "Everywhere").Id;
            db.NerdSubscriptions.Insert(EventId: EventId, NerdId: NerdId);
            db.Comments.Insert(EventId: EventId, NerdId: NerdId, CommentText: "Hakuna matata", Created: RefDate.AddHours(1));
        }

        public dynamic DB()
        {
            return Simple.Data.Database.OpenConnection(constring); ;
        }

    }
}
