using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.NerdBeers.Web.Services;
using System.Data;
using Simple.Data.Ado;
using System.IO;
using Simple.Data.Sqlite;

namespace Org.NerdBeers.Specs.Helpers
{
    public class TestDBFactory : IDBFactory, IDisposable
    {
        public IDbConnection connection;
        dynamic db;
        Action CleanUpAction;
        
        public TestDBFactory():this(":memory:") 
        {}

        public TestDBFactory(string filename)
        {
            if (filename!=":memory:") 
                filename = FileHelpers.GetFullPath(filename);
            var connectionString = string.Format("Data Source = {0}; Version = 3;", filename);
            if (filename == ":memory:")
            {
                CleanUpAction = () =>
                {
                    if (connection != null)
                    {
                        ((SqliteInMemoryDbConnection)connection).KillDashNine();
                        connection = null;
                    }
                };
            }
            else
            {
                CleanUpAction = () =>
                {
                    if (File.Exists(filename))
                        File.Delete(filename);
                };
            }

            // first open the db, so it gets cached
            db = Simple.Data.Database.OpenConnection(connectionString);

            // prepare DB schema using the same connectionstring (which is cached)
            connection = ProviderHelper.GetProviderByConnectionString(connectionString).CreateConnection();
            connection.Open();
        }

        public dynamic DB { get { return db; } }

        public void Dispose()
        {
            CleanUpAction();
        }
    }
}
