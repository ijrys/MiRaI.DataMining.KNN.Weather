using System;
using System.Text;

namespace MiRaI.DataMining.KNN.Weather {
	public class Weather {
		int id;
		int dayID;
		DateTime date;

		int _temp;
		int _humidity;
		int _slp;
		int _type;

		string toolTipString;

		public int Temp { get => _temp; set => _temp = value; }
		public int Humidity { get => _humidity; set => _humidity = value; }
		public int Slp { get => _slp; set => _slp = value; }
		public int Type { get => _type; set => _type = value; }

		public double TempM { get => Map(_temp, 15, 35); }
		public double HumidityM { get => Map(_humidity, 25, 100); }
		public double SlpM { get => Map(_slp, 990, 1020); }
		public int DayID { get => dayID; set => dayID = value; }
		public int Id { get => id; set => id = value; }
		public DateTime Date { get => date; set => date = value; }
		public string ToolTipString {
			get {
				if (toolTipString == null) {
					StringBuilder sb = new StringBuilder();
					sb.Append(date.Year);
					sb.Append('/');
					sb.Append(date.Month);
					sb.Append('/');
					sb.Append(date.Day);
					sb.Append(" - ");
					sb.Append(dayID);
					sb.Append(Environment.NewLine);
					sb.Append("[Temp] : ");
					sb.Append(_temp);
					sb.Append(Environment.NewLine);
					sb.Append("[Humi] : ");
					sb.Append(_humidity);
					sb.Append(Environment.NewLine);
					sb.Append("[Slp] : ");
					sb.Append(_slp);
					sb.Append(Environment.NewLine);
					sb.Append("[Type] : ");
					sb.Append(_type);
					toolTipString = sb.ToString();

				}
				return toolTipString;
			}
		}

		public Point_2D GetPoint () {
			return new Point_2D() { X = TempM, Y = HumidityM };
		}

		public double Distance(Weather point) {
			Point_2D p1, p2;

			p1 = this.GetPoint();
			p2 = point.GetPoint();
			return p1.Distance(p2);
		}

		private static double Map (double v, double min, double max) {
			return (v - min) / (max - min);
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append(date.Year.ToString("0000"));
			sb.Append('/');
			sb.Append(date.Month.ToString("00"));
			sb.Append('/');
			sb.Append(date.Day.ToString("00"));
			sb.Append(" - ");
			sb.Append(dayID);
			sb.Append(" | [Temp] : ");
			sb.Append(_temp);
			sb.Append(" | [Humi] : ");
			sb.Append(_humidity);
			sb.Append(" | [Slp] : ");
			sb.Append(_slp);
			sb.Append(" | [Type] : ");
			sb.Append(_type);

			return sb.ToString();
		}
	}
}
