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

	public class Aenergy
	{
		public double total { get; set; }
		public List<double> by_minute { get; set; }
		public int minute_ts { get; set; }
	}

	public class ShellyResponseGen2
	{
		public int id { get; set; }
		public string source { get; set; }
		public bool output { get; set; }
		public double apower { get; set; }
		public double voltage { get; set; }
		public double current { get; set; }
		public Aenergy aenergy { get; set; }
		public Temperature temperature { get; set; }
	}

	public class Temperature
	{
		public double tC { get; set; }
		public double tF { get; set; }
	}


	public class ShellyDeviceGen2 : ShellyDevice
	{

		public string IP;
		Dictionary<int, RestClient> sensorRestDict = new Dictionary<int, RestClient>();

		static object lockObj = new object();

		public ShellyDeviceGen2(string _IP, string _color, string _name, string _updateInterval, int sensorNumbers) : base (_color, _name, _updateInterval, sensorNumbers)
		{
			IP = _IP;

			for (int i = 0; i < sensorNumbers; i++)
			{
				sensorRestDict.Add(i, new RestClient("http://" + IP + "/rpc/Switch.GetStatus?id=" + i , HttpVerb.GET));
			}
		}

		public override void CheckConnection()
		{
			RestClient restClient = new RestClient("http://" + IP + "/rpc/Shelly.GetStatus");

			restClient.MakeRequest();
		}

		public override void UpdateWatts(object sender, EventArgs e, int sensorNo)
		{
			if (Monitor.TryEnter(lockObj))
			{
				string json = String.Empty;
				try
				{
					json = sensorRestDict[sensorNo].MakeRequest();

					ShellyResponseGen2 response = JsonSerializer.Deserialize<ShellyResponseGen2>(json);

					UpdateIcon(Convert.ToInt32(response.apower), sensorNo);

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


	}
}
