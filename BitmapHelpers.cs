namespace PilQ
{
    using System;
    using System.Linq;
    using System.IO;

    using Android.Graphics;
    using Android.Media;
    //using AForge.Imaging;
    using Android.Content.Res;
    using AForge.Imaging.Filters;
    using AForge;
    using System.Collections.Generic;
    using Accord.Imaging;
    using Accord.Math.Geometry;
    using Model;

    public static class BitmapHelpers
    {
        //private static System.Drawing.Bitmap grayed = null;
        //private static System.Drawing.Bitmap prepared = null;

        public static RecognitionResult Reccy(this string filename)
        {
            var imageBytes = File.ReadAllBytes(filename);
            var resized = ResizeImageAndroid(imageBytes, 800, 600);
            var bo = (System.Drawing.Bitmap)BitmapFactory.DecodeByteArray(resized, 0, resized.Length);
            var b = AForge.Imaging.Image.Clone((System.Drawing.Bitmap)bo, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // lock image
            //var bm = Bitmap.FromFile(@"C:\\Users\arst\Desktop\test.jpg");
            //Bitmap b = new Bitmap(@"C:\\Users\arst\Desktop\test33.jpg");
            /*GaussianSharpen filter = new GaussianSharpen(4, 11);
            filter.ApplyInPlace(b);*/
            //b.Save(@"C:\\Users\arst\Desktop\test1.jpg");
            GrayscaleBT709 gs = new GrayscaleBT709();
            var res = gs.Apply(b);
            //res.Save(@"C:\\Users\arst\Desktop\test2.jpg");
            CannyEdgeDetector canny = new CannyEdgeDetector();
            canny.ApplyInPlace(res);
            //res.Save(@"C:\\Users\arst\Desktop\test3.jpg");
            /*Threshold ts = new Threshold(160);
            ts.ApplyInPlace(res);*/
            //res.Save(@"C:\\Users\arst\Desktop\test4.jpg");
            // step 2 - locating objects
            RecursiveBlobCounter blobCounter = new RecursiveBlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 10;
            blobCounter.MinWidth = 10;

            blobCounter.ProcessImage(res);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            //bitmap.UnlockBits( bitmapData );

            // step 3 - check objects' type and highlight
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
            //shapeChecker.RelativeDistortionLimit = 0.13f;

            Bitmap mutableBitmap = ((Bitmap)b).Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(mutableBitmap);
            Paint paint = new Paint();
            paint.Color = Color.Red;
            paint.AntiAlias = true;
            paint.SetStyle(Paint.Style.Stroke);
            int counter = 0;

            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                var edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                Accord.Point center;
                float radius;

                // is circle ?
                if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                {
                    canvas.DrawCircle(
                      (int)(center.X),
                      (int)(center.Y),
                      radius, paint);
                    counter++;
                }
            }

            RecognitionResult result = new RecognitionResult();
            result.Count = counter;
            result.Image = mutableBitmap;

            return result;
            
        }


        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height, Resources res, string intensity, int circleradius = 20)
        {
            
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InScaled = true;
            Bitmap bm = BitmapFactory.DecodeFile(fileName);
            
            byte[] bitmapData;
            using (var stream = new MemoryStream())
            {
                bm.Compress(Bitmap.CompressFormat.Png, 100, stream);
                bitmapData = stream.ToArray();
            }
            var resized = ResizeImageAndroid(bitmapData, 800, 600);
            var bmm = BitmapFactory.DecodeByteArray(resized, 0, resized.Length);
            var managed = AForge.Imaging.Image.Clone((System.Drawing.Bitmap)bmm, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            /*
            var data = new UnmanagedImage(bmm.LockPixels(),bmm.Width,
            bmm.Height, bmm.Width, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            //Bitmap bm = BitmapFactory.DecodeResource(res, Resource.Drawable.test, options);
            //System.Drawing.Bitmap bit = Accord.Imaging.Image.Clone((System.Drawing.Bitmap)bm, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);
            //Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            //var grayed = AForge.Imaging.Image.Clone(bit, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //var grayed = AForge.Imaging.Image.Clone(bit, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //FiltersSequence seq = new FiltersSequence();
            //seq.Add(Grayscale.CommonAlgorithms.Y);  //First add  grayScaling filter
            //seq.Add(new Threshold(176));
            //seq.Add(new OtsuThreshold()); //Then add binarization(thresholding) filter
            var managed = data.ToManagedImage(true);
            data.Dispose();*/
            //bm.Recycle();
            //var greyed = filter.Apply(managed);
            FiltersSequence seq = new FiltersSequence();
            seq.Add(Grayscale.CommonAlgorithms.BT709);  //First add  grayScaling filter
            seq.Add(new Threshold(176));
            var prepared = seq.Apply(managed);
            //Threshold ts = new Threshold(176);
            //var prepared = ts.Apply(greyed);

            //var prepared = seq.Apply(grayed); // Apply filters on source image
           
            //temp.Save(@"c:\users\arst\desktop\test2222.jpg");
            HoughCircleTransformation circleTransform = new HoughCircleTransformation(circleradius);
            //circleTransform.LocalPeakRadius = 20;
            circleTransform.MinCircleIntensity = Int16.Parse(intensity);//112;

            // apply Hough circle transform
            circleTransform.ProcessImage(prepared);
            //Bitmap houghCirlceImage = (Bitmap)circleTransform.ToBitmap();
            //houghCirlceImage.Save(@"c:\users\arst\desktop\test5555.jpg");
            // get circles using relative intensity
            HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity(0.1);
            //Graphics g = Graphics.FromImage(image);
            //Pen redPen = new Pen(Color.Red, 2);
            List<Tuple<int, int>> alreadyAdded = new List<Tuple<int, int>>();
            int counter = 0;
            Bitmap mutableBitmap = bmm.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(mutableBitmap);
            Paint paint = new Paint();
            paint.Color = Color.Red;
            paint.AntiAlias = true;
            foreach (HoughCircle circle in circles)
            {
                Tuple<int, int> coord = new Tuple<int, int>(circle.X, circle.Y);
                if (alreadyAdded.FirstOrDefault(c => (c.Item1 <= coord.Item1 + 10
                                                    && c.Item1 >= coord.Item1 - 10) &&
                                                     (c.Item2 <= coord.Item2 + 10
                                                    && c.Item2 >= coord.Item2 - 10)) == null)
                {
                    alreadyAdded.Add(coord);
                    counter++;
                    canvas.DrawCircle(
                      (int)(circle.X - circle.Radius),
                      (int)(circle.Y - circle.Radius),
                      circle.Radius, paint);
                }

            }
            Console.WriteLine(counter);
            //image.Save(@"c:\users\arst\desktop\test3333.jpg");
            //return;

            return mutableBitmap;
            //return mutableBitmap;
        }

        public static byte[] ResizeImageAndroid(byte[] imageData, float width, float height)
        {
            // Load the bitmap 
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            //
            float ZielHoehe = 0;
            float ZielBreite = 0;
            //
            var Hoehe = originalImage.Height;
            var Breite = originalImage.Width;
            //
            if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
            {
                ZielHoehe = height;
                float teiler = Hoehe / height;
                ZielBreite = Breite / teiler;
            }
            else // Breite (61 für Avatar) ist Master
            {
                ZielBreite = width;
                float teiler = Breite / width;
                ZielHoehe = Hoehe / teiler;
            }
            //
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)ZielBreite, (int)ZielHoehe, false);
            // 
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }
        /*
        private static Bitmap ProcessImage(Bitmap input)
        {
            BlobCounter blobCounter = new BlobCounter();
            System.Drawing.Bitmap bit = (System.Drawing.Bitmap)input;
            var preparedtogray = AForge.Imaging.Image.Clone(bit, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            FiltersSequence seq = new FiltersSequence();
            seq.Add(Grayscale.CommonAlgorithms.Y);
            seq.Add(new Threshold(176));
            var temp = seq.Apply(preparedtogray);
            
            HoughCircleTransformation circleTransform = new HoughCircleTransformation(300);
            circleTransform.LocalPeakRadius = 120;
            circleTransform.MinCircleIntensity = 400;

            // apply Hough circle transform
            circleTransform.ProcessImage(temp);
            // get circles using relative intensity
            HoughCircle[] circles = circleTransform.GetCirclesByRelativeIntensity(0.1);
            Paint paint = new Paint();
            paint.Color = Color.Red;
            paint.AntiAlias = true;
            Bitmap houghCirlceImage =(Bitmap) circleTransform.ToBitmap();
            /*
            Bitmap mutableBitmap = input.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(mutableBitmap);
            List<Tuple<int, int>> alreadyAdded = new List<Tuple<int, int>>();
            int counter = 0;
            foreach (HoughCircle circle in circles)
            {
                Tuple<int, int> coord = new Tuple<int, int>(circle.X, circle.Y);
                if (alreadyAdded.FirstOrDefault(c => (c.Item1 <= coord.Item1 + 10
                                                    && c.Item1 >= coord.Item1 - 10) &&
                                                     (c.Item2 <= coord.Item2 + 10
                                                    && c.Item2 >= coord.Item2 - 10)) == null)
                {
                    alreadyAdded.Add(coord);
                    counter++;
                    canvas.DrawCircle(
                       (int)(circle.X - circle.Radius),
                       (int)(circle.Y - circle.Radius),
                       circle.Radius, paint);
                }

            }
            
            return houghCirlceImage;
        }*/
    }
}
