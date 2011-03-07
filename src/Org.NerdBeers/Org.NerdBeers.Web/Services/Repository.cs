using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Simple.Data;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Web.Services
{
    public class Repository : IRepository
    {
        const int DefaultPageAmount = 10;

        public Nerd[] GetNerdsLike(string namepart, int amount = DefaultPageAmount)
        {
            string namelike = string.Format("*{0}*", namepart);
            var l = new List<Nerd>();
            foreach (var q in DB.Nerds.FindAll(DB.Users.Name.Like(namelike)))                
                l.Add((Nerd)q);
            return l.OrderBy(e => e.Name).Take(amount).ToArray();
        }

        public BeerEvent[] GetUpComingBeerEvents(int amount = DefaultPageAmount)
        {
            var l = new List<BeerEvent>();
            foreach (var q in DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1))))
                l.Add((BeerEvent)q);
            return l.OrderBy(e => e.EventDate).Take(amount).ToArray<BeerEvent>();
        }

        public Nerd[] GetBeerEventSubscribers(int id)
        {
            var l = new List<Nerd>();
            foreach (var subscr in DB.NerdSubscriptions.FindAllByEventId(id))
            {
                l.Add((Nerd)DB.Nerds.FindById(subscr.NerdId));
            }
            return l.ToArray();
        }

        public BeerEvent[] GetSubscribedEvents(string NerdGuid)
        {
            var l = new List<BeerEvent>();
            if (string.IsNullOrEmpty(NerdGuid)) return l.ToArray();
            var n = DB.Nerds.FindByGuid(NerdGuid);
            if (n == null) return l.ToArray();
            foreach (var subscr in DB.NerdSubscriptions.FindAllByNerdId(n.Id))
            {
                l.Add((BeerEvent)DB.BeerEvents.FindById(subscr.EventId));
            }
            return l.ToArray();
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