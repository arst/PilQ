namespace PilQ.Imaging
{
    using System.Collections.Generic;

    public class RecognitionResult
    {
        public RecognitionResult(int count, List<CircleShape> circles, List<QuadrilateralShape> quadrilaterals)
        {
            this.Count = count;
            this.CircleShapes = circles;
            this.Quadrilaterals = quadrilaterals;
        }

        public int Count { get; private set; }
        public List<CircleShape> CircleShapes { get; private set; }
        public List<QuadrilateralShape> Quadrilaterals { get; private set; }
    }
}
