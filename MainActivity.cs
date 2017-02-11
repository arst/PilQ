namespace PilQ
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.OS;
    using Android.Provider;
    using Android.Widget;
    using Java.IO;
    using Environment = Android.OS.Environment;
    using Uri = Android.Net.Uri;
    using Android.Views;
    using System.Threading.Tasks;
    using Components;
    using Services;
    using Android.Preferences;

    public static class App {
        public static File _file;
        public static File _dir;     
        public static Bitmap bitmap;
        public static ProgressDialog progressDialog;
    }

    [Activity(Label = "PilQ", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private ScaleImageView _scImageView;
        private ImageProcessingService imageProcessingService;

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Canceled)
            {
                App.progressDialog.Hide();
            }

            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
            var t = Task.Run(async () => {
                var calResult = await this.imageProcessingService.RecognizeCircles(App._file.Path);
                return calResult;
                
            });
            t.ContinueWith((cs) =>
            {
                var taskResult = cs.Result;
                RunOnUiThread(() =>
                {
                    var counterField = FindViewById<TextView>(Resource.Id.counter);
                    counterField.Text = taskResult.Count.ToString();
                    if (taskResult.Image != null)
                    {
                        _scImageView.SetImageBitmap(taskResult.Image);
                    }

                    var tmp = App._file;
                    App._file = null;
                    tmp.Delete();
                    App.progressDialog.Hide();
                });
            });
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.myButton);
                Button optionsBt = FindViewById<Button>(Resource.Id.optionsBtn);
                _scImageView = FindViewById<ScaleImageView>(Resource.Id.scImageView);
                imageProcessingService = new ImageProcessingService();
                button.Click += TakeAPicture;
                optionsBt.Click += OpenOptions;
                App.progressDialog = new ProgressDialog(this);
                App.progressDialog.Indeterminate = true;
                
                App.progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                App.progressDialog.SetMessage("Processing...Please wait...");
                App.progressDialog.SetCancelable(false);
               
            }

        }

        private void OpenOptions(object sender, EventArgs e)
        {
            StartActivity(typeof(SettingsActivity));
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "PilQ");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = 
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);

            App._file = new File(App._dir, "_latest.jpg");

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            App.progressDialog.Show();
            StartActivityForResult(intent, 0);
        }
    }
}
