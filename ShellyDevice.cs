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

namespace ShellyTray
{
    public abstract class ShellyDevice
    {
        //NotifyIcon notifyIcon;
        public string color;
        public string name;
        public string updateInterval;
        List<System.Windows.Forms.Timer> timerList = new List<System.Windows.Forms.Timer>();
        Dictionary<int, NotifyIcon> sensorIconDict = new Dictionary<int, NotifyIcon>();
        ToolStripMenuItem exitMenuItem;
        public int numberOfSensors;

        public bool disabled = false;

        static object lockObj = new object();

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        public ShellyDevice(string _color, string _name, string _updateInterval, int _numberOfSensors)
        {
           
            color = _color;
            name = _name;
            updateInterval = _updateInterval;
            numberOfSensors = _numberOfSensors;

            exitMenuItem = new ToolStripMenuItem("Exit", null, new EventHandler(Exit));

            
        }

        public void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            
            foreach(KeyValuePair<int,NotifyIcon> kvp in sensorIconDict )
			{
                kvp.Value.Visible = false;
			}
            
            timerList.ForEach(x => { x.Enabled = false; x.Dispose(); });
            
            disabled = true;
            ShellyGen1Tray.EvaluateExit();

            //Application.Exit();
        }

        public void Start()
		{
            for (int i = 0; i < numberOfSensors; i++)
            {
                int sensorNo = i;
                NotifyIcon notifyIcon = new NotifyIcon();
                notifyIcon.ContextMenuStrip = new ContextMenuStrip();
                notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);
                notifyIcon.Visible = true;
                notifyIcon.Text = name + "->" + i;

                sensorIconDict.Add(i, notifyIcon);

                UpdateIcon(0, i);

                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = Convert.ToInt32(updateInterval) * 1000;
                timer.Tick += (sender2, e2) => UpdateWatts(sender2, e2, sensorNo);
                timer.Enabled = true;

                timerList.Add(timer);

            }
        }

        public abstract void UpdateWatts(object sender, EventArgs e, int sensorNo);

        public abstract void CheckConnection();

        public void UpdateIcon(int value, int sensorNo)
        {
            String str;

            if (value < 1000)
                str = String.Format("{0,3}", value);
            else
                str = String.Format("{0:#,.#}", value);

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

            sensorIconDict[sensorNo].Icon = Icon.FromHandle(Hicon);

            DestroyIcon(sensorIconDict[sensorNo].Icon.Handle);

        }



    }
}
