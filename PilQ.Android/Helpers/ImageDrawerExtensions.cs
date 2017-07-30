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
using PilQ.Imaging;
using Android.Graphics;

namespace PilQ.Helpers
{
    public static class ImageDrawerExtensions
    {
        public static void DrawPills(this ImageDrawer drawer, List<CircleShape> circleShapes, List<QuadrilateralShape> quadriletarShapes)
        {
            foreach (var circle in circleShapes)
            {
                drawer.DrawCircle(circle.Center.X, circle.Center.Y, circle.Radius);
            }
            foreach (var quadrilateral in quadriletarShapes)
            {
                drawer.DrawPath(quadrilateral.EdgePoints.Select(p => new Point(p.X, p.Y)).ToList());
            }
        }
    }
}