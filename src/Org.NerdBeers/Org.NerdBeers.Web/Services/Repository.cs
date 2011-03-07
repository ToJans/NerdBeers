using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Simple.Data;

namespace Org.NerdBeers.Web.Services
{
    public class Repository : IRepository
    {
        const int DefaultPageAmount = 10;

        public dynamic[] GetNerdsLike(string namepart, int amount = DefaultPageAmount)
        {
            string namelike = string.Format("*{0}*", namepart);
            IEnumerable<dynamic> model = DB.Nerds.FindAll(DB.Users.Name.Like(namelike));
            return model.OrderBy(e => e.Name).Take(amount).ToArray();
        }

        public dynamic[] GetUpComingBeerEvents(int amount = DefaultPageAmount)
        {
            IEnumerable<dynamic> model = DB.BeerEvents.FindAllByEventDate(DateTime.Now.to(DateTime.Now.AddYears(1)));
            return model.OrderBy(e => e.EventDate).Take(amount).ToArray();
        }

        public dynamic[] GetBeerEventSubscribers(int id)
        {
            var l = new List<dynamic>();
            foreach (var subscr in DB.NerdSubscriptions.FindAllByEventId(id))
            {
                l.Add(DB.Nerds.FindById(subscr.NerdId));
            }
            return l.ToArray();
        }

        public dynamic[] GetSubscribedEvents(string NerdGuid)
        {
            var l = new List<dynamic>();
            if (string.IsNullOrEmpty(NerdGuid)) return l.ToArray();
            var n = DB.Nerds.FindByGuid(NerdGuid);
            if (n == null) return l.ToArray();
            foreach (var subscr in DB.NerdSubscriptions.FindAllByNerdId(n.Id))
            {
                l.Add(DB.BeerEvents.FindById(subscr.EventId));
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