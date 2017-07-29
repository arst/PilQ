namespace PilQ.Imaging
{
    using System.Collections.Generic;

    public class RecognitionResult
    {
        public RecognitionResult(int count, List<CircleShape> circles)
        {
            this.Count = count;
            this.CircleShapes = circles;
        }

        public int Count { get; private set; }
        public List<CircleShape> CircleShapes { get; private set; }
    }
}
