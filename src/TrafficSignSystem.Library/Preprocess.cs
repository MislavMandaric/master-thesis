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
        public static CvMat ViolaJonesPreprocess(CvMat image)
        {
            CvMat preprocessedImage = new CvMat(image.Rows, image.Cols, image.ElemType);
            preprocessedImage.Reshape(preprocessedImage, 1);
            image.CvtColor(preprocessedImage, ColorConversion.BgrToGray);
            preprocessedImage.EqualizeHist(preprocessedImage);
            return preprocessedImage;
        }

        public static CvMat RandomForestPreprocess(CvMat image, int rows, int cols)
        {
            using (CvMat smallImage = new CvMat(rows, cols, image.ElemType))
            {
                image.Resize(smallImage);
                CvMat preprocessedImage = new CvMat(smallImage.Rows, smallImage.Cols, smallImage.ElemType);
                preprocessedImage.Reshape(preprocessedImage, 1);
                smallImage.CvtColor(preprocessedImage, ColorConversion.BgrToGray);
                preprocessedImage.EqualizeHist(preprocessedImage);
                return preprocessedImage;
            }
        }
    }
}
