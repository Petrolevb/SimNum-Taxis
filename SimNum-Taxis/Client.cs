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
        public Client(Point position)
        {
            this.m_Position = position;
            this.m_LifeTime = TimeSpan.FromSeconds(5);
            this.m_color = randomColor();
        }

        private Color randomColor()
        {
        	List<Color> colors = new List<Color>();
        	colors.Add(Colors.Orange);
        	colors.Add(Colors.Red);
        	colors.Add(Colors.Yellow);
        	colors.Add(Colors.Aqua);
        	colors.Add(Colors.Blue);
        	colors.Add(Colors.HotPink);
        	colors.Add(Colors.Green);
        	colors.Add(Colors.Lime);
        	colors.Add(Colors.Magenta);
        	colors.Add(Colors.Cyan);
        	
        	Random r = new Random();
        	return colors[(int) (r.NextDouble() * colors.Count)];
        }

        private Color m_color;
        public Color Color { get { return this.m_color; } }
        
        private Point m_destination;
        public Point Destination { get { return this.m_destination; } set { m_destination = value; } }

        public TimeSpan LifeTime { get { return this.m_LifeTime; } }
        private TimeSpan m_LifeTime;
        
        private Point m_Position;
        public Point Position { get { return this.m_Position; } }
    };
}
