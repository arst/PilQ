namespace PilQ.Imaging
{
    public class RecognitionOptions
    {
        public RecognitionOptions(int minSizeOfThePills, bool useAdditionalFilters, bool useColorFilters, ShapeType[] shapeTypesToRecognize, int threshold)
        {
            this.MinimalSizeOfThePill = minSizeOfThePills;
            this.ShapeTypes = shapeTypesToRecognize;
            this.UseAdditionalFilters = useAdditionalFilters;
            this.UseColorFilterts = useColorFilters;
            this.Threshold = threshold;
        }

        public int MinimalSizeOfThePill { get; private set; }
        public bool UseAdditionalFilters { get; private set; }
        public int Threshold { get; set; }
        public bool UseColorFilterts { get; private set; }
        public ShapeType[] ShapeTypes { get; private set; } 
    }
}
