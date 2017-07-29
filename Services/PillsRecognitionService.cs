using Android.Graphics;
using PilQ.Helpers;
using PilQ.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PilQ.Services
{
    public class PillsRecognitionService
    {
        private readonly Recognizer recognizer = new Recognizer();

        public async Task<PillsRecognitionResult> RecognizePillsAsync(string fileName, int minPillSize, bool useAdditionalFilters, bool useColorFilters)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name to process can't be null or empty");
            }
            RecognitionResult shapeRecognitionResult = null;
            Bitmap mutableAndoidBitmap = null;
            var imageBytes = File.ReadAllBytes(fileName);

            using (var androidBitmap = await ImageUtils.ResizeImageAsync(imageBytes, DefaultProcessingImageSize.Width, DefaultProcessingImageSize.Height))
            {
                mutableAndoidBitmap = androidBitmap.Copy(Bitmap.Config.Argb8888, true);
                var bitmap = (System.Drawing.Bitmap)androidBitmap;
                RecognitionOptions options = new RecognitionOptions(minPillSize, useAdditionalFilters, useColorFilters, new ShapeType[] { ShapeType.Circle });
                shapeRecognitionResult = this.recognizer.RecognizeShapes(bitmap.Clone(System.Drawing.Imaging.PixelFormat.Format24bppRgb), options);
            }

            if (mutableAndoidBitmap != null)
            {
                Paint paint = new Paint();
                paint.Color = Color.Red;
                paint.AntiAlias = true;
                paint.SetStyle(Paint.Style.Stroke);
                paint.StrokeWidth = 5;
                ImageDrawer d = new ImageDrawer(mutableAndoidBitmap, paint);
                foreach (var circle in shapeRecognitionResult.CircleShapes)
                {
                    d.DrawCircle(circle.Center.X, circle.Center.Y, circle.Radius);
                }
            }
            

            return new PillsRecognitionResult(shapeRecognitionResult.Count, mutableAndoidBitmap);
        }
    }
}