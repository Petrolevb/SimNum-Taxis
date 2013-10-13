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
    		this.m_clients = new List<Client>();
    		this.m_position = position;
    		this.m_city = city;
    		this.m_speed = speed;
    		getNewDestination();
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
    		
    		for(int i=0; i < m_city.RatioTime; i++)
    		{
    			this.m_angle = Util.AngleForShortestPath(m_position, m_target);
    			m_position = Util.MoveByPolar(m_position, m_angle, m_speed);
    		}
    		
    		// Taxi reached its target
			if(Util.Equivalent(m_target, m_position))
			{
				// Did the taxi pick up a client ?
				bool targetWasAClient = false;
				if(m_clients.Count < 2)
				{
					Client c = m_city.getClientWaitingAtPosition(m_position);
					if(c != null)
					{
						Console.WriteLine("A taxi picked up a Client !");
						m_clients.Add(c);
						c.MyTaxi = this;
						targetWasAClient = true;					
					}
				}
				// If not, the taxi reached a client destination => Success
				if(!targetWasAClient)
				{
					foreach(Client c in m_clients)
						if(Util.Equivalent(m_target, c.Destination))
						{
							m_city.ClientPleased(c);
							m_clients.Remove(c);
						}
				}
				getNewDestination();
			}
    	}
    	
    	// List of the clients in the taxi
    	private List<Client> m_clients;
    	public List<Client> Clients { get { return m_clients; } }
    	public bool hasRoomLeft() { return m_clients.Count < 2; }
    	
    	/// <summary> Takes a client and sets new taxis' destination (cf diagram) </summary>
    	public void addClientToTaxi(Client c)
    	{
    		m_clients.Add(c);
    		
    		getNewDestination();
    	}
    	
    	/// <summary> Tells the taxi where to go according to its clients </summary>
    	private void getNewDestination()
    	{
    		// If the taxi has 2 clients, it delivers first the closest one
    		if(m_clients.Count == 2)
    		{
    			if(Util.Distance(m_position, m_clients[0].Destination) < Util.Distance(m_position, m_clients[1].Destination))
    				m_target = m_clients[0].Destination;
    			else
    				m_target = m_clients[1].Destination;
    		}
    		// Otherwise, it goes to the only clients' destination
    		else if(m_clients.Count == 1)
    			m_target = m_clients[0].Destination;
    		// If the taxi has no clients, it goes to the closest one
    		else
    		{
    			Client possibleNewClient = m_city.getAwaitingClientClosestTo(m_position);
    			if(possibleNewClient == null)
    				m_target = new Point(-99999, -99999);
    			else
    				m_target = possibleNewClient.Position;
    		}
    			
    		
    		Console.WriteLine(m_target);
    	}
    	
    	/// <summary> Make a client leave the taxi </summary>
    	public void removeClientFromTaxi(Client c)
    	{
    		m_clients.Remove(c) ;
    	}

    	/// <summary> In meters per minut </summary>
    	private double m_speed;
    	
    	///  <summary> Angle of the current direction </summary>
        private double m_angle;
    	
    	private City m_city;
    	
    	private Point m_position;
    	public Point Position { get { return m_position; } }
    	
    	private Point m_target;
    	public Point Target { get { return m_target; } set { m_target = value; } }
    };
}
