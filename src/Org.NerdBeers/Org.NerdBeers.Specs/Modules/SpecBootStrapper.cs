using System;
using System.IO;
using System.Reflection;
using Nancy.Authentication.Forms;
using Org.NerdBeers.Web.Services;
using Simple.Data.Sqlite;
using Simple.Data;
using Org.NerdBeers.Web.Models;
using Nancy.Security;

namespace Org.NerdBeers.Specs.Modules
{
    class SpecBootStrapper : NerdBeers.Web.Bootstrapper,IDisposable
    {
        public CustomDBFactory fact;
        public FakeUserMapper fusermapper;

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            fact =  new CustomDBFactory();
            fusermapper = new FakeUserMapper(fact);
            container.Register<IDBFactory>(fact);
            container.Register<FakeUserMapper>();
            base.ConfigureApplicationContainer(container);
            DB = container.Resolve<IDBFactory>().DB();
        }

        public dynamic DB { get; protected set; }

        public void Dispose()
        {
            fact.Dispose();
        }
    }

        public class FakeUserMapper : IUserMapper
        {
            dynamic DB;

            public FakeUserMapper(IDBFactory DBFactory)
            {
                this.DB = DBFactory.DB();
            }

            public IUserIdentity GetUserFromIdentifier(Guid indentifier)
            {
                Nerd n = DB.Nerds.FindById(1);
                return n;
            }
        }

    class CustomDBFactory : IDBFactory,IDisposable
    {
        static bool IsInitialized = false;

        const string connectionString = "Data Source=:memory:";
        static IInMemoryDbConnection connection;
        static dynamic db;

        public dynamic DB()
        {
            if (!IsInitialized)
            {
                Dispose();
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
            if (IsInitialized)
            {
                connection.Destroy();
                connection.Dispose();
                connection = null;
                IsInitialized = false;
            }
        }
    }
}
