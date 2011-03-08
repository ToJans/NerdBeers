using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Simple.Data;
using Org.NerdBeers.Web.Models;
using System.Dynamic;

namespace Org.NerdBeers.Web.Services
{
    public class Repository : IRepository
    {
        const int DefaultPageAmount = 10;

        public IEnumerable<Nerd> GetNerdsLike(string namepart, int amount = DefaultPageAmount)
        {
            string namelike = string.Format("*{0}*", namepart);
            IEnumerable<Nerd> l = DB.Nerds.FindAll(DB.Users.Name.Like(namelike)).Cast<Nerd>();
            return l.OrderBy(e => e.Name).Take(amount);
        }

        public IEnumerable<BeerEvent> GetUpComingBeerEvents(int amount = DefaultPageAmount)
        {
            IEnumerable<BeerEvent> l =
                DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1))).Cast<BeerEvent>();
            return l.OrderBy(e => e.EventDate).Take(amount);
        }

        public IEnumerable<Nerd> GetBeerEventSubscribers(int id)
        {
            // Use implicit joining from Simple.Data
            return DB.Nerds.FindAll(DB.Nerds.NerdSubscriptions.EventId == id).Cast<Nerd>(); // Cast<T> method is on SimpleResultSet
        }

        public IEnumerable<BeerEvent> GetSubscribedEvents(string NerdGuid)
        {
            if (string.IsNullOrEmpty(NerdGuid)) return Enumerable.Empty<BeerEvent>();

            // Use implicit joining from Simple.Data
            return
                DB.BeerEvents.FindAll(DB.BeerEvents.NerdSubscriptions.Nerds.Guid == NerdGuid).Cast<BeerEvent>();
        }

        public IEnumerable<dynamic> GetBeerEventComments(int id)
        {
            foreach (var ec in DB.Comments.FindAllByEventId(id))
            {
                dynamic d = new ExpandoObject();
                d.Comment = ec;
                d.Nerd = ec.Nerd; // Use implicit lazy-loading from Simple.Data
                yield return d;
            }
        }


        
        public dynamic DB
        {
            get
            {
                var c = System.Web.HttpContext.Current;
                var s = ConfigurationManager.ConnectionStrings["NerdBeers"];

                if (string.IsNullOrWhiteSpace(s.ConnectionString))
                {
                    return Simple.Data.Database.OpenFile(c.Server.MapPath("~/App_data/Nerdbeers.sdf"));
                }
                else
                {
                    return Simple.Data.Database.OpenConnection(s.ConnectionString);
                }
            }
        }
    }
}