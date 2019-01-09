using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Graphics;
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
    [Activity(Label = "Registro")]
    public class RegisterActivity : AppCompatActivity
    {

        string dateTime = string.Empty;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.new_activity);

            ScrollView sv = FindViewById<ScrollView>(Resource.Id.scrollView1);

            TextView title = FindViewById<TextView>(Resource.Id.activityTitle);

            TextInputLayout date = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_7);
            TextInputLayout til;

            FloatingActionButton fab1 = FindViewById<FloatingActionButton>(Resource.Id.fab1);

            EditText etResponsable = FindViewById<EditText>(Resource.Id.etResponsable);
            EditText etActividad = FindViewById<EditText>(Resource.Id.etNombreActividad);
            EditText etDes = FindViewById<EditText>(Resource.Id.etDescActividad);
            EditText etCliente = FindViewById<EditText>(Resource.Id.etCliente);
            EditText etUbicacion = FindViewById<EditText>(Resource.Id.etDireccionCliente);
            EditText etTelefono = FindViewById<EditText>(Resource.Id.etTelCliente);
            EditText etFecha = FindViewById<EditText>(Resource.Id.etFechaAcordada);

            bool error;

            List<EditText> form = new List<EditText>();

            title.SetTypeface(Typeface.CreateFromAsset(Assets, "Raleway-Regular.ttf"), TypefaceStyle.Bold);

            etResponsable.Tag = "Responsable";
            etActividad.Tag = "Actividad";
            etDes.Tag = "Descripcion";
            etCliente.Tag = "Cliente";
            etUbicacion.Tag = "Ubicacion";
            etTelefono.Tag = "Telefono";
            etFecha.Tag = "Fecha";

            form.Add(etResponsable);
            form.Add(etActividad);
            form.Add(etDes);
            form.Add(etCliente);
            form.Add(etUbicacion);
            form.Add(etTelefono);
            form.Add(etFecha);

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
                        etFecha.Text = dateTime;
                    });

                    frag2.Show(FragmentManager, TimePickerFragment.TAG);


                });
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };

            fab1.Click += delegate
            {
                error = false;
                foreach (EditText et in form)
                {

                    switch (et.Tag.ToString())
                    {
                        case "Responsable":
                            if (!IsValid(".{4,}", et))
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_1);
                                til.Error = "No válido";
                                error = true;
                            }
                            else
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_1);
                                til.Error = null;
                            }
                            break;
                        case "Actividad":
                            if (!IsValid(".{10,}", et))
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_2);
                                til.Error = "No válido";
                                error = true;
                            }
                            else
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_2);
                                til.Error = null;
                            }
                            break;
                        case "Descripcion":
                            if (!IsValid(".{30,}", et))
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_3);
                                til.Error = "No válido";
                                error = true;
                            }
                            else
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_3);
                                til.Error = null;
                            }
                            break;
                        case "Cliente":
                            if (!IsValid(".{5,}", et))
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_4);
                                til.Error = "No válido";
                                error = true;
                            }
                            else
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_4);
                                til.Error = null;
                            }
                            break;
                        case "Ubicacion":
                            if (!IsValid(".{20,}", et))
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_5);
                                til.Error = "No válido";
                                error = true;
                            }
                            else
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_5);
                                til.Error = null;
                            }
                            break;
                        case "Telefono":
                            if (!IsValid(".{6,}", et))
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_6);
                                til.Error = "No válido";
                                error = true;
                            }
                            else
                            {
                                til = FindViewById<TextInputLayout>(Resource.Id.text_input_layout_6);
                                til.Error = null;
                            }
                            break;
                        case "Fecha":
                            if (!IsValid(".{5,}", et))
                            {
                                date.Error = "No válido";
                                error = true;
                            }
                            else
                            {
                                date.Error = null;
                            }
                            break;
                    }
                }

                if (error)
                {
                    Snackbar.Make(FindViewById<LinearLayout>(Resource.Id.newActivityLinearLayout), "Hay campos erróneos o incompletos. Verifícalos y vuialve a intentar", Snackbar.LengthIndefinite).SetAction("Ok", (view) => { /*Undo message sending here.*/ }).Show();
                    //Toast.MakeText(this, "Hay campos erróneos o incompletos. Verifícalos y vuialve a intentar", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "¡Listo!", ToastLength.Long).Show();
                }

            };

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

        private bool IsValid(string regex, EditText et)
        {
            Regex r = new Regex(@regex, RegexOptions.IgnoreCase);
            Match m = r.Match(et.Text);
            return m.Success;
        }

    }
}