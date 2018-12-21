using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace ETracker
{
    [Activity(Label = "Nueva Actividad")]
    public class RegisterActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.new_activity);

            List<string> labels = new List<string>();
            ArrayAdapter arrayAdapter;
            ListView listView = FindViewById<ListView>(Resource.Id.listViewNewActivity);

            labels.Add("Responsable");
            labels.Add("Nombre de la actividad");
            labels.Add("Descripción de la actividad");
            labels.Add("Cliente");
            labels.Add("Dirección cliente");
            labels.Add("Tel. Contacto cliente");
            labels.Add("Fecha acordada");

            arrayAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleExpandableListItem1, labels);

            listView.Adapter = new NewActivityAdapter(this, labels);

        }

    }
}