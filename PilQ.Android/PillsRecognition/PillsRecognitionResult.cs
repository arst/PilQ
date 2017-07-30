namespace PilQ.PillsRecognition
{
    using Android.Graphics;

    public class PillsRecognitionResult
    {
        public PillsRecognitionResult(int roundPillsCount, int quadrilateralPillsCount, Bitmap image)
        {
            this.RoundPillsCount = roundPillsCount;
            this.QuadrilateralPillsCount = quadrilateralPillsCount;
            this.MarkedImage = image;
        }

        public int RoundPillsCount { get; private set; }
        public int QuadrilateralPillsCount{ get; private set; }
        public Bitmap MarkedImage { get; private set; }
    }
}