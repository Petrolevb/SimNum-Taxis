using System;
using System.Windows;

namespace SimNum_Taxis
{
	public class Util
	{
		#region Distance
		/// <summary> Euclidian distance between p1 and p2. </summary>
		public static double Distance(Point p1, Point p2)
		{
			double dx = p1.X - p2.X;
			double dy = p1.Y - p2.Y;
			
			return Math.Sqrt(dx*dx + dy*dy);
		}
		#endregion
		
		#region Movement related methods
		/// <summary> Returns the position of the movement of p by dX and dY, at maxSpeed. </summary>
		public static Point MoveBy(Point p, double dX, double dY)
		{
			p.X = dX + p.X;
			p.Y = dY + p.Y;
			
			return p;
		}
		
		/// <summary> Returns the position of the movement of p at angle a (in degres), at maxSpeed. </summary>
		public static Point MoveByPolar(Point p, double a, double maxSpeed)
		{
			return MoveBy(p, maxSpeed*Math.Cos(a*Math.PI/180), maxSpeed*Math.Sin(a*Math.PI/180));
		}
		
		/// <summary> Returns the angle (in degres) to take for the shortest path from p1 to p2. </summary>
		public static double AngleForShortestPath(Point p1, Point p2)
		{
			double dx = p2.X - p1.X;
			double dy = p2.Y - p1.Y;

			return Math.Atan2(dy, dx) * 180 / Math.PI;
		}
		#endregion
		
		#region Equivalent methods
		/// <summary> Returns true if p1 and p2 has nearly identical values </summary>
		public static Boolean Equivalent(Point p1, Point p2, double prec)
		{
			return p2.X >= p1.X - prec && p2.X <= p1.X + prec && p2.Y >= p1.Y - prec && p2.Y <= p1.Y + prec;
		}
		
		public static Boolean Equivalent(Point p1, Point p2)
		{
			return Equivalent(p1, p2, 30);
		}
		
		public static Boolean Equivalent(double x1, double x2, double prec)
		{
			return x2 >= x1 - prec && x2 <= x1 + prec;
		}
		#endregion
	
		#region Gaussian function
		/// <summary> Returns f(x), where f is a Gaussian with parameter sigma and nu. </summary>
		public static double Gaussian(double x, double sigma, double nu)
		{
			// value of Math.Sqrt(2*Math.PI).... For performance optimisation
			double sqrtOf2Pi = 2.50663;
			
			return Math.Exp((-(x - nu)*(x - nu))/(2*sigma*sigma)) / (sqrtOf2Pi * sigma);
		}
		
		public static double Gaussian(double x)
		{
			return Gaussian(x, 1, 0);
		}
		#endregion
		
		#region Double Gaussian function
        public static double doubleGaussian(double x)
        {
        	double gaussian1 = 5 * Util.Gaussian(x, 3.5, 12);
        	double gaussian2 = -2.2 * Util.Gaussian(x, 1.8, 12);
        	
        	return 3.5 * (gaussian1 + gaussian2) + 0.1;
        }
        #endregion
	}
}
