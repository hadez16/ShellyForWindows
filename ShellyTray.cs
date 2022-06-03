using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using System.Threading;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using IniParser;
using IniParser.Model;

namespace ShellyGen1Tray
{
    

    public class ShellyResponse
    {
        public double power { get; set; }
        public bool is_valid { get; set; }
        public int timestamp { get; set; }
        public List<double> counters { get; set; }
        public int total { get; set; }
    }

    

    public partial class ShellyGen1Tray : ApplicationContext
    {

        static List<ShellyDevice> shellys = new List<ShellyDevice>();


        public ShellyGen1Tray()
        {
            ReadConfigAndInitializeShellys();
        }

        public static void EvaluateExit()
        {
            if (shellys.All(x => x.disabled))
                Application.Exit();
        }

        void ReadConfigAndInitializeShellys()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\config.ini");

            foreach (SectionData section in data.Sections)
            {
                 
                RestClient restClient = new RestClient("http://" + section.Keys["IPAddress"] + "/meter/0", HttpVerb.GET);
                try
                {
                    string json = restClient.MakeRequest();

                    shellys.Add(new ShellyDevice(section.Keys["IPAddress"], section.Keys["Color"], section.SectionName, section.Keys["UpdateInterval"]));

                }
                catch
                {
                    MessageBox.Show("Shelly \"" + section.SectionName + "\" ist nicht erreichbar, oder verwendet die neuere API-Generation.\nÜberspringe oder Beende....,", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } 

            }

            if (shellys.Count == 0)
                Application.Exit();
        }


    }
}
