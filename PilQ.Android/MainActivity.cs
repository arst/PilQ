namespace PilQ
{
    using Android.App;
    using Android.Content;
    using Android.Content.PM;
    using Android.Graphics;
    using Android.OS;
    using Android.Provider;
    using Android.Views;
    using Android.Widget;
    using com.refractored.fab;
    using Components;
    using ImageViews.Photo;
    using Java.IO;
    using PilQ.PillsRecognition;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Environment = Android.OS.Environment;
    using Uri = Android.Net.Uri;

    public static class ApplicationStateHolder {
        public static File _file;
        public static File _dir;     
        public static Bitmap bitmap;
        public static ProgressDialog progressDialog;
        public static MainActivity mainActivity;
        public static Task runningTask; 
    }

    [Activity(Label = "PilQ", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private PhotoView _scImageView;
        private PillsRecognitionService pillsRecognitionService;


        #region  Activity Lifecycle
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            PilQ.ApplicationStateHolder.mainActivity = this;
            

            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
                FloatingActionButton floatingButton = FindViewById<FloatingActionButton>(Resource.Id.fab);
                _scImageView = FindViewById<PhotoView>(Resource.Id.scImageView);

                pillsRecognitionService = new PillsRecognitionService();
                floatingButton.Click += TakeAPicture;
                PilQ.ApplicationStateHolder.progressDialog = new ProgressDialog(this);
                PilQ.ApplicationStateHolder.progressDialog.Indeterminate = true;
                PilQ.ApplicationStateHolder.progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                PilQ.ApplicationStateHolder.progressDialog.SetMessage("Processing...Please wait...");
                PilQ.ApplicationStateHolder.progressDialog.SetCancelable(false);
                if (PilQ.ApplicationStateHolder.runningTask != null)
                {
                    PilQ.ApplicationStateHolder.runningTask.ContinueWith((t) => {
                        PilQ.ApplicationStateHolder.progressDialog.Hide();
                    });
                    PilQ.ApplicationStateHolder.progressDialog.Show();
                }
            }
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "PilQ";
        }
        #endregion

        #region Activity Events
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Canceled)
            {
                PilQ.ApplicationStateHolder.progressDialog.Hide();
                return;
            }

            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(PilQ.ApplicationStateHolder._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
            ApplicationStateHolder.progressDialog.Show();
            var recognitionTask = new Task<Task<PillsRecognitionResult>>(async () => {
                var recognitionResult = await 
                                        this.pillsRecognitionService
                                        .RecognizePillsAsync(
                                            PilQ.ApplicationStateHolder._file.Path,
                                            Helpers.Settings.MinCircleRadiusSettings,
                                            Helpers.Settings.UseAdditionalFiltersSettings,
                                            Helpers.Settings.UseColorFiltersSettings);
                return recognitionResult;
                
            });
            PilQ.ApplicationStateHolder.runningTask = recognitionTask.ContinueWith(this.OnRecognitionCompleted);
            recognitionTask.Start();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case (Resource.Id.menu_edit):
                    PilQ.ApplicationStateHolder.mainActivity.OpenOptions();
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnRecognitionCompleted(Task<Task<PillsRecognitionResult>> completedTask)
        {
            var taskResult = completedTask.Result.Result;
            RunOnUiThread(() =>
            {
                var counterField = PilQ.ApplicationStateHolder.mainActivity.FindViewById<TextView>(Resource.Id.counter);
                var imageView = PilQ.ApplicationStateHolder.mainActivity.FindViewById<PhotoView>(Resource.Id.scImageView);
                if (taskResult.MarkedImage != null)
                {
                    imageView.SetImageBitmap(taskResult.MarkedImage);
                }
                counterField.Text = taskResult.Count.ToString();
                var tmp = PilQ.ApplicationStateHolder._file;
                PilQ.ApplicationStateHolder._file = null;
                tmp.Delete();
                PilQ.ApplicationStateHolder.progressDialog.Hide();
                ApplicationStateHolder.runningTask = null;
            });
        }
        #endregion

        #region Helpers
        private void OpenOptions()
        {
            StartActivity(typeof(SettingsActivity));
        }

        private void CreateDirectoryForPictures()
        {
            PilQ.ApplicationStateHolder._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "PilQ");
            if (!PilQ.ApplicationStateHolder._dir.Exists())
            {
                PilQ.ApplicationStateHolder._dir.Mkdirs();
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

            PilQ.ApplicationStateHolder._file = new File(PilQ.ApplicationStateHolder._dir, "_latest.jpg");

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(PilQ.ApplicationStateHolder._file));
            StartActivityForResult(intent, 0);
        }
        #endregion
    }
}
