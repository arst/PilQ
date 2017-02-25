using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Graphics;
using System.IO;

namespace PilQ.Helpers
{
    public static class ImageUtils
    {
        public static async Task<byte[]> ResizeImageAsync(byte[] imageData, float width, float height)
        {

            if (imageData == null || (imageData != null && imageData.Length == 0))
            {
                throw new ArgumentException("Image data can't be null or empty");
            }

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