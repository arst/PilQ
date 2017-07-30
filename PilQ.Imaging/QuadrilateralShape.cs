using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace PilQ.Imaging
{
    public class QuadrilateralShape
    {
        public QuadrilateralShape(IList<Point> points)
        {
            this.EdgePoints = new ReadOnlyCollection<Point>(points);
        }
        public ReadOnlyCollection<Point> EdgePoints { get; private set; }
    }
}