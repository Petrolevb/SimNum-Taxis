using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimNum_Taxis
{
    class Taxi
    {
    	public Taxi(Point position, double speed, City city)
    	{
    		this.m_position = position;
    		// this.m_target = new Point(-99999, -99999);
    		this.m_target = new Point(0, 0);    			
    		this.m_speed = speed;
    		this.m_city = city;
    	}
    	
    	/// <summary> Moves the taxi </summary>
    	public void move()
    	{
    		if(m_target.X == -99999 && m_target.Y == -99999)
    			return;
    		
    		double scaledSpeed = m_speed * m_city.RatioTime;
    		double dX = m_target.X - m_position.X,
    			   dY = m_target.Y - m_position.Y;
    		
    		if(Math.Abs(dX) >= scaledSpeed)
    			m_position.X += scaledSpeed * Math.Sign(dX);
    		else
    			m_position.X = m_target.X;
    			
			if(Math.Abs(dY) >= scaledSpeed)
    			m_position.Y += scaledSpeed * Math.Sign(dY);
			else
    			m_position.Y = m_target.Y;
			
			// Client arrivé
			if(m_target.X == m_position.X && m_target.Y == m_position.Y)
			{
				m_target = new Point(-99999, -99999);
			}
    	}

    	/// <summary> In meters per minut </summary>
    	private double m_speed;
    	
    	private City m_city;
    	
    	private Point m_position;
    	public Point Position { get { return m_position; } }
    	
    	private Point m_target;
    	public Point Target { get { return m_target; } }
    	
    };
}
