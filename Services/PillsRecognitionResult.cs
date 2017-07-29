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

namespace PilQ.Services
{
    public class PillsRecognitionResult
    {
        public PillsRecognitionResult(int count, Bitmap image)
        {
            this.Count = count;
            this.MarkedImage = image;
        }

        public int Count { get; private set; }
        public Bitmap MarkedImage { get; private set; }
    }
}