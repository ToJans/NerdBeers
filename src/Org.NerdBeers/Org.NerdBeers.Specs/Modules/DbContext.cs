using System;
using System.Data;
using System.IO;
using Machine.Specifications;
using Simple.Data;
using Simple.Data.Ado;
using Simple.Data.Sqlite;

namespace Org.NerdBeers.Specs.Modules
{
    public abstract class DbContext
    {
        Establish context = () =>
        {
            connectionString = "Data Source=:memory:";
            db = Database.OpenConnection(connectionString);
            connection = ProviderHelper.GetProviderByConnectionString(connectionString)
                                       .CreateConnection();

            connection.Open();

            GenerateSchema();
        };

        Cleanup cleanup = () =>
        {
            ((SqliteInMemoryDbConnection)connection).KillDashNine();
            connection = null;
            db = null;
        };

        static void GenerateSchema()
        {
            var schema = File.ReadAllText(GetFullPath("BuildNerdDb.SQLite.txt"));
            var command = connection.CreateCommand();
            command.CommandText = schema;
            command.ExecuteNonQuery();
        }

        static string GetFullPath(string fn)
        {
            return Path.Combine(Path.GetDirectoryName(typeof(DbContext).Assembly.Location), fn);
        }

        protected static string connectionString;
        protected static dynamic db;
        protected static IDbConnection connection;
    }

    public class with_the_db_context : DbContext
    {
        Because of = () =>
        {
            beerEvent = db.BeerEvents.Insert(Name: "Drunkapalooza", Location: "Paddy Coynes", EventDate: DateTime.Today);
        };

        It should_have_a_result = () =>
        {
            
            beerEvent.ShouldNotBeNull();
        };

        static object beerEvent;
    }
}