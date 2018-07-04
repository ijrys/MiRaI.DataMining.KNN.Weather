using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MiRaI.DataMining.KNN.Weather;

namespace MiRaI.DataMining.KNN.ShowWindow.TestNS {
	public enum TestState {
		/// <summary>
		/// 就绪
		/// </summary>
		ready,
		/// <summary>
		/// 完成
		/// </summary>
		ok,
		/// <summary>
		/// 需重做
		/// </summary>
		redo,
		/// <summary>
		/// 正在加载
		/// </summary>
		loading,
		/// <summary>
		/// 出现错误
		/// </summary>
		error
	}

	public class WeatherDistancePair : WeatherViewPoint, IComparable<WeatherDistancePair> {
		private WeatherViewPoint _weather;
		private double _theDistance;
		private double _weight;

		public WeatherViewPoint Weather { get => _weather; set => _weather = value; }
		public double TheDistance { get => _theDistance; set => _theDistance = value; }
		public double Weight { get => _weight; set => _weight = value; }

		public WeatherDistancePair(WeatherViewPoint wvp, double dis) : base(wvp) {
			_theDistance = dis;
		}

		public int Compare(WeatherDistancePair x, WeatherDistancePair y) {
			if (x.TheDistance > y.TheDistance) return 1;
			else if (x.TheDistance == y.TheDistance) return 0;
			else return -1;
		}

		public int CompareTo(WeatherDistancePair other) {
			if (TheDistance > other.TheDistance) return 1;
			else if (TheDistance == other.TheDistance) return 0;
			else return -1;
		}
	}

	class Test : INotifyPropertyChanged {
		#region StaticResourse
		public static Uri readyUri = new Uri("pack://application:,,,/imgs/ready.png");
		public static Uri okUri = new Uri("pack://application:,,,/imgs/ok.png");
		public static Uri redoUri = new Uri("pack://application:,,,/imgs/redo.png");
		public static Uri loadingUri = new Uri("pack://application:,,,/imgs/loading.png");
		public static Uri errorUri = new Uri("pack://application:,,,/imgs/error.png");

		public static BitmapImage readyImg = new BitmapImage(readyUri);
		public static BitmapImage okImg = new BitmapImage(okUri);
		public static BitmapImage redoImg = new BitmapImage(redoUri);
		public static BitmapImage loadingImg = new BitmapImage(loadingUri);
		public static BitmapImage errorImg = new BitmapImage(errorUri);

		public static Brush BrushRadius = new SolidColorBrush(Color.FromRgb(255, 0, 0));
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		#region 属性和字段
		private TestState _state;
		private Weather.Weather _weather;
		private double _selectRadius;
		private double _showRadius;
		private int k = 3;
		private List<WeatherDistancePair> relist;
		private string _testString;
		private List<WeatherViewPoint> _pointList = null;
		private Ellipse _selectRadiusElement;
		private Point_2D _zeroPoint;
		private Point_2D _unitLength;
		private bool _analyseByK = true;
		private string _resaultDescribe;

		public TestState State {
			get => _state;
			set {
				_state = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("State"));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StateImage"));
			}
		}
		public Weather.Weather Weather {
			get => _weather;
		}
		public double ShowRadius {
			get => _showRadius;
			set {
				_showRadius = value;
				Point_2D center = _weather.GetPoint();

				double w, h;
				w = value * UnitLength.X;
				h = value * UnitLength.Y;
				double t, l;
				t = ZeroPoint.Y - center.Y * UnitLength.Y - h / 2;
				l = ZeroPoint.X + center.X * UnitLength.X - w / 2;

				SelectRadiusElement.Width = w;
				SelectRadiusElement.Height = h;
				SelectRadiusElement.Margin = new System.Windows.Thickness(l, t, 0, 0);
			}
		}

		public double SelectRadius {
			get => _selectRadius;
			set {
				_selectRadius = value;

			}
		}
		public int K {
			get => k;
			set {
				k = value;
			}
		}
		public List<WeatherDistancePair> Relist { get => relist; }
		public List<WeatherViewPoint> PointList {
			get {
				if (_pointList == null) {
					List<WeatherViewPoint> re = new List<WeatherViewPoint>(Relist.Count);
					foreach (var item in Relist) {
						re.Add(item);
					}
					WeatherViewPoint wvp = new WeatherViewPoint(Weather, 12, new SolidColorBrush(Color.FromRgb(255, 255, 255)), new Point_2D(0, 768), new Point_2D(768));
					re.Add(wvp);
					_pointList = re;
				}
				return _pointList;
			}
		}
		public string TestString {
			get => _testString;
			set {
				_testString = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TestString"));
			}
		}
		public BitmapImage StateImage {
			get {
				BitmapImage re = null;
				switch (State) {
					case TestState.ready:
						re = readyImg;
						break;
					case TestState.ok:
						re = okImg;
						break;
					case TestState.redo:
						re = redoImg;
						break;
					case TestState.loading:
						re = loadingImg;
						break;
					case TestState.error:
						re = errorImg;
						break;
					default:
						break;
				}
				return re;
			}
		}
		public bool AnalyseByK {
			get => _analyseByK;
			set => _analyseByK = value;
		}
		public string ResaultDescribe {
			get => _resaultDescribe;
			set {
				_resaultDescribe = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ResaultDescribe"));
			}
		}

		public Ellipse SelectRadiusElement { get => _selectRadiusElement; }
		public Point_2D ZeroPoint {
			get => _zeroPoint;
			set => _zeroPoint = value;
		}
		public Point_2D UnitLength {
			get => _unitLength;
			set => _unitLength = value;
		}

		#endregion

		public void FreshPoints() {
			//Dictionary<int, double> res = new Dictionary<int, double>();
			double[] res = new double[6];
			for (int i = 0; i < 6; i++) res[i] = 0.0;
			int k;
			double r, totleweight = 0;
			if (AnalyseByK) {
				List<WeatherDistancePair> points = Relist;
				int e = points.Count;
				k = K;
				if (k > e) k = e;
				int i = 0;
				for (; i < k; i++) {
					points[i].IsNear = true;
					//if (res.ContainsKey(points[i].Type)) {
					//	res[points[i].Type] += points[i].Weight;
					//}
					//else {
					//	res[points[i].Type] = points[i].Weight;
					//}
					res[points[i].Type] += points[i].Weight;
				}
				for (; i < e; i++) {
					points[i].IsNear = false;
				}
				r = relist[k - 1].TheDistance;
			}
			else {
				k = 0;
				foreach (var item in Relist) {
					if (item.TheDistance <= _selectRadius) {
						item.IsNear = true;
						//if (res.ContainsKey(item.Type)) {
						//	res[item.Type] += item.Weight;
						//}
						//else {
						//	res[item.Type] = item.Weight;
						//}
						res[item.Type] += item.Weight;
						k++;
					}
					else {
						item.IsNear = false;
					}
				}
				r = SelectRadius;
			}

			ShowRadius = r * 2;

			StringBuilder sb = new StringBuilder();
			sb.Append($"共获取{k}个数据");

			//foreach (var item in res) {
			//	sb.Append($"{Environment.NewLine}Type {item.Value / totleweight:00.00}");
			//}
			for (int i = 0; i < 6; i ++) {
				totleweight += res[i];
			}
			for (int i = 0; i < 6; i++) {
				if (i == 1) continue;
				sb.Append($"{Environment.NewLine}Type {i} : {res[i] / totleweight * 100.0:00.00}%");
			}
			ResaultDescribe = sb.ToString();
		}

		#region KNN About
		public void KNNDo(ICollection<WeatherViewPoint> ws) {
			relist = KNNDo(_weather, ws);
			_pointList = null;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Relist"));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointList"));
		}
		public void KNNDoAppend(ICollection<WeatherViewPoint> ws) {
			relist.AddRange(KNNDo(_weather, ws));
			relist.Sort();
			_pointList = null;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Relist"));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointList"));
		}
		/// <summary>
		/// 使用集合对一个数据进行KNN测试
		/// </summary>
		/// <param name="t">待测试点</param>
		/// <param name="ws">测试数据</param>
		/// <returns></returns>
		public static List<WeatherDistancePair> KNNDo(Weather.Weather t, ICollection<WeatherViewPoint> ws) {
			List<WeatherDistancePair> res = new List<WeatherDistancePair>();
			foreach (var item in ws) {
				double dis = t.Distance(item);
				WeatherDistancePair p = new WeatherDistancePair(item, dis);
				p.Weight = GetWeight(dis);
				res.Add(p);
			}

			res.Sort();
			return res;
		}
		/// <summary>
		/// 根据距离返回权重
		/// </summary>
		/// <param name="dis"></param>
		/// <returns></returns>
		private static double GetWeight(double dis) {
			if (dis < 0) return 1.0;
			if (dis < 0.25) return 1.0 - dis * 0.4;
			if (dis < 0.75) return 1.3 - dis * 8.0 / 5.0;
			if (dis < 1) return 0.4 - 0.4 * dis;
			return 0;
		}
		#endregion
		public Test(Weather.Weather weather, Point_2D zeroPoint, Point_2D uniteLen) {
			_zeroPoint = zeroPoint;
			_unitLength = uniteLen;
			_weather = weather;
			_testString = weather.ToString();
			_selectRadiusElement = new Ellipse();//new WeatherViewPoint(weather, 2, null, new Point_2D(0, 768), new Point_2D(768));
			_selectRadiusElement.Fill = null;
			_selectRadiusElement.Stroke = BrushRadius;
			_selectRadiusElement.StrokeThickness = 2;
		}
	}
}
