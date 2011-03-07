using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.NerdBeers.Web.Services
{
    public interface IRepository
    {
        dynamic DB { get; }
        dynamic[] GetNerdsLike(string namepart, int amount );
        dynamic[] GetUpComingBeerEvents(int amount );
        dynamic[] GetBeerEventSubscribers(int id);
        dynamic[] GetSubscribedEvents(string Guid);
    }
}
