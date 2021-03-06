namespace PilQ
{
    using Android.App;
    using Android.OS;
    using Android.Text;
    using Android.Widget;
    using PilQ.Helpers;
    using System;

    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Settings);
            var useAdditionalFiltersCheckBox = FindViewById<CheckBox>(Resource.Id.additionalFiltersCheckbox);
            var useColorFiltersCheckbox = FindViewById<CheckBox>(Resource.Id.colorFiltersCheckbox);
            var minCircleReadius = FindViewById<EditText>(Resource.Id.minCircleRadius);
            var thresholdValue = FindViewById<EditText>(Resource.Id.thresholdValue);
            useColorFiltersCheckbox.Checked = Settings.UseColorFiltersSettings;
            useAdditionalFiltersCheckBox.Checked = Settings.UseAdditionalFiltersSettings;
            minCircleReadius.Text = Settings.MinCircleRadiusSettings.ToString();
            thresholdValue.Text = Settings.Threshold.ToString();

            useAdditionalFiltersCheckBox.CheckedChange += additionalFiltersCheckbox_changed;
            useColorFiltersCheckbox.CheckedChange += useColorFiltersCheckbox_changed;
            minCircleReadius.TextChanged += minCircleReadius_changed;
            thresholdValue.TextChanged += thresholdValue_changed;
        }

        private void thresholdValue_changed(object sender, TextChangedEventArgs e)
        {
            int threshold;

            if (Int32.TryParse(e.Text.ToString(), out threshold))
            {
                Settings.Threshold = threshold;
            }
        }

        private void minCircleReadius_changed(object sender, TextChangedEventArgs e)
        {
            int minCircleRadius;

            if (Int32.TryParse(e.Text.ToString(), out minCircleRadius))
            {
                Settings.MinCircleRadiusSettings = minCircleRadius;
            }
        }

        private void useColorFiltersCheckbox_changed(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Settings.UseColorFiltersSettings = e.IsChecked;
        }

        private void additionalFiltersCheckbox_changed(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Settings.UseAdditionalFiltersSettings = e.IsChecked;

            if (Settings.UseAdditionalFiltersSettings)
            {
                var threshold = FindViewById<RelativeLayout>(Resource.Id.thresholdLayout);
                threshold.Visibility = Android.Views.ViewStates.Visible;
            }
            else {
                var threshold = FindViewById<RelativeLayout>(Resource.Id.thresholdLayout);
                threshold.Visibility = Android.Views.ViewStates.Invisible;
            }
        }
    }
}