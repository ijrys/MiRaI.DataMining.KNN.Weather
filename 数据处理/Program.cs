using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 数据处理 {
	class Wea {
		public int dayid;

		public int year, mounth, day;

		public int temp;
		public int humidity;
		public int slp;
		public int type;

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append(year);
			sb.Append(',');
			sb.Append(mounth);
			sb.Append(',');
			sb.Append(day);
			sb.Append(',');
			sb.Append(dayid);
			sb.Append(',');
			sb.Append(temp);
			sb.Append(',');
			sb.Append(humidity);
			sb.Append(',');
			sb.Append(slp);
			sb.Append(',');
			sb.Append(type);

			return sb.ToString();
		}
	}
	class Program {
		static int[] daynums = new int[] { 0, 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

		static Dictionary<int, Wea> days = new Dictionary<int, Wea>();

		static int DayId(int y, int m, int d) {
			int re = 0;
			re += (y - 2008) * 365;
			re += daynums[m];
			re += d;
			if (m == 2 && d == 29) re--;
			return re;
		}
		static void Main(string[] args) {
			for (int i = 2; i <= 13; i++) {
				daynums[i] += daynums[i - 1];
			}

			Console.WriteLine("请输入文件名：");
			string filePath = Console.ReadLine();
			string[] infos = File.ReadAllLines(filePath);

			for (int i = 1; i < infos.Length; i++) {
				int y, m, d, t, h, s, e;
				string[] ifs = infos[i].Split(',');
				y = int.Parse(ifs[0]);
				m = int.Parse(ifs[1]);
				d = int.Parse(ifs[2]);
				t = int.Parse(ifs[3]);
				h = int.Parse(ifs[4]);
				s = int.Parse(ifs[5]);
				if (ifs.Length > 6 && ifs[6].IndexOf("Rain") != -1) e = 5;
				else e = 0;

				int did = DayId(y, m, d);

				Wea wea = new Wea() { year = y, mounth = m, day = d, dayid = did, humidity = h, slp = s, temp = t, type = e };
				days[did] = wea;
			}

			Console.WriteLine("Read End");

			foreach (Wea item in days.Values) {
				if (item.type != 5) {
					continue;
				}
				int did = item.dayid;
				for (int i = 1; i < 4; i++) {
					if (days.ContainsKey(did - i) &&
						days[did - i].type < 5 - i) {
						days[did - i].type = 5 - i;
					}
				}
			}

			Console.WriteLine("out file:");
			string outPath = Console.ReadLine();
			FileStream fs = new FileStream(outPath, FileMode.OpenOrCreate);
			StreamWriter sw = new StreamWriter(fs);
			sw.AutoFlush = true;
			foreach (Wea item in days.Values) {
				sw.WriteLine(item.ToString());
			}

			sw.Close();
			fs.Close();
			Console.WriteLine("End");
		}
	}
}
