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
    	public Taxi(Point position)
    	{
    		this.m_position = position;
    	//	this.m_target = null;
    		this.m_speed = 833; // 50 km/h
    	}
    	
    	/// <summary> Moves the taxi </summary>
    	public void move()
    	{
    		if(m_target == null)
    			return;
    	}

    	/// <summary> In meters per minut </summary>
    	private double m_speed;
    	
    	private Point m_position;
    	public Point Position { get { return m_position; } }
    	
    	private Point m_target;
    	public Point Target { get { return m_target; } }
    	
    };
}
