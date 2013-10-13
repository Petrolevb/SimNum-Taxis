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
    	
    	public void tick()
    	{
    		move();
    	}
    	
    	/// <summary> Moves the taxi </summary>
    	public void move()
    	{
    		if(m_target.X == -99999 && m_target.Y == -99999)
    			return;
    		
    		for(int i=0; i<m_city.RatioTime; i++)
    		{
    			this.m_angle = Util.AngleForShortestPath(m_position, m_target);
    			m_position = Util.MoveByPolar(m_position, m_angle, m_speed);
    		}

    		// Taxi arrivé
			if(Util.Equivalent(m_target, m_position))
			{
				m_target = new Point(-99999, -99999);
			}
    	}

    	/// <summary> In meters per minut </summary>
    	private double m_speed;
    	
    	///  <summary> Angle of the current direction </summary>
        private double m_angle;
    	
    	private City m_city;
    	
    	private Point m_position;
    	public Point Position { get { return m_position; } }
    	
    	private Point m_target;
    	public Point Target { get { return m_target; } }
    	
    };
}
