using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace SignhostApiClientLibrary.Models
{
    public class Activity
    {
        public Guid Id { get; internal set; }
        public int Code { get; internal set; }
        [DeserializeAs(Name = "Activity")]
        public string Description { get; internal set; }
        public DateTime CreatedDateTime { get; internal set; }
    }
}
