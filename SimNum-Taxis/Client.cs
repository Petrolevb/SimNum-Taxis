using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimNum_Taxis
{
    class Client
    {
        public Client(Point position)
        {
            this.m_Position = position;
            this.m_LifeTime = TimeSpan.FromSeconds(5);
        }
        
        private Point m_destination;
        public Point Destination { get { return m_destination; } set { m_destination = value; } }

        public TimeSpan LifeTime { get { return this.m_LifeTime; } }
        private TimeSpan m_LifeTime;
        
        private Point m_Position;
        public Point Position { get { return this.m_Position; } }
    };
}
