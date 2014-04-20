using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public interface IRecognition
    {
        string Recognize(Parameters parameters);
    }
}
