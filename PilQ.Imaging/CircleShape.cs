using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PilQ.Imaging
{
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
