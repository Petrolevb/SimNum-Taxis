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
            this.m_MyTaxi = null;
            
            Console.WriteLine("LifeTime : " + (int) life + " => " + (int) life/(City.FPS));
        }
        #endregion

        #region clientTick
        public void tick()
        {
        	// If a client is in the street, he becomes older
        	if(m_MyTaxi == null)
        	{
        		m_LifeTime -= m_city.RatioTime;
        		
        		if(m_LifeTime <= 0)
        			m_city.ClientDied(this);
        	}
        	// If not, he follows his taxi
        	// TODO move a little m_position
        	else
        		m_Position = m_MyTaxi.Position;
        }
        #endregion
        
        #region Method giving the client a random color
        private Color randomColor()
        {        	
        	Random r = new Random();
        	return allColors[(int) (r.NextDouble() * allColors.Count())];
        }
        #endregion

        #region Client attributes
        private City m_city;
        
        // Taxi the client belongs to (can be null).
        private Taxi m_MyTaxi;
        public Taxi MyTaxi { get { return m_MyTaxi; } set { m_MyTaxi = value; } }
        
        // Client's color.
        private Color m_Color;
        public Color Color { get { return this.m_Color; } }
        
        // Destination of the client.
        private Point m_destination;
        public Point Destination { get { return this.m_destination; } }

        // Time in tick before despawn.
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
