using MiRaI.DataMining.KNN.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiRaI.DataMining.KNN.ShowWindow {
	public class WeatherViewPoint : Weather.Weather {
		#region type def
		public delegate void ItemSelect(WeatherViewPoint vp);
		#endregion

		public static event ItemSelect ItemSelectFuns;


		public static Dictionary<Ellipse, WeatherViewPoint> DictionaryED = new Dictionary<Ellipse, WeatherViewPoint>();

		static SolidColorBrush defaultBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
		private static Brush SelectBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

		#region 属性和字段
		double _size;
		double _sizeZoom = 1;
		Brush _colorBrush;
		Ellipse _showElement;
		Canvas _parent;
		Point_2D _point;
		Point_2D _zeroPoint;
		Point_2D _unitLength;
		bool _isSelect = false;
		bool _isMouseOver = false;
		bool _isNear = true;

		public Point_2D Point { get => _point; }
		/// <summary>
		/// 显示的元素，自动生成
		/// </summary>
		public Ellipse ShowElement { get => _showElement; }
		/// <summary>
		/// 显示元素的颜色笔刷
		/// </summary>
		public Brush ColorBrush {
			get => _colorBrush;
			set {
				_colorBrush = value;
				FreshUIBrush();
			}
		}
		/// <summary>
		/// 显示元素的直径大小
		/// </summary>
		public double Size {
			get => _size;
			set {
				_size = value;
				FreshUIPosition();
			}
		}
		/// <summary>
		/// 大小缩放
		/// </summary>
		public double SizeZoom {
			get => _sizeZoom;
			set {
				_sizeZoom = value;
				FreshUIPosition();
			}
		}
		public Point_2D ZeroPoint {
			get => _zeroPoint;
			set {
				_zeroPoint = value;
				FreshUIPosition();
			}
		}
		public Point_2D UnitLength {
			get => _unitLength;
			set {
				_unitLength = value;
				FreshUIPosition();
			}
		}
		/// <summary>
		/// 包含元素
		/// </summary>
		public Canvas Parent {
			get => _parent;
			set {
				if (_parent == value) return;
				if (_parent != null) _parent.Children.Remove(ShowElement);
				_parent = value;
				if (_parent != null) _parent.Children.Add(ShowElement);
			}
		}
		public bool IsNear {
			get => _isNear;
			set {
				if (_isNear == value) return;
				_isNear = value;
				if (_isNear) {
					_showElement.Opacity = 1;
				}
				else {
					_showElement.Opacity = 0.5;
				}
			}
		}
		public bool IsSelect {
			get => _isSelect;
			set {
				if (_isSelect == value) return;
				_isSelect = value;
				FreshUISize();
				if (IsSelect) {
					_showElement.Stroke = SelectBrush;
					_showElement.StrokeThickness = _size / 4;
				} else {
					_showElement.Stroke = null;
				}
			}
		}

		public bool IsMouseOver {
			get => _isMouseOver;
			set {
				if (_isMouseOver == value) return;
				_isMouseOver = value;
				FreshUISize();
			}
		}
		#endregion

		#region AboutFresh
		public void FreshUI() {
			FreshUIPosition();
			FreshUIBrush();
		}
		public void FreshUIPosition() {
			_point = GetPoint();
			Ellipse se = ShowElement;
			double r = Size * SizeZoom;
			se.Width = r;
			se.Height = r;
			double t, l;
			t = ZeroPoint.Y - _point.Y * UnitLength.Y - r / 2;
			l = ZeroPoint.X + _point.X * UnitLength.X - r / 2;
			se.Margin = new System.Windows.Thickness(l, t, 0, 0);
		}
		public void FreshUIBrush() {
			_showElement.Fill = _colorBrush;
		}
		private void FreshUISize () {
			if (_isSelect && _isMouseOver) {
				SizeZoom = 2;
			} else if (!_isSelect && !_isMouseOver) {
				SizeZoom = 1;
			} else {
				SizeZoom = 1.5;
			}
		}
		#endregion

		public void DeleteFromStaticDictionary() {
			DictionaryED.Remove(_showElement);
		}

		#region 构造函数
		public WeatherViewPoint(DateTime date, int temp, int humidity, int slp, int type, double size, Point_2D zeroPoint, Point_2D unitLength) :
			this(date, temp, humidity, slp, type, size, defaultBrush, zeroPoint, unitLength) {

		}
		public WeatherViewPoint(Weather.Weather weather, double size, Point_2D zeroPoint, Point_2D unitLength) :
			this(weather.Date, weather.Temp, weather.Humidity, weather.Slp, weather.Type, size, defaultBrush, zeroPoint, unitLength) {

		}
		public WeatherViewPoint(Weather.Weather weather, double size, Brush brush, Point_2D zeroPoint, Point_2D unitLength) :
			this(weather.Date, weather.Temp, weather.Humidity, weather.Slp, weather.Type, size, brush, zeroPoint, unitLength) {
		}
		public WeatherViewPoint(WeatherViewPoint wvp) : this(wvp.Date, wvp.Temp, wvp.Humidity, wvp.Slp, wvp.Type, wvp._size, wvp._colorBrush, wvp._zeroPoint, wvp._unitLength) {
		}
		public WeatherViewPoint(DateTime date, int temp, int humidity, int slp, int type, double size, Brush brush, Point_2D zeroPoint, Point_2D unitLength) {
			//base
			Temp = temp;
			Humidity = humidity;
			Slp = slp;
			Type = type;
			Date = date;

			//this
			_size = size;
			_colorBrush = brush;
			_showElement = new Ellipse();
			//_showElement.DataContext = this;

			_zeroPoint = zeroPoint;
			_unitLength = unitLength;
			//_parent = parent;

			FreshUI();

			_showElement.MouseEnter += _showElement_MouseEnter;
			_showElement.MouseLeave += _showElement_MouseLeave;
			_showElement.MouseLeftButtonDown += _showElement_MouseLeftButtonDown;

			DictionaryED[_showElement] = this;
		}
		#endregion

		#region ShowElementEvents
		private static void _showElement_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Ellipse es = sender as Ellipse;
			WeatherViewPoint vp = DictionaryED[es];
			if (vp == null) return;
			vp.IsSelect = true;
			ItemSelectFuns?.Invoke(vp);
		}
		private static void _showElement_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
			Ellipse es = sender as Ellipse;
			WeatherViewPoint vp = DictionaryED[es];
			if (vp == null || !vp.IsMouseOver) return;
			vp.IsMouseOver = false;
		}
		private static void _showElement_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
			Ellipse es = sender as Ellipse;
			WeatherViewPoint vp = DictionaryED[es];
			if (vp == null || vp.IsMouseOver) return;
			vp.IsMouseOver = true;
		}
		#endregion

	}
}
