using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Drawing;

namespace ShellyGen1Tray
{
    public class ShellyDevice
    {
        NotifyIcon notifyIcon;
        public string IP;
        public string color;
        public string name;
        public string updateInterval;
        System.Windows.Forms.Timer timer1;
        RestClient restClient;

        public bool disabled = false;

        static object lockObj = new object();

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public ShellyDevice(string _IP, string _color, string _name, string _updateInterval)
        {
            IP = _IP;
            color = _color;
            name = _name;
            updateInterval = _updateInterval;

            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit", null, new EventHandler(Exit));

            restClient = new RestClient("http://" + IP + "/meter/0", HttpVerb.GET);

            notifyIcon = new NotifyIcon();
            UpdateIcon(0);
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
            notifyIcon.Visible = true;
            notifyIcon.Text = name;

            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = Convert.ToInt32(updateInterval) * 1000;
            timer1.Tick += new EventHandler(UpdateWatts);
            timer1.Enabled = true;

        }

        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;
            timer1.Enabled = false;
            timer1.Dispose();

            disabled = true;
            ShellyGen1Tray.EvaluateExit();

            //Application.Exit();
        }

        void UpdateWatts(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(lockObj))
            {
                string json = String.Empty;
                try
                {
                    json = restClient.MakeRequest();

                    ShellyResponse response = JsonSerializer.Deserialize<ShellyResponse>(json);

                    UpdateIcon(Convert.ToInt32(response.power));

                }
                catch (Exception ex)
                {
                    //ignore timeouts due to Wifi instability
                }
                finally
                {
                    Monitor.Exit(lockObj);
                }
            }
        }


        private void UpdateIcon(int value)
        {
            String str;

            str = String.Format("{0,3}", value);

            Pen pen = new Pen(Color.FromName(color));

            SolidBrush brush = new SolidBrush(Color.White);
            Font font = new Font("Tahoma", 7);
            PointF origin = new PointF(-1, 3);

            Bitmap bitmap = new Bitmap(16, 16);
            Graphics graph = Graphics.FromImage(bitmap);
            graph.DrawLine(pen, 0, 15, 15, 15);
            graph.DrawLine(pen, 0, 0, 15, 0);

            graph.DrawString(str, font, brush, origin);

            IntPtr Hicon = bitmap.GetHicon();
            
            pen.Dispose();
            brush.Dispose();
            font.Dispose();
            bitmap.Dispose();
            graph.Dispose();

            notifyIcon.Icon = Icon.FromHandle(Hicon);

            DestroyIcon(notifyIcon.Icon.Handle);

        }



    }
}
