using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace TrafficSignSystem.Library
{
    public static class Preprocess
    {
        public static IplImage ViolaJonesPreprocess(IplImage image)
        {
            IplImage preprocessedImage = new IplImage(image.Size, image.Depth, 1);
            image.CvtColor(preprocessedImage, ColorConversion.BgrToGray);
            preprocessedImage.EqualizeHist(preprocessedImage);
            return preprocessedImage;
        }

        public static IplImage RandomForestPreprocess(IplImage image, int width, int height)
        {
            using (IplImage smallImage = new IplImage(new CvSize(width, height), image.Depth, image.ElemChannels))
            {
                image.Resize(smallImage);
                IplImage preprocessedImage = new IplImage(image.Size, image.Depth, 1);
                smallImage.CvtColor(preprocessedImage, ColorConversion.BgrToGray);
                preprocessedImage.EqualizeHist(preprocessedImage);
                return preprocessedImage;
            }
        }
    }
}
