using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimNum_Taxis
{
	// TODO Create AI class
	
    class Taxi
    {
    	#region Constructor
    	public Taxi(Point position, double speed, City city)
    	{
    		this.m_clients = new List<Client>();
    		this.m_position = position;
    		this.m_myCity = city;
    		this.m_speed = speed;
    		AI.RefreshDestination(this);
    	}
    	#endregion
    	
    	#region taxiTick
    	public void tick()
    	{
    		AI.move(this);
    		AI.ManageArrival(this);
    	}
    	#endregion
    	
    	#region Clients in taxi management methods
    	/// <summary> Is the taxi not full ? </summary>
    	public bool hasRoomLeft()
    	{
    		return m_clients.Count < 2;
    	}
    	
    	/// <summary> Takes a client and sets new taxis' destination </summary>
    	public void addClientToTaxi(Client c)
    	{
    		m_clients.Add(c);
    		AI.RefreshDestination(this);
    	}
    	
    	/// <summary> Make a client leave the taxi </summary>
    	public void removeClientFromTaxi(Client c)
    	{
    		m_clients.Remove(c);
    	}
    	#endregion

    	#region Taxi attributes
    	/// <summary> In meters per minut </summary>
    	private double m_speed;
    	public double Speed { get { return m_speed; } }
        
        // List of the clients in the taxi
    	private List<Client> m_clients;
    	public List<Client> Clients { get { return m_clients; } }
    	
    	private City m_myCity;
    	public City MyCity { get { return m_myCity; } }
    	
    	private Point m_position;
    	public Point Position { get { return m_position; } set { m_position = value; } }
    	
    	/// <summary> Destination. Default value is new Point(-99999 -99999) </summary>
    	private Point m_target;
    	public Point Target { get { return m_target; } set { m_target = value; } }
    	#endregion
    };
}
