using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public class TrafficSignException : ApplicationException
    {
        public TrafficSignException() : base() { }
        public TrafficSignException(string message) : base(message) { }
        public TrafficSignException(string message, Exception exception) : base(message, exception) { }
    }
}
