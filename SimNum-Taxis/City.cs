using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;
using System.Windows;

namespace SimNum_Taxis
{
    class City
    { 
    	// TODO CORRECT ME !!!!
    	public static int FPS = 40;
    	public static int INTERVAL = 1000 / FPS;
    	
        #region Constructor
        public City()
        {
            this.m_RatioTime = 1;
            this.m_Taxis = new List<Taxi>();
            this.m_Clients = new List<Client>();
            this.m_SizeCity = 10;
            this.m_NumberOfClient = 0;
            this.m_NumberOfAwaiting = 0;
            this.m_NumberInsideTaxis = 0;
            this.m_NumberOfUnsatisfied = 0;
            this.m_NumberOfPleased = 0;
            this.m_random = new RandomMethods(); 
        }
        #endregion
        
		#region Ticks management
        /// <summary> Main loop. Is called several times each second. </summary>
        public void gameTick()
        {
        	// TODO Change PositionInCircle According to day's time
        	// Trys to spawn a new Client
        	for(int i = 0; i < (int) RatioTime; i++)
        		if(m_random.TrySpawnClient(m_Time))
	        		SpawnClient(m_random.CalculateUniformPositionInCircle(m_SizeCity));
        /**/	

        	// Makes every taxi move
        	foreach(Taxi t in m_Taxis)
        		t.tick();

        	// Makes every client move
        	for(int i = 0; i < m_Clients.Count; i++)
        	{
        		if(m_Clients[i].tick() == false)
        			i--;
        	}
        }
		#endregion
        
        #region Taxis management
        /// <summary> Adds a new Taxi </summary>
        public void AddsTaxi() 
        {
        	// Speed of the taxi
        	double  speed = 50; 		 	    // 50 km/h
	    			speed *= 1000 / 60;   		// 833.33 m/min
	    			speed /= 1000 / INTERVAL;	// 16.66 m/min every tick
        	
        	this.m_Taxis.Add(new Taxi(m_random.CalculateUniformPositionInCircle(m_SizeCity), speed, this));
        }
        /// <summary> Removes a Taxi and its clients </summary>
        public void RemovesTaxi() 
        { 
            if(this.m_Taxis.Count > 0) 
            {
            	foreach(Client c in this.m_Taxis[this.NumberOfTaxis - 1].Clients)
            		ClientDied(c);
            	this.m_Taxis.RemoveAt(this.NumberOfTaxis-1);
            }
        }
        /// <return> Get Number of taxis </return>
        public int NumberOfTaxis { get { return this.m_Taxis.Count; } }
        private List<Taxi> m_Taxis;
        ///<return>List of all taxis position</return>
        public List<Point> TaxisPosition
        {
        	get
        	{
        		List<Point> res = new List<Point>();
        		foreach(Taxi t in this.m_Taxis)
        			res.Add(t.Position);
        		return res;
        	}
        }
        
        /// <summary> Returns a list of taxis that have room left </summary>
        private List<Taxi> NonFullTaxis()
        {
        	List<Taxi> res = new List<Taxi>();
        	foreach(Taxi t in m_Taxis)
        		if(t.hasRoomLeft())
        			res.Add(t);
        	
        	return res;
        }
        
        #endregion
        
        #region Manage the size of the city
        private EventHandler e_SizeCityChanged;
        public event EventHandler SizeCityChanged
        {
            add { e_SizeCityChanged += value; }
            remove { e_SizeCityChanged -= value; }
        }
        private void RaiseSizeCityChanged(object sender, EventArgs data)
        {
            if(e_SizeCityChanged != null)
                e_SizeCityChanged(sender, data);
        }
        /// <summary> Size of City in km </summary>
        public int SizeCity
        {
            get { return this.m_SizeCity; }
            set
            {
                if(value > 0)
                {
                    this.m_SizeCity = value;
                    this.RaiseSizeCityChanged(this, new EventArgs());
                }
            }
        }
        private int m_SizeCity;
        #endregion

        #region Clients Management
        /// <summary> Add a new client at the given position </summary>
        public void SpawnClient(Point position)
        {
        	Client c = new Client(this, position, m_random.CalculateClientDestination(position, m_SizeCity), m_random.CalculateClientLifeTime(FPS));
           	this.m_Clients.Add(c);
           
           	AI.AssignBestTaxiToCLient(c, NonFullTaxis());

            this.m_NumberOfClient++; RaiseNumberOfClientChanged(this, new EventArgs());
            this.m_NumberOfAwaiting++; RaiseNumberOfAwaitingChanged(this, new EventArgs());
        }
        
        /// <summary> Removes a client from the list and decreases the % accordingly </summary>
        public void ClientDied(object sender)
        {
        	Console.WriteLine("A client gave up...");
            this.m_Clients.Remove((Client)sender);
            
            this.m_NumberOfAwaiting--; RaiseNumberOfAwaitingChanged(this, new EventArgs());
            this.m_NumberOfUnsatisfied++; RaiseNumberOfUnsatisfiedChanged(this, new EventArgs());
        }
        
        public void ClientPleased(object sender)
        {
        	Console.WriteLine("A client reached his destination !");
        	this.m_Clients.Remove((Client)sender);
        	
        	this.m_NumberInsideTaxis--; RaiseNumberOfInsideTaxisChanged(this, new EventArgs());
        	this.m_NumberOfPleased++; RaiseNumberOfPleasedChanged(this, new EventArgs());
        }
        
        public void ClientPickedUp()
        {
        	Console.WriteLine("A taxi picked up a Client !");
        	
        	this.m_NumberInsideTaxis++; RaiseNumberOfInsideTaxisChanged(this, new EventArgs());
        	this.m_NumberOfAwaiting--; RaiseNumberOfAwaitingChanged(this, new EventArgs());
        }

        #region Clients accessors
        private List<Client> m_Clients;
        
        ///<return> List of all client positions </return>
        public List<Point> ClientPositions 
        {
            get 
            {
                List<Point> positions = new List<Point>();
                foreach(Client c in this.m_Clients)
                    positions.Add(c.Position);
                return positions;
            }
        }
        
        ///<return> List of all clients </return>
        public List<Client> Clients
        {
        	get
        	{
        		return m_Clients;
        	}
        }
        
        /// <returns> List of clients that have no taxis </returns>
        public List<Client> AwaitingClients	
        {
        	get
        	{
        		List<Client> res = new List<Client>();
        		foreach(Client c in m_Clients)
        			if(c.MyTaxi == null)
        				res.Add(c);
        		return res;
        	}
        }
        
        /// <summary> Returns the client the closest to p or null if none exists </summary>
        public Client getAwaitingClientClosestTo(Point p)
        {
        	Client res = null;
        	foreach(Client c in AwaitingClients)
        	{
        		if(res == null) res = c;
        		else if(Util.Distance(p, c.Position) < Util.Distance(p, res.Destination))
        				res = c;
        	}
        	return res;
        }
        
        /// <summary> Returns the awaiting client located at p, or null if it doesn't exist </summary>
        public Client getClientWaitingAtPosition(Point p)
        {
        	foreach(Client c in AwaitingClients)
				if(Util.Equivalent(c.Position, p))
					return c;
			return null;
        }
        #endregion
        
        #endregion
        
        #region City's variables
        
        // Manages the randomized aspects of the app
		private RandomMethods m_random;

        // Number of mins per seconds
        private double m_RatioTime;
        public double RatioTime { get { return this.m_RatioTime; } set { this.m_RatioTime = value; } }
        private DateTime m_Time;
        public DateTime Time { get { return m_Time; } set { m_Time = value; } }
        
        #region Client Numbers
        private int m_NumberOfClient;
        public int NumberOfClient { get { return this.m_NumberOfClient; } }

        private int m_NumberOfAwaiting;
        public int NumberOfAwaiting { get { return this.m_NumberOfAwaiting; } }
        
        private int m_NumberOfUnsatisfied;
        public int NumberOfUnsatisfied { get { return this.m_NumberOfUnsatisfied;  } }
        
        private int m_NumberOfPleased;
        public int NumberOfPleased { get { return this.m_NumberOfPleased; } }
        
        private int m_NumberInsideTaxis;
        public int NumberInsideTaxis { get { return this.m_NumberInsideTaxis; } }
        #endregion

        #endregion

        #region Events NumbersChanged
        private EventHandler e_NumberOfClientChanged;
        public event EventHandler NumberOfClientChanged
        {
            add { e_NumberOfClientChanged += value; }
            remove { e_NumberOfClientChanged -= value; }
        }
        private void RaiseNumberOfClientChanged(object sender, EventArgs data)
        {
            if(e_NumberOfClientChanged != null)
                e_NumberOfClientChanged(sender, data);
        }

        private EventHandler e_NumberOfAwaitingChanged;
        public event EventHandler NumberOfAwaitingChanged
        {
            add { e_NumberOfAwaitingChanged += value; }
            remove { e_NumberOfClientChanged -= value; }
        }
        private void RaiseNumberOfAwaitingChanged(object sender, EventArgs data)
        {
            if(e_NumberOfAwaitingChanged != null)
                e_NumberOfAwaitingChanged(sender, data);
        }

        private EventHandler e_NumberOfUnsatisfiedChanged;
        public event EventHandler NumberOfUnsatisfiedChanged
        {
            add { e_NumberOfUnsatisfiedChanged += value; }
            remove { e_NumberOfClientChanged -= value; }
        }
        private void RaiseNumberOfUnsatisfiedChanged(object sender, EventArgs data)
        {
            if(e_NumberOfUnsatisfiedChanged != null)
                e_NumberOfUnsatisfiedChanged(sender, data);
        }
        
        private EventHandler e_NumberOfPleasedChanged;
        public event EventHandler NumberOfPleasedChanged
        {
        	add { e_NumberOfPleasedChanged += value; }
        	remove { e_NumberOfPleasedChanged -= value; }
        }
        private void RaiseNumberOfPleasedChanged(object sender, EventArgs data)
        {
        	if(e_NumberOfPleasedChanged != null)
        		e_NumberOfPleasedChanged(sender, data);
        }
        
        private EventHandler e_NumberOfInsideTaxisChanged;
        public event EventHandler NumberOfInsideTaxisChanged
        {
        	add { e_NumberOfInsideTaxisChanged += value; }
        	remove { e_NumberOfInsideTaxisChanged -= value; }
        }
        private void RaiseNumberOfInsideTaxisChanged(object sender, EventArgs data)
        {
        	if(e_NumberOfInsideTaxisChanged != null)
        		e_NumberOfInsideTaxisChanged(sender, data);
        }
        #endregion
    };
}
