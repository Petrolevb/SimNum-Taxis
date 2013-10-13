using System;
using System.Windows;

namespace SimNum_Taxis
{
	public class Util
	{
		public Util() { }
		
		/// <summary> Euclidian distance between p1 and p2. </summary>
		public static double Distance(Point p1, Point p2)
		{
			double dx = p1.X - p2.X;
			double dy = p1.Y - p2.Y;
			
			return Math.Sqrt(dx*dx + dy*dy);
		}
		
		
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
		
		/// <summary> Returns true if p1 and p2 has nearly identical values </summary>
		public static Boolean Equivalent(Point p1, Point p2, double prec)
		{
			return p2.X >= p1.X - prec && p2.X <= p1.X + prec && p2.Y >= p1.Y - prec && p2.Y <= p1.Y + prec;
		}
		public static Boolean Equivalent(Point p1, Point p2)
		{
			return Equivalent(p1, p2, 5);
		}
		
	}
}
