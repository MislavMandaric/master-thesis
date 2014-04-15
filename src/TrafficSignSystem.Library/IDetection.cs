using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public interface IDetection : ITrainable
    {
        OpenCV.Net.Seq Detect(Parameters parameters);
    }
}
