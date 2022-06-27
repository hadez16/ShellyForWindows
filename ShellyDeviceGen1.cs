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

	public class ShellyResponseGen1
	{
		public double power { get; set; }
		public bool is_valid { get; set; }
		public int timestamp { get; set; }
		public List<double> counters { get; set; }
		public int total { get; set; }
	}

	public class ShellyDeviceGen1 : ShellyDevice
	{

		public string IP;
		//public RestClient restClient;
		Dictionary<int, RestClient> sensorRestDict = new Dictionary<int, RestClient>();

		static object lockObj = new object();

		public ShellyDeviceGen1(string _IP, string _color, string _name, string _updateInterval, int sensorNumbers) : base (_color, _name, _updateInterval, sensorNumbers)
		{
			IP = _IP;

			for(int i = 0; i < sensorNumbers; i++)
			{
				sensorRestDict.Add(i, new RestClient("http://" + IP + "/meter/" + i, HttpVerb.GET));
			}

	
		}

		public override void CheckConnection()
		{
			RestClient restClient = new RestClient("http://" + IP + "/status", HttpVerb.GET);

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

					ShellyResponseGen1 response = JsonSerializer.Deserialize<ShellyResponseGen1>(json);

					UpdateIcon(Convert.ToInt32(response.power), sensorNo);

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
