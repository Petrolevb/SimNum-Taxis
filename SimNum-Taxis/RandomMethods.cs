using System;
using System.Windows;

namespace SimNum_Taxis
{
	public class RandomMethods
	{
		public RandomMethods()
		{
			m_random = new Random();
		}
		
		private Random m_random;
		
		/// <summary> Chooses a random, evenly probable, position inside the circle of size size </summary>
        public Point CalculateUniformPositionInCircle(int size)
        {
    		double u1 = m_random.NextDouble();
    		double u2 = m_random.NextDouble();

    		Point pos = new Point();    		
    		pos.X = Math.Sqrt(u2) * Math.Cos(2 * Math.PI * u1) * size * 1000;
    		pos.Y  = Math.Sqrt(u2) * Math.Sin(2 * Math.PI * u1) * size * 1000;

    		return pos;
        }

        // TODO improve me !!!
        /// <summary> Tells wether to spawn a client </summary>
        public bool TrySpawnClient(DateTime time)
        {
        	double hourOfDay = time.Hour + (double) time.Minute / 60;
        	
        	bool res = false;
        	int proba = 200;
        	
        	if((int) (m_random.NextDouble() * proba) == 1)
        		if(m_random.NextDouble() <= Util.doubleGaussian(hourOfDay))
        			res = true;

        	return res;
        }
        
        // TODO Arthurian function
        /// <summary> Chooses a destination inside the city </summary>
        public Point CalculateClientDestination(Point clientPosition, int size)
        {
    		double u1 = m_random.NextDouble();
    		double u2 = m_random.NextDouble();

    		Point pos = new Point();
    		pos.X = Math.Sqrt(u2) * Math.Cos(2 * Math.PI * u1) * size * 1000;
    		pos.Y  = Math.Sqrt(u2) * Math.Sin(2 * Math.PI * u1) * size * 1000;

    		return pos;
        }
        
        // TODO Improve it. Currently it waits 5 minuts + r minuts, where r € [0 .. 60]
		/// <summary> Gives the life time of a client </summary>        
        public double CalculateClientLifeTime(int fps)
        {
        	double res = 5;
        	res += m_random.NextDouble() * 60;
        	
        	return res * fps;
        }
		
		
		
		
	}
}
