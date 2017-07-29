using Accord;
using Accord.Imaging;
using Accord.Imaging.Filters;
using Accord.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PilQ.Imaging
{
    public class Recognizer
    {
        public RecognitionResult RecognizeShapes(Bitmap image, RecognitionOptions options)
        {

            if (image == null)
            {
                throw new ArgumentNullException("Image data can't be null");
            }

            if (options == null)
            {
                throw new ArgumentNullException("Recognition options can't be null");
            }
            RecognitionResult result = null;
            var filters = this.AssembleFilters(options.UseAdditionalFilters, options.UseColorFilterts);
            var afterApplyFilterImageResult = filters.Apply(image);
            RecursiveBlobCounter blobCounter = new RecursiveBlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = options.MinimalSizeOfThePill;
            blobCounter.MinWidth = options.MinimalSizeOfThePill;
            blobCounter.ProcessImage(afterApplyFilterImageResult);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            List<CircleShape> recognizedCircles = new List<CircleShape>();

            foreach (var blob in blobs)
            {
                float radius;
                Accord.Point centerPoint;

                if (shapeChecker.IsCircle(blobCounter.GetBlobsEdgePoints(blob), out centerPoint, out radius))
                {
                    recognizedCircles.Add(new CircleShape(radius, (int)centerPoint.X, (int)centerPoint.Y));
                }
            }
            result = new RecognitionResult(recognizedCircles.Count, recognizedCircles.ToList());
            return result;
        }

        [SecurityCritical]
        private FiltersSequence AssembleFilters(bool useAdditionalFilters, bool useColorFilters)
        {
            FiltersSequence filters = new FiltersSequence();

            if (useColorFilters)
            {
                ColorFiltering colorFilter = new ColorFiltering();

                colorFilter.Red = new IntRange(0, 64);
                colorFilter.Green = new IntRange(0, 64);
                colorFilter.Blue = new IntRange(0, 64);
                colorFilter.FillOutsideRange = false;

                filters.Add(colorFilter);
            }

            if (useAdditionalFilters)
            {
                filters.Add(new GaussianSharpen(4, 11));
            }
            filters.Add(Grayscale.CommonAlgorithms.BT709);
            filters.Add(new CannyEdgeDetector());

            if (useAdditionalFilters)
            {
                Threshold ts = new Threshold(130); // TODO: pass it from settings
                filters.Add(ts);
            }

            return filters;
        }
    }
}
