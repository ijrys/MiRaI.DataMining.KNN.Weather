using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace MiRaI.DataMining.KNN.ShowWindow {
	class DrawingCanvas : Canvas {
		private List<Visual> childrens = new List<Visual>();

		protected override int VisualChildrenCount => childrens.Count;
		protected override Visual GetVisualChild(int index) {
			return childrens[index];
		}

		public void AddVisual (Visual visual) {
			childrens.Add(visual);

			base.AddVisualChild(visual);
			base.AddLogicalChild(visual);
		}

		public void DeleteVisual (Visual visual) {
			childrens.Remove(visual);
			base.RemoveVisualChild(visual);
			base.RemoveLogicalChild(visual);
		}
	}
}
