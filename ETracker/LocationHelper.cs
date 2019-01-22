using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ETracker
{
    public class LocationHelper
    {

        //const string cs = "Data Source=192.168.15.12;Initial Catalog=Apps;User Id=sa;Password=hachibi3033;";
        const string cs = "Data Source=latitud.gotdns.com; Initial Catalog=Apps; Persist Security Info=True; User Id=sa;Password=hachibi3033;";

        public int InsertLocation(string user, float latitude, float longitude, DateTime CreationDate, out string msg)
        {
            int id = -1;
            object result;

            msg = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(cs))
                {
                    try
                    {
                        
                        conn.Open();

                        if (conn.State == ConnectionState.Open)
                        {

                            using (SqlCommand cmd = new SqlCommand(string.Format("INSERT INTO ETrackerLocation (Person, Latitude, Longitude, CDate) VALUES ('{0}', {1}, {2}, '{3}')", user, latitude, longitude, CreationDate.ToString("yyyy-MM-dd H:mm:ss")), conn))
                            {
                                cmd.CommandType = CommandType.Text;
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch(Exception ex)
                                {
                                    msg = ex.Message;
                                }
                            }

                            using (SqlCommand r = new SqlCommand("SELECT MAX(IdLocation) FROM ETrackerLocation", conn))
                            {

                                r.CommandType = CommandType.Text;
                                try
                                {
                                    result = r.ExecuteScalar();
                                    id = int.Parse(result.ToString());
                                }
                                catch (Exception ex)
                                {
                                    msg = ex.Message;
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        msg = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return id;
        }
    }
}