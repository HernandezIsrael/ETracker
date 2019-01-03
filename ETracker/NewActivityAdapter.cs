using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace ETracker
{
    class NewActivityAdapter : BaseAdapter<string>
    {

        Activity context;
        List<string> items;

        public NewActivityAdapter(Activity context, List<string> items)
        {
            this.context = context;
            this.items = items;
        }


        public override long GetItemId(int position)
        {
            return position;
        }

        public override string this[int position]
        {
            get
            {
                return items[position];
            }
        }

        public override int Count
        {
            get
            {
                return items.Count;
            }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            string item;

            if (view == null)
            {
                //This means we don't get a cell or a row we can recycle, so we create one
                view = context.LayoutInflater.Inflate(Resource.Layout.new_activity_row, null);
            }

            //Let's see where we are inside of the list

            item = items[position];

            //We have our row.
            //Now inside of that row we have some text elements that we want to populate with data
            view.FindViewById<TextView>(Resource.Id.textViewActivityFormRow).Text = item;

            switch (position)
            {
                case 0:
                    //view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_account_circle_24, 0, 0, 0);
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).Hint = "Juan Pérez";
                    break;
                case 1:
                    //view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_assignment_24, 0, 0, 0);
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).Hint = "Instalación de serivicios";
                    break;
                case 2:
                    //view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_chrome_reader_mode_24, 0, 0, 0);
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).Hint = "Descripción";
                    break;
                case 3:
                    //view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_record_voice_over_24, 0, 0, 0);
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).Hint = "López López";
                    break;
                case 4:
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).InputType = Android.Text.InputTypes.TextFlagMultiLine;
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).Hint = "445 Mount Eden Road, Mount Eden, Auckland";
                    //view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_pin_drop_24, 0, 0, 0);
                    break;
                case 5:
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).InputType = Android.Text.InputTypes.ClassPhone;
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).Hint = "55-000-000-00";
                    //view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_phone_24, 0, 0, 0);
                    break;
                case 6:
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).InputType = Android.Text.InputTypes.ClassDatetime;
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).Hint = "DD/MM/YYYY";
                    //view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.baseline_date_range_24, 0, 0, 0);
                    break;
                default:
                    view.FindViewById<EditText>(Resource.Id.editTextActivityFormRow).InputType = Android.Text.InputTypes.TextVariationPersonName;
                    break;
            }

            
            return view;
        }

    }
}