namespace PilQ.Imaging
{
    using Accord;
    using Accord.Imaging;
    using Accord.Imaging.Filters;
    using Accord.Math.Geometry;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Security;

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
            var filters = this.AssembleFilters(options);
            var afterApplyFilterImageResult = filters.Apply(image);
            RecursiveBlobCounter blobCounter = new RecursiveBlobCounter();
            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = options.MinimalSizeOfThePill;
            blobCounter.MinWidth = options.MinimalSizeOfThePill;
            blobCounter.ProcessImage(afterApplyFilterImageResult);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            List<CircleShape> recognizedCircles = new List<CircleShape>();
            List<QuadrilateralShape> recognizeQuadrilaterals = new List<QuadrilateralShape>();

            foreach (var blob in blobs)
            {
                float radius;
                Accord.Point centerPoint;

                if (options.ShapeTypes.Contains(ShapeType.Circle) && shapeChecker.IsCircle(blobCounter.GetBlobsEdgePoints(blob), out centerPoint, out radius))
                {
                    recognizedCircles.Add(new CircleShape(radius, (int)centerPoint.X, (int)centerPoint.Y));
                }
                else if (options.ShapeTypes.Contains(ShapeType.Rectangle) &&  shapeChecker.IsQuadrilateral(blobCounter.GetBlobsEdgePoints(blob)))
                {
                    var shapeEdgePoints = blobCounter
                                            .GetBlobsEdgePoints(blob)
                                            .Select(ep => new System.Drawing.Point(ep.X, ep.Y))
                                            .ToList();
                    recognizeQuadrilaterals.Add(new QuadrilateralShape(shapeEdgePoints));
                }
            }
            result = new RecognitionResult(recognizedCircles.Count + recognizeQuadrilaterals.Count, recognizedCircles, recognizeQuadrilaterals);
            return result;
        }

        [SecurityCritical]
        private FiltersSequence AssembleFilters(RecognitionOptions options)
        {
            FiltersSequence filters = new FiltersSequence();

            if (options.UseColorFilterts)
            {
                ColorFiltering colorFilter = new ColorFiltering();

                colorFilter.Red = new IntRange(0, 64);
                colorFilter.Green = new IntRange(0, 64);
                colorFilter.Blue = new IntRange(0, 64);
                colorFilter.FillOutsideRange = false;

                filters.Add(colorFilter);
            }

            if (options.UseAdditionalFilters)
            {
                filters.Add(new GaussianSharpen(4, 11));
            }
            filters.Add(Grayscale.CommonAlgorithms.BT709);
            filters.Add(new CannyEdgeDetector());

            if (options.UseAdditionalFilters)
            {
                Threshold ts = new Threshold(options.Threshold);
                filters.Add(ts);
            }

            return filters;
        }
    }
}
