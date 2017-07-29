namespace PilQ.Helpers
{
    using Plugin.Settings;
    using Plugin.Settings.Abstractions;
    using System.Security;

    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    [SecurityCritical]
  public static class Settings
  {
        private static ISettings AppSettings
        {
            get
            {
            return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string UseAdditionalFilters = "use_additional_filters";
        private static readonly bool UseAdditionalFiltersDefault = false;

        private const string UseColorFilters = "use_color_filters";
        private static readonly bool UseColorFiltersDefault = false;

        private const string MinimumCircleRadius = "min_circle_radius";
        private static readonly int MinCircleRadiusDefault = 10;

        private const string ThresholdFilter = "threshold_filter";
        private const int ThresholdFilterDefault = 130;
        #endregion


        public static bool UseAdditionalFiltersSettings
        {
            get
            {
            return AppSettings.GetValueOrDefault(UseAdditionalFilters, UseAdditionalFiltersDefault);
            }
            set
            {
            AppSettings.AddOrUpdateValue(UseAdditionalFilters, value);
            }
        }

        public static bool UseColorFiltersSettings
        {
                get
                {
                    return AppSettings.GetValueOrDefault(UseColorFilters, UseColorFiltersDefault);
                }
                set
                {
                    AppSettings.AddOrUpdateValue(UseColorFilters, value);
                }
            }

        public static int MinCircleRadiusSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(MinimumCircleRadius, MinCircleRadiusDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(MinimumCircleRadius, value);
            }
        }

        public static int Threshold
        {
            get
            {
                return AppSettings.GetValueOrDefault(ThresholdFilter, ThresholdFilterDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(ThresholdFilter, value);
            }
        }
    }   
}