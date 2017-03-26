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
    using System.Threading.Tasks;
    using Components;
    using Services;
    using Android.Views;
    using System.Threading;
    using com.refractored.fab;
    using System.Collections.Concurrent;
    using PilQ.Model;

    public static class ApplicationContext {
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
        private ScaleImageView _scImageView;
        private PillsRecognitionService pillsRecognitionService;


        #region  Activity Lifecycle
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            PilQ.ApplicationContext.mainActivity = this;
            

            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();

                Button button = FindViewById<Button>(Resource.Id.myButton);
                FloatingActionButton floatingButton = FindViewById<FloatingActionButton>(Resource.Id.fab);
                _scImageView = FindViewById<ScaleImageView>(Resource.Id.scImageView);

                pillsRecognitionService = new PillsRecognitionService();
                button.Click += TakeAPicture;
                floatingButton.Click += TakeAPicture;
                PilQ.ApplicationContext.progressDialog = new ProgressDialog(this);
                PilQ.ApplicationContext.progressDialog.Indeterminate = true;
                PilQ.ApplicationContext.progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                PilQ.ApplicationContext.progressDialog.SetMessage("Processing...Please wait...");
                PilQ.ApplicationContext.progressDialog.SetCancelable(false);
                if (PilQ.ApplicationContext.runningTask != null)
                {
                    PilQ.ApplicationContext.runningTask.ContinueWith((t) => {
                        PilQ.ApplicationContext.progressDialog.Hide();
                    });
                    PilQ.ApplicationContext.progressDialog.Show();
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
                PilQ.ApplicationContext.progressDialog.Hide();
            }

            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            Uri contentUri = Uri.FromFile(PilQ.ApplicationContext._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
            
            var recognitionTask = Task.Run(async () => {
                var recognitionResult = await 
                                        this.pillsRecognitionService
                                        .RecognizePillsAsync(
                                            PilQ.ApplicationContext._file.Path,
                                            Helpers.Settings.MinCircleRadiusSettings,
                                            Helpers.Settings.UseAdditionalFiltersSettings,
                                            Helpers.Settings.UseColorFiltersSettings);
                return recognitionResult;
                
            });
            
            PilQ.ApplicationContext.runningTask = recognitionTask.ContinueWith(this.OnRecognitionCompleted);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case (Resource.Id.menu_edit):
                    PilQ.ApplicationContext.mainActivity.OpenOptions();
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public void OnRecognitionCompleted(Task<RecognitionResult> completedTask)
        {
            var taskResult = completedTask.Result;
            RunOnUiThread(() =>
            {
                var counterField = PilQ.ApplicationContext.mainActivity.FindViewById<TextView>(Resource.Id.counter);
                var imageView = PilQ.ApplicationContext.mainActivity.FindViewById<ScaleImageView>(Resource.Id.scImageView);
                counterField.Text = taskResult.Count.ToString();
                if (taskResult.Image != null)
                {
                    imageView.SetImageBitmap(taskResult.Image);
                }
                var tmp = PilQ.ApplicationContext._file;
                PilQ.ApplicationContext._file = null;
                tmp.Delete();
                PilQ.ApplicationContext.progressDialog.Hide();
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
            PilQ.ApplicationContext._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "PilQ");
            if (!PilQ.ApplicationContext._dir.Exists())
            {
                PilQ.ApplicationContext._dir.Mkdirs();
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

            PilQ.ApplicationContext._file = new File(PilQ.ApplicationContext._dir, "_latest.jpg");

            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(PilQ.ApplicationContext._file));
            PilQ.ApplicationContext.progressDialog.Show();
            StartActivityForResult(intent, 0);
        }
        #endregion
    }
}
