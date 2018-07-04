using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiRaI.DataMining.KNN.ShowWindow {
	class AxisCanvers : Panel {
		public static DependencyProperty AxisX, AxisY;
		double xNum, yNum;

		public double XNum { get => xNum; set => xNum = value; }
		public double YNum { get => yNum; set => yNum = value; }

		protected override Size MeasureOverride(Size availableSize) {
			//return base.MeasureOverride(availableSize);
			Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
			//foreach (UIElement item in base.InternalChildren) {
			//	item.mea
			//}
			return new Size();
		}
	}
}
