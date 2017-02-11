using PilQ.Model;
using System.IO;
using Android.Graphics;
using Accord.Imaging.Filters;
using Accord.Imaging;
using Accord.Math.Geometry;
using System.Threading.Tasks;
using PilQ.Helpers;
using PilQ.Constants;
using System;
using System.Linq;
using System.Collections.Generic;
using Accord;

namespace PilQ.Services
{
    public class ImageProcessingService
    {

        public async Task<RecognitionResult> RecognizeShapesAsync(byte[] imageBytes, int minSize, FiltersSequence filters, PillsTypeEnum shapeType)
        {
            var counter = 0;
            Bitmap mutableBitmap = null;
            var resized = await ImageUtils.ResizeImageAsync(imageBytes, DefaultProcessingImageSize.Width, DefaultProcessingImageSize.Height);

            using (var resizedImageOriginal = await BitmapFactory.DecodeByteArrayAsync(resized, 0, resized.Length))
            using (var resizedImage = (System.Drawing.Bitmap)resizedImageOriginal)
            using (System.Drawing.Bitmap processingImage = Accord.Imaging.Image.Clone((System.Drawing.Bitmap)resizedImage, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                var afterApplyFilterImageResult = filters.Apply(processingImage);
                RecursiveBlobCounter blobCounter = new RecursiveBlobCounter();
                blobCounter.FilterBlobs = true;
                blobCounter.MinHeight = minSize;
                blobCounter.MinWidth = minSize;
                blobCounter.ProcessImage(afterApplyFilterImageResult);
                Blob[] blobs = blobCounter.GetObjectsInformation();
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                resizedImageOriginal.PrepareToDraw();

                mutableBitmap = ((Bitmap)processingImage).Copy(Bitmap.Config.Argb8888, true);

                Paint paint = new Paint();
                paint.Color = Color.Red;
                paint.AntiAlias = true;
                paint.SetStyle(Paint.Style.Stroke);
                paint.StrokeWidth = 5;

                using (ImageDrawer drawer = new ImageDrawer(mutableBitmap, paint))
                {
                    for (int i = 0, n = blobs.Length; i < n; i++)
                    {
                        var edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                        Accord.Point center;
                        float radius;

                        if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                        {
                            drawer.DrawCircle(center.X, center.Y, radius);
                            counter++;
                        }
                    }
                }
            }

            RecognitionResult result = new RecognitionResult();
            result.Count = counter;
            result.Image = mutableBitmap;

            return result;
        }
    }
}