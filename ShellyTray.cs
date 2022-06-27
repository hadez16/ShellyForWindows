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

namespace ShellyTray
{


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
                ShellyDevice shelly = null;

                if (section.Keys["APIgeneration"] == "1")
                {
                    shelly = new ShellyDeviceGen1(section.Keys["IPAddress"], section.Keys["Color"], section.SectionName, section.Keys["UpdateInterval"], Convert.ToInt32(section.Keys["NumberOfSensors"]));
                }
                else if(section.Keys["APIgeneration"] == "2")
				{
                    shelly = new ShellyDeviceGen2(section.Keys["IPAddress"], section.Keys["Color"], section.SectionName, section.Keys["UpdateInterval"], Convert.ToInt32(section.Keys["NumberOfSensors"]));
                }
                    
                try
                {
                    if(shelly != null)
                    { 
                        shelly.CheckConnection();
                        shellys.Add(shelly);
                    }

                }
                catch
                {
                    MessageBox.Show("Shelly \"" + section.SectionName + "\" ist nicht erreichbar, oder verwendet die neuere API-Generation.\nÜberspringe oder Beende....,", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            shellys.ForEach(x => x.Start());

            if (shellys.Count == 0)
                Application.Exit();
        }


    }
}
