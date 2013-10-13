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
        	else
        		m_Position = m_MyTaxi.Position;
        }
        
        private Color randomColor()
        {
        	List<Color> colors = new List<Color>();
        	colors.Add(Colors.Orange);
        	colors.Add(Colors.Red);
        	colors.Add(Colors.YellowGreen);
        	colors.Add(Colors.Brown);
        	colors.Add(Colors.Blue);
        	colors.Add(Colors.HotPink);
        	colors.Add(Colors.Green);
        	colors.Add(Colors.Lime);
        	colors.Add(Colors.Magenta);
        	colors.Add(Colors.Black);
        	
        	Random r = new Random();
        	return colors[(int) (r.NextDouble() * colors.Count)];
        }

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
    };
}
