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
        }

        private Point m_Position;
        public Point Position { get { return this.m_Position; } }
    };
}
