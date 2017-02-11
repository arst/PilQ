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
using PilQ.Model;
using System.IO;
using Accord.Imaging.Filters;
using Accord;
using PilQ.Constants;
using System.Threading.Tasks;

namespace PilQ.Services
{
    public class PillsRecognitionService
    {
        private readonly ImageProcessingService imageProcessingService = new ImageProcessingService();

        public async Task<RecognitionResult> RecognizePillsAsync(string fileName, int minPillSize, bool useAdditionalFilters, bool useColorFilters)
        {
            var imageBytes = File.ReadAllBytes(fileName);
            var result = await this.imageProcessingService.RecognizeShapesAsync(imageBytes, minPillSize, this.AssembleFilters(useAdditionalFilters, useColorFilters), PillsTypeEnum.Round);


            return result;
        }

        private FiltersSequence AssembleFilters(bool useAdditionalFilters, bool useColorFilters)
        {
            FiltersSequence filters = new FiltersSequence();

            if (useColorFilters)
            {
                ColorFiltering colorFilter = new ColorFiltering();

                colorFilter.Red = new IntRange(0, 64);
                colorFilter.Green = new IntRange(0, 64);
                colorFilter.Blue = new IntRange(0, 64);
                colorFilter.FillOutsideRange = false;

                filters.Add(colorFilter);
            }

            if (useAdditionalFilters)
            {
                filters.Add(new GaussianSharpen(4, 11));
            }
            filters.Add(Grayscale.CommonAlgorithms.BT709);
            filters.Add(new CannyEdgeDetector());

            if (useAdditionalFilters)
            {
                Threshold ts = new Threshold(160);
                filters.Add(ts);
            }

            return filters;
        }
    }
}