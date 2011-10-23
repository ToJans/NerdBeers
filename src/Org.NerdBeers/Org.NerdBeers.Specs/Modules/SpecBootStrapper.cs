using System;
using System.IO;
using System.Reflection;
using Nancy.Authentication.Forms;
using Org.NerdBeers.Web.Services;
using Simple.Data.Sqlite;
using Simple.Data;

namespace Org.NerdBeers.Specs.Modules
{
    class SpecBootStrapper : NerdBeers.Web.Bootstrapper,IDisposable
    {
        CustomDBFactory fact = new CustomDBFactory();

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<IDBFactory>(fact);
            container.Register<IUserMapper>(new UserMapper(fact));
        }

        public dynamic DB
        {
            get {
                return fact.DB();
            }
        }

        public void Dispose()
        {
            fact.Dispose();
        }
    }

    class CustomDBFactory : IDBFactory,IDisposable
    {
        bool IsInitialized = false;

        const string connectionString = "Data Source=:memory:";
        IInMemoryDbConnection connection;
        dynamic db;

        public dynamic DB()
        {
            if (!IsInitialized)
            {
                connection = Database.Opener.OpenMemoryConnection(connectionString);
                db = Database.OpenConnection(connectionString);
                var createTableSql = Properties.Resources.CreateSqliteDML;
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = createTableSql;
                command.ExecuteNonQuery();
                IsInitialized = true;
            }
            return db;
        }

        public void Dispose()
        {
            connection.Destroy();
            connection.Dispose();
            connection = null;
        }
    }
}
