using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Locations;
using Android.Content.PM;
using Android.Widget;
using Android;
using Android.Util;
using Android.Support.V4.Content;
using System.Collections.Generic;
using System.Linq;
using Android.Provider;
using Java.IO;
using Android.Graphics;
using Uri = Android.Net.Uri;

namespace ETracker
{
    [Activity(Label = "Actividades", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ILocationListener
    {

        TextView latitude;
        TextView longitude;
        TextView provider;

        ImageView cameraView;

        TextInputEditText t;

        Button locate;
        Button takePic;

        Location currentLocation;
        LocationManager locationManager;

        string locationProvider;
        public static File _file;
        public static File _dir;

        static readonly int REQUEST_LOCATION = 0;
        static readonly int REQUEST_CAMERA = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            latitude = FindViewById<TextView>(Resource.Id.latitude);
            longitude = FindViewById<TextView>(Resource.Id.longitude);
            provider = FindViewById<TextView>(Resource.Id.provider);
            t = FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);
            locate = FindViewById<Button>(Resource.Id.locate);
            cameraView = FindViewById<ImageView>(Resource.Id.cameraImageView);
            takePic = FindViewById<Button>(Resource.Id.takePic);

            InitializeLocationManager();

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
            }

            provider.Text += " " + locationProvider;

            takePic.Click += TakeAPicture;

            //takePic.Click += delegate
            //{

            //    Intent intent = new Intent(MediaStore.ActionImageCapture);
            //    StartActivityForResult(intent, 0);

            //};

            locate.Click += delegate
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
                {
                    // We have permission, go ahead and use the location.

                    //locationProvider = locationManager.GetBestProvider(new Criteria { Accuracy = Accuracy.Fine }, true);

                    Location lastKnownLocation = locationManager.GetLastKnownLocation(locationProvider);

                    if (lastKnownLocation != null)
                    {
                        locationManager.RequestLocationUpdates(locationProvider, 5000, 2, this);
                        latitude.Text = "Latitude: " + lastKnownLocation.Latitude.ToString();
                        longitude.Text = "Longitude: " + lastKnownLocation.Longitude.ToString();
                        t.Text = lastKnownLocation.Latitude.ToString() + ", " + lastKnownLocation.Longitude.ToString();
                    }
                    else
                    {
                        Toast.MakeText(this, "Couldn't get last know location", ToastLength.Long).Show();
                    }
                }
                else
                {
                    // Camera permission is not granted. If necessary display rationale & request.
                    if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
                    {
                        // Provide an additional rationale to the user if the permission was not granted
                        // and the user would benefit from additional context for the use of the permission.
                        // For example if the user has previously denied the permission.
                        Log.Info("TAG", "Displaying camera permission rationale to provide additional context.");

                        var requiredPermissions = new String[] { Manifest.Permission.AccessFineLocation };
                        Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.linearLayout1), "Location access is required", Snackbar.LengthIndefinite).SetAction("OK",
                                           new Action<View>(delegate (View obj) {
                                               ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);
                                           }
                                )
                        ).Show();
                    }
                    else
                    {
                        ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessFineLocation }, REQUEST_LOCATION);
                    }
                }

            };

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(RegisterActivity));
            StartActivity(intent);
        }


        /******************** Android ********************/


        protected override void OnResume()
        {
            base.OnResume();
        }
        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }


        /*************** Permision ***************/


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == REQUEST_LOCATION)
            {
                // Received permission result for camera permission.
                Log.Info("*************** TAG ***************", "Received response for Location permission request.");

                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    Log.Info("*************** TAG ***************", "Permission has now been granted.");
                    //Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.linearLayout1), "Permissions has been grtanted", Snackbar.LengthShort).Show();
                    InitializeLocationManager();
                }
                else
                {
                    Log.Info("*************** TAG ***************", "Location permission was NOT granted.");
                    //Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.linearLayout1), "Permissions not granted", Snackbar.LengthShort).Show();
                }
            }
            else
            {

                if (requestCode == REQUEST_CAMERA)
                {
                    // Received permission result for camera permission.
                    Log.Info("*************** TAG ***************", "Received response for camera permission request.");

                    // Check if the only required permission has been granted
                    if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                    {
                        // Location permission has been granted, okay to retrieve the location of the device.
                        Log.Info("*************** TAG ***************", "Permission has now been granted.");
                        //Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.linearLayout1), "Permissions has been grtanted", Snackbar.LengthShort).Show();
                    }
                    else
                    {
                        Log.Info("*************** TAG ***************", "Camera permission was NOT granted.");
                        Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.linearLayout1), "Permissions not granted", Snackbar.LengthShort).Show();
                    }
                }
                else
                {
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                }
            }
        }


        /*************** Location ***************/


        private void InitializeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria{ Accuracy = Accuracy.Fine };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);
            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }
            Log.Debug("************************ DEBUG ************************", "Using " + locationProvider + ".");
        }


        public void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if (currentLocation == null)
            {
                //Error Message 
                Toast.MakeText(this, "Couldn't get location", ToastLength.Long).Show();
            }
            else
            {
                latitude.Text = "Latitude: " + currentLocation.Latitude.ToString();
                longitude.Text = "Longitude: " + currentLocation.Longitude.ToString();
                t.Text = currentLocation.Latitude.ToString() + ", " + currentLocation.Longitude.ToString();
            }
        }

        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
            Log.Debug("************************ DEBUG ************************", "Provider: " + provider + ", Availability: " + status.ToString() + ", Extras: " + extras.ToString());
        }



        /*************** Camera ***************/



        private void CreateDirectoryForPictures()
        {
            _dir = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "C#Corner");
            if (!_dir.Exists())
            {
                _dir.Mkdirs();
            }
        }
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {

            Intent intent = new Intent(MediaStore.ActionImageCapture);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == (int)Permission.Granted)
            {
                // We have permission, go ahead and use the camera.
                
                Intent i = new Intent(MediaStore.ActionImageCapture);
                StartActivityForResult(i, 0);

            }
            else
            {
                // Camera permission is not granted. If necessary display rationale & request.
                if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
                {
                    // Provide an additional rationale to the user if the permission was not granted
                    // and the user would benefit from additional context for the use of the permission.
                    // For example if the user has previously denied the permission.
                    Log.Info("TAG", "Displaying camera permission rationale to provide additional context.");

                    var requiredPermissions = new String[] { Manifest.Permission.Camera };
                    Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.linearLayout1), "Camera access is required", Snackbar.LengthIndefinite).SetAction("OK",
                                       new Action<View>(delegate (View obj) {
                                           ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_CAMERA);
                                       }
                            )
                    ).Show();
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, REQUEST_CAMERA);
                }
            }

        }


        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Bitmap bm = (Bitmap)data.Extras.Get("data");
            cameraView.SetImageBitmap(bm);

        }

    }
}

