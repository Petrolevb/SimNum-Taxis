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
            this.m_color = randomColor();
            
            Console.WriteLine("LifeTime : " + (int) life + " => " + (int) life/(City.FPS));
        }

        public void tick()
        {
			m_LifeTime -= m_city.RatioTime;

        	if(m_LifeTime <= 0)
        		m_city.clientDied(this);
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
        
        private Color m_color;
        public Color Color { get { return this.m_color; } }
        
        private Point m_destination;
        public Point Destination { get { return this.m_destination; } }

        private double m_LifeTime;
        public double LifeTime { get { return this.m_LifeTime; } }
        
        private Point m_Position;
        public Point Position { get { return this.m_Position; } }
    };
}
