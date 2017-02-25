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
using System.Security;

namespace PilQ.Services
{
    public class PillsRecognitionService
    {
        private readonly ImageProcessingService imageProcessingService = new ImageProcessingService();

        public async Task<RecognitionResult> RecognizePillsAsync(string fileName, int minPillSize, bool useAdditionalFilters, bool useColorFilters)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name to process can't be null or empty");
            }
            var imageBytes = File.ReadAllBytes(fileName);
            var result = await this.imageProcessingService.RecognizeShapesAsync(imageBytes, minPillSize, this.AssembleFilters(useAdditionalFilters, useColorFilters), PillsTypeEnum.Round);


            return result;
        }

        [SecurityCritical]
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
                Threshold ts = new Threshold(PilQ.Helpers.Settings.Threshold);
                filters.Add(ts);
            }

            return filters;
        }
    }
}