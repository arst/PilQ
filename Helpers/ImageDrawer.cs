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
using Android.Graphics;

namespace PilQ.Helpers
{
    public class ImageDrawer : IDisposable
    {
        private readonly Canvas underlayingCanvas;
        private readonly Bitmap underlayingImage;
        private readonly Paint defaultPaint;

        public ImageDrawer(Bitmap imageToDrawOn, Paint drawer)
        {
            this.underlayingImage = imageToDrawOn;
            this.underlayingCanvas = new Canvas(this.underlayingImage);
            this.defaultPaint = drawer;
        }

        public void DrawCircle(float x, float y, float radius)
        {
            this.DrawCircle(x, y, radius, this.defaultPaint);
        }

        public void DrawCircle(float x, float y, float radius, Paint paint)
        {
            underlayingCanvas.DrawCircle(x, y, radius, paint);
        }

        public void DrawPath(List<Point> points)
        {
            this.DrawPath(points, this.defaultPaint);
        }

        public void DrawPath(List<Point> points, Paint paint)
        {
            if (points != null && points.Any())
            {
                var startingPoint = points.FirstOrDefault();
                Path pathToDraw = new Path();
                pathToDraw.MoveTo(startingPoint.X, startingPoint.Y);

                foreach (var point in points.Skip(1))
                {
                    pathToDraw.LineTo(point.X,point.Y);
                }
                this.underlayingCanvas.DrawPath(pathToDraw, paint);
            }
        }

        public void Dispose()
        {
            var capturedCanvas = this.underlayingCanvas;

            if (capturedCanvas != null)
            {
                capturedCanvas.Dispose();
            }
        }
    }
}