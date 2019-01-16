using Android.Graphics;
using SQLite;

namespace ETracker
{
    public class UserLocation
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string User { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Bitmap Image { get; set; }
    }
}