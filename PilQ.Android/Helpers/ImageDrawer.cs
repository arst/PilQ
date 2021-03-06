namespace PilQ.Helpers
{
    using Android.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImageDrawer : IDisposable
    {
        private readonly Canvas underlayingCanvas;
        private readonly Bitmap underlayingImage;
        private readonly Paint defaultPaint;

        public ImageDrawer(Bitmap imageToDrawOn, Paint drawer)
        {

            if (imageToDrawOn == null)
            {
                throw new ArgumentException("Image to draw can't be null");
            }

            if(drawer == null)
            {
                throw new ArgumentException("Drawer can't be null");
            }

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
            if (paint == null)
            {
                throw new ArgumentException("Paint can't be null");
            }

            underlayingCanvas.DrawCircle(x, y, radius, paint);
        }

        public void DrawPath(List<Point> points)
        {
            if (points == null)
            {
                throw new ArgumentException("List of the points to draw can't be null");
            }

            this.DrawPath(points, this.defaultPaint);
        }

        public void DrawPath(List<Point> points, Paint paint)
        {
            if (paint == null)
            {
                throw new ArgumentException("Paint can't be null");
            }

            if (points == null)
            {
                throw new ArgumentException("List of the points to draw can't be null");
            }

            if (points.Any())
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var capturedCanvas = this.underlayingCanvas;

                if (capturedCanvas != null)
                {
                    capturedCanvas.Dispose();
                }
            }
        }
    }
}