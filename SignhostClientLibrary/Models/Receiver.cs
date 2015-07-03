using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignhostApiClientLibrary.Models
{
    public class Receiver
    {
        public Guid Id { get; internal set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Language { get; set; }
        public string Message { get; set; }
        public string Reference { get; set; }
        public List<Activity> Activities { get; internal set; }
        public DateTime CreatedDateTime { get; internal set; }
        public DateTime ModifiedDateTime { get; internal set; }
    }
}
