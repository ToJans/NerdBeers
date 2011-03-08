using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.NerdBeers.Web.Models;

namespace Org.NerdBeers.Web.Services
{
    public interface IRepository
    {
        dynamic DB { get; }
        IEnumerable<Nerd> GetNerdsLike(string namepart, int amount );
        IEnumerable<BeerEvent> GetUpComingBeerEvents(int amount );
        IEnumerable<Nerd> GetBeerEventSubscribers(int id);
        IEnumerable<BeerEvent> GetSubscribedEvents(string Guid);
        IEnumerable<dynamic> GetBeerEventComments(int id);
    }
}
