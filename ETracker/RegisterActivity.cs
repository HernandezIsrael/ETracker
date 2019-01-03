using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace ETracker
{
    [Activity(Label = "Nueva Actividad")]
    public class RegisterActivity : AppCompatActivity
    {

        string dateTime = string.Empty;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.new_activity);

            ScrollView sv = FindViewById<ScrollView>(Resource.Id.scrollView1);

            TextInputLayout date = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_7);

            FloatingActionButton fab1 = FindViewById<FloatingActionButton>(Resource.Id.fab1);

            EditText etResponsable = FindViewById<EditText>(Resource.Id.etResponsable);
            EditText etActividad = FindViewById<EditText>(Resource.Id.etNombreActividad);
            EditText etDes = FindViewById<EditText>(Resource.Id.etDescActividad);
            EditText etCliente = FindViewById<EditText>(Resource.Id.etCliente);
            EditText etUbicacion = FindViewById<EditText>(Resource.Id.etDireccionCliente);
            EditText etTelefono = FindViewById<EditText>(Resource.Id.etTelCliente);
            EditText etFecha = FindViewById<EditText>(Resource.Id.etFechaAcordada);

            sv.Enabled = false;

            etFecha.FocusChange += delegate
            {
                if (etFecha.HasFocus)
                {
                    etFecha.ShowSoftInputOnFocus = false;
                    HideKeyboard(this);
                }
            };

            etFecha.Click += delegate
            {
                HideKeyboard(this);                 
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    dateTime = time.ToLongDateString();

                    TimePickerFragment frag2 = TimePickerFragment.NewInstance(
                    delegate (DateTime time2)
                    {
                        dateTime += " " + time2.ToShortTimeString();
                        Log.Debug("------------- DEBUG -------------", "Datetime: " + dateTime);
                        etFecha.Text = dateTime;
                    });

                    frag2.Show(FragmentManager, TimePickerFragment.TAG);


                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };

            fab1.Click += FabOnClick;

        }

        public static void HideKeyboard(Activity activity)
        {
            InputMethodManager imm = (InputMethodManager)activity.GetSystemService(Activity.InputMethodService);
            //Find the currently focused view, so we can grab the correct window token from it.
            View view = activity.CurrentFocus;
            //If no view currently has focus, create a new one, just so we can grab a window token from it
            if (view == null)
            {
                view = new View(activity);
            }
            imm.HideSoftInputFromWindow(view.WindowToken, 0);
        }

        void TimeSelectOnClick(object sender, EventArgs eventArgs)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(
                delegate (DateTime time)
                {
                    dateTime += time.ToShortTimeString();
                });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong).SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

    }
}