namespace PilQ.Imaging
{
    using System.Drawing;

    public class CircleShape
    {
        public CircleShape(float radius, int x, int y)
        {
            this.Radius = radius;
            this.Center = new Point(x, y);
        }

        public float Radius { get; private set; }
        public Point Center { get; private set; }

    }
}
