using System;
using System.Windows;

namespace SimNum_Taxis
{
	public class RandomMethods
	{
		private Random m_random;
		
		public RandomMethods()
		{
			m_random = new Random();
		}
				
		#region Clients spawning position and destination algorithms
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

        /// <summary> Chooses a destination inside the city following a Gaussian distribution around the point clientPosition :
		/// 		  Most of the results will be 7 kms away from the client. Fewer will be very close or very far... </summary>
        public Point CalculateClientDestination(Point clientPosition, int size)
        {
        	// N is the distance in kms most of the clients will like to go to
        	double N = Math.Min(7, size);
        	
    		double u1 = m_random.NextDouble();
    		// This time, u2 is not uniform => there's the highest chances for the client to spawn N kms away from its position.
    		// The gaussian has sigma = 1/6 because we want 99% of the values to range in "x" € [0..1]
    		// And it has a nu of N/size because we want most of the clients to go N kms from their current position
    		double u2 = GetRandomValueFollowingGaussian(1/6.0, N/size);
    		

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
        #endregion
        
        #region Random value picker in a Gaussian
        /// <summary> Returns a random number following the given Gaussian random distribution. 
        ///           This will only check values in the 99% of the gaussian distribution, ie +- 3 sigmas from the center </summary>
        public double GetRandomValueFollowingGaussian(double sigma, double nu)
        {
        	double res = nu;
        	bool accept = false;
        	double maxValue = Util.Gaussian(nu, sigma, nu);
        	
        	// Bounds in between wich the random value will range.
        	// We set A and B in order to get 99% of the Gaussian values covered.
        	double A = nu - 3*sigma;
        	double B = nu + 3*sigma;
        	  	
        	while(!accept)
    		{
        		res = m_random.NextDouble()*(B-A) + A;
    			
    			if(m_random.NextDouble() < (Util.Gaussian(res, sigma, nu))/maxValue)
					accept = true;
    		}
        	
        	return res;
        }
        #endregion

        #region Client frequency of spawn algorithm
        /// <summary> Tells wether to spawn a client </summary>
        public bool TrySpawnClient(DateTime time, int size)
        {
        	bool res = false;
        	double hourOfDay = time.Hour + (double) time.Minute / 60;
        	
        	int ratioRP = 200;
        	int minimumSpawnPerDay = 300;
        	double cityArea = Math.PI * size * size / 4;
        	double probabilityMinimum = minimumSpawnPerDay / ((double) (60 * City.FPS * 12));
        	double probability = (cityArea * 1900) / ((double) (ratioRP * City.FPS * 60 * 12));
        	probability = Math.Max(probabilityMinimum, probability);
        	
        	if((int) (m_random.NextDouble() / probability) == 1)
        		if(m_random.NextDouble() <= Util.doubleGaussian(hourOfDay))
        			res = true;

        	return res;
        }
        #endregion
        
        #region Client time before despawn algorithm
		/// <summary> Gives the life time of a client </summary>        
        public double CalculateClientLifeTime()
        {
        	return GetRandomValueFollowingGaussian(4, 15) * City.FPS;
        }
        #endregion
	}
}
