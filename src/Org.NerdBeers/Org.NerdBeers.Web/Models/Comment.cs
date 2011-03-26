using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Org.NerdBeers.Web.Models
{
    public class Comment
    {
        public Int64 Id { get; set; }
        public Int64 NerdId { get; set; }
        public Int64 EventId { get; set; }
        public string CommentText { get; set; }
        public DateTime Created {get;set;}
    }
}