using System;
using System.Collections.Generic;

namespace SignhostApiClientLibrary.Models
{
    public class Transaction
    {
        public Guid Id { get; internal set; }
        public int Status { get; internal set; }
        public File File { get; set; }
        public bool Seal { get; set; }
        public List<Signer> Signers { get; set; }
        public List<Receiver> Receivers { get; set; }
        public string Reference { get; set; }
        public string PostbackUrl { get; set; }
        public int SignRequestMode { get; set; }
        public int DaysToExpire { get; set; }
        public bool SendEmailNotifications { get; set; }
        public DateTime CreatedDateTime { get; internal set; }
        public DateTime ModifiedDateTime { get; internal set; }
        public DateTime? CanceledDateTime { get; internal set; }
    }
}
