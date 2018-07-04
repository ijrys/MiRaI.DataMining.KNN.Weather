using System;
using System.Collections.Generic;
using System.Text;

namespace MiRaI.DataMining.KNN.Weather {
	public struct Point_2D {
		double x, y;

		public double X { get => x; set => x = value; }
		public double Y { get => y; set => y = value; }

		public double Distance(Point_2D p) => Distance(this, p);

		public static double Distance (Point_2D p1, Point_2D p2) {
			return Math.Sqrt(Power(p1.x - p2.x) + Power(p1.y - p2.y));
		}

		private static double Power(double d) => d * d;
		public Point_2D (double x, double y) {
			this.x = x;
			this.y = y;
		}
		public Point_2D(double v) {
			this.x = v;
			this.y = v;
		}
	}
}
