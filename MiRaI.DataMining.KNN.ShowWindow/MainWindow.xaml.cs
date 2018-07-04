using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MiRaI.DataMining.KNN.ShowWindow.Data;
using MiRaI.DataMining.KNN.Weather;
using MiRaI.DataMining.KNN.ShowWindow.TestNS;
using System.Collections.ObjectModel;

namespace MiRaI.DataMining.KNN.ShowWindow {
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window {
		private bool isShowingTest = false;
		private Test selectedTest = null;
		private static Point_2D ZeroPoint = new Point_2D(0, 768);
		private static Point_2D UniteLength = new Point_2D(768, 768);
		private static ICollection<WeatherViewPoint> EmptyPoints = new WeatherViewPoint[0];

		public MainWindow() {
			InitializeComponent();
			WeatherViewPoint.ItemSelectFuns += DataViewPoint_ItemSelectFuns;
			listBoxTest.ItemsSource = tests;
		}

		private void DataViewPoint_ItemSelectFuns(WeatherViewPoint vp) {
			if (isShowingTest) {
				dataList_Test.SelectedItem = vp;
				dataList_Test.ScrollIntoView(vp);
			}
			else {
				DataList_Default.SelectedItem = vp;
				DataList_Default.ScrollIntoView(vp);
			}
		}

		const double pointSize = 4;

		private List<WeatherViewPoint> points = new List<WeatherViewPoint>();
		private ObservableCollection<Test> tests = new ObservableCollection<Test>();
		private ICollection<WeatherViewPoint> showingPoints;
		//ICollection<Weather.Weather> weathers;

		/// <summary>
		/// 显示附加信息
		/// </summary>
		/// <param name="message"></param>
		private void DetilShow(string message) {
			txtDetils.Text = message;
		}

		/// <summary>
		/// 在绘图区域内显示点
		/// </summary>
		/// <param name="ps"></param>
		private void ShowPoints(ICollection<WeatherViewPoint> ps) {
			if (showingPoints != null) {
				foreach (var item in showingPoints) {
					item.Parent = null;
				}
			}
			canvShow.Children.Clear();
			foreach (var item in ps) {
				//canvShow.Children.Add(item.ShowElement);
				item.Parent = canvShow;
			}
			showingPoints = ps;
		}

		/// <summary>
		/// 展示一个测试，测试未处理时显示提示板
		/// </summary>
		/// <param name="test"></param>
		private void ShowTest(Test test) {
			selectedTest = test;
			DataList_Default.Visibility = Visibility.Hidden;
			if (test.State == TestState.ready) {
				gridTestShow.Visibility = Visibility.Hidden;
				gridNeedDo.Visibility = Visibility.Visible;
				ShowPoints(EmptyPoints);
			}
			else {
				gridTestShow.Visibility = Visibility.Visible;
				gridNeedDo.Visibility = Visibility.Hidden;
				ShowOKTest(test);
			}
		}
		/// <summary>
		/// 展示一个已完成的测试
		/// </summary>
		/// <param name="test"></param>
		private void ShowOKTest(Test test) {
			isShowingTest = true;
			ShowPoints(test.PointList);
			dataList_Test.ItemsSource = test.Relist;
			gridTestPanel.DataContext = test;

			canvShow.Children.Add(test.SelectRadiusElement);

			sliderK.Value = test.K;
			sliderDis.Value = test.SelectRadius;
			if (test.AnalyseByK) {
				radioK.IsChecked = true;
			}
			else {
				radioDis.IsChecked = true;
			}
		}
		/// <summary>
		/// 导入测试数据
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Click(object sender, RoutedEventArgs e) {
			WeatherViewPoint.DictionaryED.Clear();
			DataList_Default.SelectedItem = null;

			Brush t0 = new SolidColorBrush(Color.FromRgb(0, 255, 0));
			Brush t1 = new SolidColorBrush(Color.FromRgb(255, 255, 0));
			Brush t2 = new SolidColorBrush(Color.FromRgb(255, 255, 0));
			Brush t3 = new SolidColorBrush(Color.FromRgb(255, 0, 0));
			Brush t4 = new SolidColorBrush(Color.FromRgb(0, 0, 255));

			canvShow.Children.Clear();

			ICollection<Weather.Weather> weas = new FileData(".\\Datas\\ou.csv").GetDatas();
			//weathers = weas;

			foreach (var item in weas) {
				Brush brush = null;
				switch (item.Type) {
					case 0: brush = t0; break;
					case 5: brush = t4; break;
					case 4: brush = t3; break;
					case 3: brush = t2; break;
					case 2: brush = t1; break;
				}

				WeatherViewPoint point = new WeatherViewPoint(item, 8, brush, ZeroPoint, UniteLength); //WeatherViewPoint(item, 8, brush, canvShow, new Point_2D(0, 768), new Point_2D(768));
				points.Add(point);
			}
			DataList_Default.ItemsSource = points;
			ShowPoints(points);
			Button btn = sender as Button;
			if (btn == null) return;
			btn.IsEnabled = false;
		}



		/// <summary>
		/// 添加测试按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnAddTest_Click(object sender, RoutedEventArgs e) {
			ICollection<Weather.Weather> weas = new FileData(".\\Datas\\test.csv").GetDatas();
			//weathers = weas;
			foreach (var item in weas) {
				Test t = new Test(item, ZeroPoint, UniteLength);
				tests.Add(t);

			}

			Button btn = sender as Button;
			if (btn == null) return;
			btn.IsEnabled = false;
		}

		/// <summary>
		/// 显示所有数据
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnShowAll_Click(object sender, RoutedEventArgs e) {
			ShowPoints(points);
			DataList_Default.Visibility = Visibility.Visible;
			gridTestShow.Visibility = Visibility.Hidden;
			gridNeedDo.Visibility = Visibility.Hidden;
			selectedTest = null;
			listBoxTest.SelectedItem = null;
		}

		/// <summary>
		/// 测试集列表选择变更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listBoxTest_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			Test t = listBoxTest.SelectedItem as Test;
			if (t == null) return;
			ShowTest(t);
		}

		private void btnDoFun_Click(object sender, RoutedEventArgs e) {
			if (selectedTest == null) return;
			selectedTest.KNNDo(points);
			selectedTest.State = TestState.ok;
			gridTestShow.Visibility = Visibility.Visible;
			gridNeedDo.Visibility = Visibility.Hidden;
			selectedTest.FreshPoints();
			ShowOKTest(selectedTest);
		}

		private void DataList_Default_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			//foreach (object items in e.AddedItems) {
			//	WeatherViewPoint vpoint = items as WeatherViewPoint;
				
			//}

			//foreach (object items in e.RemovedItems) {
			//	WeatherViewPoint vpoint = items as WeatherViewPoint;
			//	vpoint.IsSelect = false;
			//}

			var slist = DataList_Default.SelectedItems;

			int count = slist.Count;
			if (count <= 1) {
				foreach (var item in showingPoints) {
					item.IsNear = true;
					item.IsSelect = false;
				}
				if (count == 1) {
					WeatherViewPoint wvp = DataList_Default.SelectedItem as WeatherViewPoint;
					DetilShow(wvp.ToString());
					wvp.IsSelect = true;
				}
			}
			else {
				DetilShow($"已选择 {count} 项");
				foreach (var item in showingPoints) {
					if (slist.Contains(item)) {
						item.IsNear = true;
						item.IsSelect = true;
					}
					else {
						item.IsNear = false;
						item.IsSelect = false;
					}
				}
			}
		}

		#region ResPanelFuns
		private void sliderK_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			if (selectedTest == null) return;
			int k = Convert.ToInt32(e.NewValue);
			selectedTest.K = k;
			if (selectedTest.AnalyseByK) {
				selectedTest.FreshPoints();
				txtResault.Text = selectedTest.ResaultDescribe;

			}
		}

		private void sliderDis_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			if (selectedTest == null) return;
			selectedTest.SelectRadius = e.NewValue;
			if (!selectedTest.AnalyseByK) {
				selectedTest.FreshPoints();
				txtResault.Text = selectedTest.ResaultDescribe;
			}
		}

		private void radioK_Click(object sender, RoutedEventArgs e) {
			if (selectedTest == null) return;
			if (radioK.IsChecked == true) {
				if (selectedTest.AnalyseByK != true) {
					selectedTest.AnalyseByK = true;
					selectedTest.FreshPoints();
					txtResault.Text = selectedTest.ResaultDescribe;

				}
			}
			else {
				if (selectedTest.AnalyseByK) {
					selectedTest.AnalyseByK = false;
					selectedTest.FreshPoints();
					txtResault.Text = selectedTest.ResaultDescribe;

				}
			}
		}

		private void radioDis_Click(object sender, RoutedEventArgs e) {
			if (selectedTest == null) return;
			if (radioK.IsChecked == true) {
				if (selectedTest.AnalyseByK != true) {
					selectedTest.AnalyseByK = true;
					selectedTest.FreshPoints();
					txtResault.Text = selectedTest.ResaultDescribe;

				}
			}
			else {
				if (selectedTest.AnalyseByK) {
					selectedTest.AnalyseByK = false;
					selectedTest.FreshPoints();
					txtResault.Text = selectedTest.ResaultDescribe;

				}
			}
		}


		private void DataList_Test_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			foreach (object items in e.AddedItems) {
				WeatherViewPoint vpoint = items as WeatherViewPoint;
				vpoint.IsSelect = true;
			}

			foreach (object items in e.RemovedItems) {
				WeatherViewPoint vpoint = items as WeatherViewPoint;
				vpoint.IsSelect = false;
			}

			var slist = DataList_Default.SelectedItems;

			int count = slist.Count;
			if (count <= 1) {
				if (count == 1) DetilShow((DataList_Default.SelectedItem as Weather.Weather).ToString());
				//foreach (var item in showingPoints) {
				//	item.IsNear = true;
				//}
			}
			else {
				DetilShow($"已选择 {count} 项");
				//foreach (var item in showingPoints) {
				//	if (slist.Contains(item)) {
				//		item.IsNear = true;
				//	}
				//	else {
				//		item.IsNear = false;
				//	}
				//}
			}
		}
		#endregion




	}
}
