// original code by @notmyself
// https://gist.github.com/886279

using System;
using System.Data;
using System.Linq;
using System.IO;
using Machine.Specifications;
using Simple.Data;
using Simple.Data.Ado;
using Simple.Data.Sqlite;
using System.Collections.Generic;
using Org.NerdBeers.Web.Models;
using Org.NerdBeers.Specs.Modules;
using System.Dynamic;
using System.Diagnostics;
using Nancy;

namespace Org.NerdBeers.Specs
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
            return Path.Combine(Path.GetDirectoryName(typeof(with_the_db_context).Assembly.Location), fn);
        }

        protected static string connectionString;
        protected static dynamic db;
        protected static IDbConnection connection;
    }

    public class with_the_db_context : DbContext
    {
        Establish context = () =>
        {
            beerEvent = db.BeerEvents.Insert(Name: "Drunkapalooza", Location: "Paddy Coynes", EventDate: DateTime.Today.AddDays(1));
        };

        Because of = () =>
        {
            ube = db.BeerEvents.FindAllByName("Drunkapalooza");
        };

        It should_have_a_result = () => 
            ube.Count().ShouldBeGreaterThan(0);

        // THIS FAILS !!!
        It should_allow_another_query = () =>
        {
            ube = db.BeerEvents.FindAllByName("Drunkapalooza");
            ube.Count().ShouldBeGreaterThan(0);
        };

        static object beerEvent;
        protected static IEnumerable<dynamic> ube;
    }
}