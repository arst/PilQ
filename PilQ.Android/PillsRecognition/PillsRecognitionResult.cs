namespace PilQ.PillsRecognition
{
    using Android.Graphics;

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