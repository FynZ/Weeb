using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TonsOfDamage.Services
{
    public class AuthMessageSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }

        public AuthMessageSenderOptions()
        {
            SendGridUser = "Fynzie";
            SendGridKey = "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";
            //SendGridKey = "lqnZ3DQlQxmZXgydJPeeoA";
        }
    }
}
