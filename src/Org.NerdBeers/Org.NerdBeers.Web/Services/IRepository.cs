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
        Nerd[] GetNerdsLike(string namepart, int amount );
        BeerEvent[] GetUpComingBeerEvents(int amount );
        Nerd[] GetBeerEventSubscribers(int id);
        BeerEvent[] GetSubscribedEvents(string Guid);
        dynamic[] GetBeerEventComments(int id);
    }
}
