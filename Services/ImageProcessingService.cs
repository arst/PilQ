using PilQ.Model;
using System.IO;
using Android.Graphics;
using Accord.Imaging.Filters;
using Accord.Imaging;
using Accord.Math.Geometry;
using System.Threading.Tasks;

namespace PilQ.Services
{
    public class ImageProcessingService
    {

        public async Task<RecognitionResult> RecognizeCircles(string filename)
        {
            var counter = 0;
            Bitmap mutableBitmap = null;
            var imageBytes = File.ReadAllBytes(filename);
            var resized = await ResizeImageAsync(imageBytes, 800, 600);

            using (var resizedImageOriginal = await BitmapFactory.DecodeByteArrayAsync(resized, 0, resized.Length))
            using (var resizedImage = (System.Drawing.Bitmap)resizedImageOriginal)
            using (System.Drawing.Bitmap b = Accord.Imaging.Image.Clone((System.Drawing.Bitmap)resizedImage, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            {
                var res = Grayscale.CommonAlgorithms.BT709.Apply(b);
                CannyEdgeDetector canny = new CannyEdgeDetector();
                canny.ApplyInPlace(res);
                RecursiveBlobCounter blobCounter = new RecursiveBlobCounter();
                blobCounter.FilterBlobs = true;
                blobCounter.MinHeight = 20;
                blobCounter.MinWidth = 20;
                blobCounter.ProcessImage(res);
                Blob[] blobs = blobCounter.GetObjectsInformation();
                SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
                resizedImageOriginal.PrepareToDraw();

                mutableBitmap = ((Bitmap)b).Copy(Bitmap.Config.Argb8888, true);

                using (Canvas canvas = new Canvas(mutableBitmap))
                {
                    Paint paint = new Paint();
                    paint.Color = Color.Red;
                    paint.AntiAlias = true;
                    paint.SetStyle(Paint.Style.Stroke);
                    paint.StrokeWidth = 5;

                    for (int i = 0, n = blobs.Length; i < n; i++)
                    {
                        var edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);
                        Accord.Point center;
                        float radius;
                        if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                        {
                            canvas.DrawCircle(
                              (int)(center.X),
                              (int)(center.Y),
                              radius, paint);
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
      
        private async Task<byte[]> ResizeImageAsync(byte[] imageData, float width, float height)
        {

            using (Bitmap originalImage = await BitmapFactory.DecodeByteArrayAsync(imageData, 0, imageData.Length))
            {
                float targetHeight = 0;
                float targetWidth = 0;
                var imageHeight = originalImage.Height;
                var imageWidth = originalImage.Width;

                if (imageHeight > imageWidth)
                {
                    targetHeight = height;
                    float factor = imageHeight / height;
                    targetWidth = imageWidth / factor;
                }
                else
                {
                    targetWidth = width;
                    float factor = imageWidth / width;
                    targetHeight = imageHeight / factor;
                }
                using (Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)targetWidth, (int)targetHeight, false))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                        resizedImage.Recycle();
                        originalImage.Recycle();
                        return ms.ToArray();
                    }
                }
                
            }
        }
    }
}