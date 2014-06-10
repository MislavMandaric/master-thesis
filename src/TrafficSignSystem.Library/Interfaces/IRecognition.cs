using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    internal interface IRecognition : IDisposable
    {
        ClassesEnum Recognize(Parameters parameters);
    }
}
