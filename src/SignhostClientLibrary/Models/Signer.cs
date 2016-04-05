using System;
using System.Collections.Generic;

namespace SignhostApiClientLibrary.Models
{
    public class Signer
    {
        public Guid Id { get; internal set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Iban { get; set; }
        public bool RequireScribble { get; set; }
        public bool RequireEmailVerification { get; set; }
        public bool RequireSmsVerification { get; set; }
        public bool RequireIdealVerification { get; set; }
        public bool SendSignRequest { get; set; }
        public bool? SendSignConfirmation { get; set; }
        public string SignRequestMessage { get; set; }
        public int DaysToRemind { get; set; }
        public string Language { get; set; }
        public string ScribbleName { get; set; }
        public bool ScribbleNameFixed { get; set; }
        public string Reference { get; set; }
        public string ReturnUrl { get; set; }
        public List<Activity> Activities { get; internal set; }
        public string RejectReason { get; internal set; }
        public string SignUrl { get; internal set; }
        public DateTime? SignedDateTime { get; internal set; }
        public DateTime? RejectDateTime { get; internal set; }
        public DateTime CreatedDateTime { get; internal set; }
        public DateTime ModifiedDateTime { get; internal set; }
    }
}
