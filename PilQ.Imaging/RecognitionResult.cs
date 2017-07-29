using Accord;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PilQ.Imaging
{
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
