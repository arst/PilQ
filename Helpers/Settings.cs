// Helpers/Settings.cs This file was automatically added when you installed the Settings Plugin. If you are not using a PCL then comment this file back in to use it.
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PilQ.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
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
        private static readonly int MinCircleRadiusDefault = 20;
            #endregion


        public static bool UseAdditionalFiltersSettings
        {
            get
            {
            return AppSettings.GetValueOrDefault<bool>(UseAdditionalFilters, UseAdditionalFiltersDefault);
            }
            set
            {
            AppSettings.AddOrUpdateValue<bool>(UseAdditionalFilters, value);
            }
        }

        public static bool UseColorFiltersSettings
        {
                get
                {
                    return AppSettings.GetValueOrDefault<bool>(UseColorFilters, UseColorFiltersDefault);
                }
                set
                {
                    AppSettings.AddOrUpdateValue<bool>(UseColorFilters, value);
                }
            }

        public static int MinCircleRadiusSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>(MinimumCircleRadius, MinCircleRadiusDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<int>(MinimumCircleRadius, value);
            }
        }
    }   
}