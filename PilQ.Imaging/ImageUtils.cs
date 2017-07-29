//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PilQ.Imaging
//{
//    public static class ImageUtils
//    {
//        public static async Task<byte[]> ResizeImageAsync(byte[] imageData, float width, float height)
//        {
//            if (imageData == null || (imageData != null && imageData.Length == 0))
//            {
//                throw new ArgumentException("Image data can't be null or empty");
//            }

//            var image = Bitmap.FromStream(new MemoryStream(imageData));
//            using (Bitmap originalImage = new Bitmap(image))
//            {
//                float targetHeight = 0;
//                float targetWidth = 0;
//                var imageHeight = originalImage.Height;
//                var imageWidth = originalImage.Width;

//                if (imageHeight > imageWidth)
//                {
//                    targetHeight = height;
//                    float factor = imageHeight / height;
//                    targetWidth = imageWidth / factor;
//                }
//                else
//                {
//                    targetWidth = width;
//                    float factor = imageWidth / width;
//                    targetHeight = imageHeight / factor;
//                }
//                using (Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)targetWidth, (int)targetHeight, false))
//                {
//                    using (MemoryStream ms = new MemoryStream())
//                    {
//                        resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
//                        resizedImage.Recycle();
//                        originalImage.Recycle();
//                        return ms.ToArray();
//                    }
//                }

//            }
//        }
//    }
//}
