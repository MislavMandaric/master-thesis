using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public interface IRecognition : ITrainable, IDisposable
    {
        string Recognize(Parameters parameters);
    }
}
