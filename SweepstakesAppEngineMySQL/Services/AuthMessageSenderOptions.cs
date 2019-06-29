using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweepstakesAppEngineMySQL.Services
{
    public class AuthMessageSenderOptions
    {
        public string MailgunKey { get; set; }
        public string BaseURL { get; set; }
        public string DomainName { get; set; }
    }
}
