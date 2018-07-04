using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiRaI.DataMining.KNN.Weather;

namespace MiRaI.DataMining.KNN.ShowWindow.Data {
	class FileData {
		string filepath;

		public ICollection<Weather.Weather> GetDatas () {
			List<Weather.Weather> res = new List<Weather.Weather>();
			string[] strs = null;
			try {
				strs = File.ReadAllLines(filepath);
			} catch (Exception ex){
				Console.WriteLine("ReadFile Error : " + ex.Message);
			}
			foreach (string str in strs) {
				try {
					string[] ss = str.Split(',');
					int y, m, d, did, t, h, s, e;
					y = int.Parse(ss[0]);
					m = int.Parse(ss[1]);
					d = int.Parse(ss[2]);
					did = int.Parse(ss[3]);
					t = int.Parse(ss[4]);
					h = int.Parse(ss[5]);
					s = int.Parse(ss[6]);
					if (ss.Length >= 8) e = int.Parse(ss[7]);
					else e = -1;
					Weather.Weather w = new Weather.Weather() {
						DayID = did,
						Date = new DateTime(y, m, d),
						Temp = t,
						Humidity = h,
						Slp = s,
						Type = e
					};
					res.Add(w);
				} catch (Exception ex) {
					Console.WriteLine("Error " + ex.Message);
				}
			}

			return res;
		}

		public FileData (string path) {
			filepath = path;
		}
	}
}
