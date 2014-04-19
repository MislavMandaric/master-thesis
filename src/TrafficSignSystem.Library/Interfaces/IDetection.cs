using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficSignSystem.Library
{
    public interface IDetection
    {
        OpenCV.Net.Rect[] Detect(Parameters parameters);
    }
}
