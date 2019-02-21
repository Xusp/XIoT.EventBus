using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLife;

namespace XIoT.EventBus
{
    public class EventBusException : XException
    {
        public EventBusException(string message) : base(message)
        {
        }
    }
}
