using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace TrafficSignSystem.Library
{
    internal interface IDetection : IDisposable
    {
        CvSeq Detect(Parameters parameters);
    }
}
