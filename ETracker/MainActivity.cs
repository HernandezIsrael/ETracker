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
using Android.Graphics;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Android.Telephony;
using System.Threading.Tasks;

namespace ETracker
{
    [Activity(Label = "Check-in", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ILocationListener
    {

        LinearLayout linearLayout;
        ImageView cameraView;
        TextInputEditText t;
        TextView title;
        FloatingActionButton fab;
        FloatingActionButton takePic;

        Location currentLocation;
        LocationManager locationManager;

        TelephonyManager mTelephonyMgr;

        Bitmap bm;

        LocationHelper lh = new LocationHelper();

        string locationProvider;
        string number;

        static readonly int REQUEST_LOCATION = 0;
        static readonly int REQUEST_CAMERA = 1;
        static readonly int REQUEST_PHONE = 2;

        const string photoTag = "Photo";
        const string mailTag = "Mail";
        const string doneTag = "Done";

        string latitude = string.Empty;
        string longitude = string.Empty;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            dialog.SetTitle("AVISO!");
            dialog.SetMessage("Esta aplicación hará uso de servicios como \"Teléfono\", \"Cámará\" y \"Localización\" para conocer el estado actual del teléfono. Es necesario que autorices el uso de estos servicios cuando el dispositivo lo solicite.\n Se enviará un correo por cada aviso de arribo o \"Check-in\" a tus supervisores directos. Favor de utilizarla con moderación.");
            dialog.SetIcon(Resource.Drawable.baseline_warning_24);
            dialog.SetNeutralButton("Entendido", NeutralAction);
            dialog.Show();            

            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            t = FindViewById<TextInputEditText>(Resource.Id.textInputEditText1);
            cameraView = FindViewById<ImageView>(Resource.Id.cameraImageView);
            takePic = FindViewById<FloatingActionButton>(Resource.Id.takePic);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout1);
            title = FindViewById<TextView>(Resource.Id.homeTitle);
            //mDrawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            takePic.Tag = photoTag;

            InitializeLocationManager();

            mTelephonyMgr = (TelephonyManager)GetSystemService(TelephonyService);

            if ( !(ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneState) == (int)Permission.Granted) )
            {
                // permission is not granted. If necessary display rationale & request.
                RequestPhonePermission();
            }

            title.SetTypeface(Typeface.CreateFromAsset(Assets, "Product Sans Regular.ttf"), TypefaceStyle.Bold);
            t.SetTypeface(Typeface.CreateFromAsset(Assets, "Product Sans Regular.ttf"), TypefaceStyle.Normal);

            fab.Visibility = ViewStates.Invisible;

            fab.Click += FabOnClick;
            takePic.Click += TakeAPicture;

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            int id = item.ItemId;

            switch (id)
            {
                case Resource.Id.action_settings:
                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    dialog.SetTitle("Etracker");
                    dialog.SetMessage("Creado por Israel Hernández (Depto. de Sistemas) para uso exclusivo de Clusmext S.A de C.V\nVersión: 1.0");
                    dialog.SetIcon(Resource.Drawable.baseline_account_circle_24);
                    dialog.SetNeutralButton("OK", NeutralAction);
                    dialog.Show();
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
                Log.Info("@string/log_debug_tag", "Received response for Location permission request.");

                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    Log.Info("@string/log_debug_tag", "Permission has now been granted.");

                    try
                    {
                        Snackbar.Make(linearLayout, "Gracias por otorgar los permisos", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "Gracias por otorgar los permisos", ToastLength.Long).Show();
                        Log.Debug("@string/log_debug_tag", ex.Message);
                    }

                    InitializeLocationManager();

                    ////////////////////////////////////// BORRAR SI NO FUNCIONA ///////////////////////////////////////////////

                    Location lastKnownLocation = locationManager.GetLastKnownLocation(locationProvider);

                    if (lastKnownLocation != null)
                    {
                        locationManager.RequestLocationUpdates(locationProvider, 5000, 2, this);
                        latitude = lastKnownLocation.Latitude.ToString();
                        longitude = lastKnownLocation.Longitude.ToString();
                        t.Text = "N: " + latitude + ", W: " + longitude;
                    }
                    else
                    {
                        Toast.MakeText(this, "Couldn't get last know location", ToastLength.Long).Show();
                    }

                    ////////////////////////////////////// BORRAR SI NO FUNCIONA ///////////////////////////////////////////////
                }
                else
                {
                    Log.Info("@string/log_debug_tag", "Location permission was NOT granted.");
                    try
                    {
                        Snackbar.Make(linearLayout, "No se otorgó el permiso, la aplicación no funcionará correctamente :(", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "No se otorgó el permiso, la aplicación no funcionará correctamente :(", ToastLength.Long).Show();
                        Log.Debug("@string/log_debug_tag", ex.Message);
                    }
                }
            }
            else if (requestCode == REQUEST_CAMERA)
            {
                // Received permission result for camera permission.
                Log.Info("@string/log_debug_tag", "Received response for camera permission request.");

                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    Log.Info("@string/log_debug_tag", "Permission has now been granted.");

                    try
                    {
                        Snackbar.Make(linearLayout, "¡Gracias! :D", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "¡Gracias! :D", ToastLength.Long).Show();
                        Log.Debug("@string/log_debug_tag", ex.Message);
                    }

                }
                else
                {
                    Log.Info("@string/log_debug_tag", "Camera permission was NOT granted.");
                    try
                    {
                        Snackbar.Make(linearLayout, "No se otorgó el permiso, la aplicación no funcionará correctamente :c", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "No se otorgó el permiso, la aplicación no funcionará correctamente :c", ToastLength.Long).Show();
                        Log.Debug("@string/log_debug_tag", ex.Message);
                    }
                }
            }
            else if (requestCode == REQUEST_PHONE)
            {
                // Received permission result for phone permission.
                Log.Info("@string/log_debug_tag", "Received response for PHONE permission request.");

                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    Log.Info("@string/log_debug_tag", "Permission has now been granted.");

                    try
                    {
                        Snackbar.Make(linearLayout, "¡Gracias! :D", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "¡Gracias! :D", ToastLength.Long).Show();
                        Log.Debug("@string/log_debug_tag", ex.Message);
                    }

                }
                else
                {
                    Log.Info("@string/log_debug_tag", "PHONE permission was NOT granted.");
                    try
                    {
                        Snackbar.Make(linearLayout, "No se otorgó el permiso, la aplicación no funcionará correctamente :c", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "No se otorgó el permiso, la aplicación no funcionará correctamente :c", ToastLength.Long).Show();
                        Log.Debug("@string/log_debug_tag", ex.Message);
                    }
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        public void RequestPhonePermission()
        {
            // permission is not granted. If necessary display rationale & request.
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadPhoneState))
            {
                // Provide an additional rationale to the user if the permission was not granted
                Log.Info("TAG", "Displaying readphonestate permission rationale to provide additional context.");

                var requiredPermissions = new String[] { Manifest.Permission.ReadPhoneState };
                Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.linearLayout1), "Phone state access is required", Snackbar.LengthIndefinite).SetAction("OK",
                                   new Action<View>(delegate (View obj)
                                   {
                                       ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_PHONE);
                                   }
                        )
                ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadPhoneState }, REQUEST_PHONE);
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
            Log.Debug("@string/log_debug_tag", "Using " + locationProvider + ".");
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
                latitude = currentLocation.Latitude.ToString();
                longitude = currentLocation.Longitude.ToString();
                t.Text = "N: " + latitude + ", W: " + longitude;
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
            Log.Debug("@string/log_debug_tag", "Provider: " + provider + ", Availability: " + status.ToString() + ", Extras: " + extras.ToString());
        }


        /*************** Camera ***************/


        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            string msg = string.Empty;
            int result;
            Intent intent;

            switch (takePic.Tag.ToString())
            {
                case photoTag:

                    intent = new Intent(MediaStore.ActionImageCapture);

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

                    break;
                case mailTag:

                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    dialog.SetIcon(Resource.Drawable.baseline_mail_outline_24);
                    dialog.SetTitle("Enviando...");
                    dialog.SetMessage("Tu alerta se enviará por correo en breve.");
                    dialog.SetNeutralButton("OK", NeutralAction);
                    dialog.Show();

                    Task.Factory.StartNew(()=> {

                        try
                        {
                            number = mTelephonyMgr.Line1Number.ToString();
                        }
                        catch (Exception ex)
                        {
                            number = "The user rejected [READ_PHONE_STATE] permission";
                            Log.Debug("DEBUG", ex.Message);

                        }

                        SendEmail(bm, number);

                        result = lh.InsertLocation(number, float.Parse(latitude), float.Parse(longitude), DateTime.Now, out msg);

                        if (result > 0)
                        {
                            try
                            {
                                Snackbar.Make(linearLayout, "Se ha actualizado la base de datos", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                            }
                            catch
                            {
                                Toast.MakeText(this, "Se ha actualizado la base de datos", ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            try
                            {
                                Snackbar.Make(linearLayout, "No se pudo conectar con la BD.", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                            }
                            catch
                            {
                                Toast.MakeText(this, "No se pudo conectar con la BD.", ToastLength.Long).Show();
                            }
                        }

                    });

                    takePic.SetImageResource(Resource.Drawable.baseline_linked_camera_24);
                    takePic.Tag = photoTag;
                    cameraView.SetImageBitmap(null);
                    cameraView.SetImageResource(Resource.Drawable.baseline_speaker_phone_24);

                    break;
                case doneTag:
                    break;
            }

        }

        private void NeutralAction(object sender, DialogClickEventArgs e)
        {
            return;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                bm = (Bitmap)data.Extras.Get("data");
                cameraView.SetImageBitmap(bm);
                takePic.SetImageResource(Resource.Drawable.baseline_mail_outline_24);
                takePic.Tag = mailTag;
            }
            else
            {
                takePic.Tag = photoTag;
                cameraView.SetImageBitmap(null);
                cameraView.SetImageResource(Resource.Drawable.baseline_speaker_phone_24);
            }

            /*********** Obtain the location ***********/

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                // We have permission, go ahead and use the location.

                Location lastKnownLocation = locationManager.GetLastKnownLocation(locationProvider);

                if (lastKnownLocation != null)
                {
                    locationManager.RequestLocationUpdates(locationProvider, 5000, 2, this);
                    latitude = lastKnownLocation.Latitude.ToString();
                    longitude = lastKnownLocation.Longitude.ToString();
                    t.Text = "N: "+ latitude + ", W: " + longitude;
                }
                else
                {
                    Toast.MakeText(this, "Couldn't get last know location", ToastLength.Long).Show();
                }
            }
            else
            {
                // Location permission is not granted. If necessary display rationale & request.
                if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
                {

                    Log.Info("@string/log_debug_tag", "Displaying camera permission rationale to provide additional context.");

                    var requiredPermissions = new String[] { Manifest.Permission.AccessFineLocation };

                    try
                    {
                        Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.linearLayout1), "Location access is required", Snackbar.LengthIndefinite).SetAction("OK", new Action<View>(delegate (View obj) {
                            ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);
                        }
                                )
                        ).Show();
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, "Location access is required", ToastLength.Long).Show();
                        Log.Debug("@string/log_debug_tag", ex.Message);
                    }


                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessFineLocation }, REQUEST_LOCATION);
                }
            }

        }

        /*************** Mail ***************/

        public void SendEmail(Bitmap bmpImg, string user)
        {

            MemoryStream stream = new MemoryStream();
            MemoryStream imageStream;
            Attachment image;
            byte[] ms;

            bmpImg.Compress(Bitmap.CompressFormat.Png, 100, stream);
            ms = stream.ToArray();

            //write bytes back into the stream
            imageStream = new MemoryStream(ms);

            //Attachment constructor accepts three parameters, stream, imageName, and contentType
            image = new Attachment(imageStream, "location.png", "image/png");

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("etacker@gmail.com");
                mail.To.Add("israel.hernandez@clusmext.com");
                mail.Subject = "Informe de visita.";
                mail.Body = string.Format("Informe de visita." +
                    "\n\nRealizó: {0}\nUsuario: {1}" +
                    "\nUbicación: {2}" +
                    "\nEvidencia: {3}" +
                    "\n\nEste correo se generó automáticamente a través de la aplicación ETracker y no puede ser manipiulado por el usuario." +
                    "\nSi el correo presenta algún valor faltante o dudoso puede significar que el usuario no aprovó los permisos solicitados por la aplicación.", "informe de arribo al lugar (check-in).", user, (t.Text == "")?"The user has not given the requested [LOCATION] permission":t.Text, "Se ha adjuntado la fotografía capturada por el usuario");
                mail.Attachments.Add(image);
                SmtpServer.Port = 587;  //gmail default port
                SmtpServer.Credentials = new System.Net.NetworkCredential("termoinnova1@gmail.com", "Iyhnbsfg55+*");
                SmtpServer.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
                SmtpServer.Send(mail);
                try
                {
                    Snackbar.Make(linearLayout, "Se ha enviado la confirmación correctamente.", Snackbar.LengthIndefinite).SetAction("OK", (view) => { }).Show();
                }
                catch
                {
                    Toast.MakeText(this, "Se ha enviado la confirmación correctamente.", ToastLength.Short).Show();
                }

            }

            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }

    }
}

