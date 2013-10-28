using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SimNum_Taxis
{
    class Client
    {
    	#region Constructor
        public Client(City city, Point position, Point destination, double life)
        {
        	this.m_city = city;
            this.m_Position = position;
            this.m_LifeTime = life;
            this.m_destination = destination;
            this.m_Color = randomColor();
            this.m_isTargeted = false;
            this.m_MyTaxi = null;
            
            //Console.WriteLine("LifeTime : " + (int) life + " => " + (int) life/(City.FPS));
        }
        #endregion

        #region clientTick
        /// <summary> Returns false if the client gives up. </summary>
        public bool tick()
        {
        	// If a client is in the street, he becomes older
        	if(m_MyTaxi == null)
        	{
        		m_LifeTime -= m_city.RatioTime;
        		
        		if(m_LifeTime <= 0)
        		{
        			m_city.ClientDied(this);
        			return false;
        		}
        	}
        	// If not, he follows his taxi
        	else
        		m_Position = m_MyTaxi.Position;
        	
        	return true;
        }
        #endregion
        
        #region Method giving the client a random color
        private Color randomColor()
        {        	
        	Random r = new Random();
        	return allColors[(int) (r.NextDouble() * allColors.Count())];
        }
        #endregion
        
        #region Client's place in his taxi
        /// <summary> Returns 0 if the client has no taxi. -1 if he is the first client of his taxi and 1 otherwise. </summary>
        public int placeInTaxi()
        {
        	if(m_MyTaxi == null)
        		return 0;
        	return m_MyTaxi.Clients[0].Equals(this)?-1:1;
        }
        #endregion       

        #region Client attributes
        private City m_city;
        
        // Taxi the client belongs to (can be null).
        private Taxi m_MyTaxi;
        public Taxi MyTaxi { get { return this.m_MyTaxi; } set { this.m_MyTaxi = value; } }
        
        // Client's color.
        private Color m_Color;
        public Color Color { get { return this.m_Color; } }
        
        // Whever a taxi is currently going after this client.
        private bool m_isTargeted;
        public bool Targeted { get { return this.m_isTargeted; } set { this.m_isTargeted = value; } }
        
        // Destination of the client.
        private Point m_destination;
        public Point Destination { get { return this.m_destination; } }

        // Time in ticks before despawn.
        private double m_LifeTime;
        public double LifeTime { get { return this.m_LifeTime; } }
        
        private Point m_Position;
        public Point Position { get { return this.m_Position; } }
        
        #region Enum of all possible client colors.
        public static Color[] allColors = { Colors.Orange,
    										Colors.Red,
    										Colors.YellowGreen,
	    									Colors.Brown,
	    									Colors.Blue,
	    									Colors.HotPink,
	    									Colors.Green,
	    									Colors.Lime,
	    									Colors.Magenta,
	    									Colors.Black };
        #endregion    	
        #endregion
    };
}
