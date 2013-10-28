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
        	// u1 represents the randomly choosed angle and u2 the distance to the center of the returned point.
    		double u1 = m_random.NextDouble();
    		double u2 = m_random.NextDouble();

    		Point res = new Point();    		
    		res.X = Math.Sqrt(u2) * Math.Cos(2 * Math.PI * u1) * size * 1000;
    		res.Y = Math.Sqrt(u2) * Math.Sin(2 * Math.PI * u1) * size * 1000;

    		return res;
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
        
        /// <summary> Chooses a destination inside the city following a Gaussian distribution around the point clientPosition :
		/// 		  Most of the results will be 7 kms away from the client. Fewer will be very close or very far... </summary>
        public Point CalculateClientDestination(Point clientPosition, int size)
        {
        	bool accept = false;
        	// N is the distance in kms most of the clients will like to go to
        	double N = Math.Min(7, size);
        	
    		double u1 = m_random.NextDouble();
    		// This time, u2 is not uniform => there's the highest chances for the client to spawn N kms away from its position.
    		double u2 = 0;
    		while(!accept)
    		{
    			u2 = m_random.NextDouble();
    			// The gaussian has sigma = 1/6 because we want 99% of the values to range in "x" € [0..1]
    			// And it has a nu of N/size because we want most of the clients to go N kms from their current position
    			// We divide by 2.39365 because 2.39365 is the maximum and we want the gaussian's "y" values to range between 0 and 1
    			if(m_random.NextDouble() < (Util.Gaussian(u2, 1/6.0, N/size))/2.39365)
					accept=true;
    		}

    		Point res = new Point();
    		// nb : (N/size) * size = N => Most of the clients' destination will be N kms away from their position
    		res.X = u2 * Math.Cos(2 * Math.PI * u1) * size * 1000 + clientPosition.X;
    		res.Y = u2 * Math.Sin(2 * Math.PI * u1) * size * 1000 + clientPosition.Y;

    		// This is the rejection method.
    		// If the result is inside the city, we return the result
    		// Otherwise, we calculate another result    		
    		if(res.X*res.X + res.Y*res.Y < size*size*1000000)
    			return res;
    		else
    			return CalculateClientDestination(clientPosition, size);
        }

        // TODO Improve it. Currently it waits 5 minuts + r minuts, where r € [0..60]
		/// <summary> Gives the life time of a client </summary>        
        public double CalculateClientLifeTime(int fps)
        {
        	double res = 5;
        	res += m_random.NextDouble() * 60;
        	
        	return res * fps;
        }
	}
}
