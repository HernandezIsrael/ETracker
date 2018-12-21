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
            EditText editText = FindViewById<EditText>(Resource.Id.editTextActivityFormRow);
            string item;

            if (view == null)
            {
                //This means we don't get a cell or a row we can recycle, so we create one
                view = context.LayoutInflater.Inflate(Resource.Layout.new_activity_row, null);
            }

            //Let's see where we are inside of the list

            item = items[position];

            //if (position == 3)
            //{
            //    item = "(Available to subscribers only)";
            //    view.FindViewById<TextView>(Resource.Id.textViewActivityFormRow).SetTextColor(Color.Coral);
            //}
            //else
            //{
            //    view.FindViewById<TextView>(Resource.Id.textViewActivityFormRow).SetTextColor(Color.Black);
            //}

            switch (position)
            {
                case 0:
                    
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                default:

                    break;
            }

            //We have our row.
            //Now inside of that row we have some text elements that we want to populate with data
            view.FindViewById<TextView>(Resource.Id.textViewActivityFormRow).Text = item;
            return view;
        }

    }
}