namespace PilQ.Helpers
{
    using Android.Graphics;
    using System;
    using System.Threading.Tasks;

    public static class ImageUtils
    {
        public static async Task<Bitmap> ResizeImageAsync(byte[] imageData, float width, float height)
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
                Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)targetWidth, (int)targetHeight, false);
                originalImage.Recycle();
                return resizedImage;
            }
        }
    }
}