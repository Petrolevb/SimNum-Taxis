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
    	public static int FPS = 40;
    	public static int INTERVAL = 1000 / FPS;
    	
        #region Constructor
        public City()
        {
            this.m_TimeInApplication = new System.Timers.Timer(1000);
            this.m_TimeInApplication.Start();
            this.m_timeTick = new System.Timers.Timer(INTERVAL);
            this.m_timeTick.Start();
            this.m_RatioTime = 2;
            this.m_Taxis = new List<Taxi>();
            this.m_Clients = new List<Client>();
            this.m_SizeCity = 10;
            this.m_NumberOfClient = 0;
            this.m_CurrentNumberOfClient = 0;
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
        /*	for(int i = 0; i < (int) RatioTime; i++)
        		if(m_random.TrySpawnClient())
	        		SpawnClient(m_random.CalculatePositionInCircle(m_SizeCity));
        /**/	

        	// Makes every taxi move
        	foreach(Taxi t in m_Taxis)
        		t.tick();

        	// Makes every client move
        	foreach(Client c in m_Clients)
        		c.tick();
        }
        
        private System.Timers.Timer m_timeTick;
        public event ElapsedEventHandler TimeTicked
        {
        	add { this.m_timeTick.Elapsed += value; }
        	remove { this.m_timeTick.Elapsed -= value; }
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
        	
        	this.m_Taxis.Add(new Taxi(m_random.CalculatePositionInCircle(m_SizeCity), speed, this));
        }
        /// <summary> Removes a Taxi </summary>
        public void RemovesTaxi() 
        { 
            if(this.m_Taxis.Count > 0) 
                this.m_Taxis.RemoveAt(this.NumberOfTaxis-1); 
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
        	Client c = new Client(this, position, m_random.CalculateClientDestination(m_SizeCity), m_random.CalculateClientLifeTime(FPS));
            this.m_Clients.Add(c);
            
            // TODO Choose the good taxi
            List<Taxi> tmp = NonFullTaxis();
            if(tmp.Count > 0)
            {
            	tmp[0].Target = c.Position;
            }

            this.m_NumberOfClient++; RaiseNumberOfClientChanged(this, new EventArgs());
            this.m_CurrentNumberOfClient++; RaiseCurrentNumberOfClientChanged(this, new EventArgs());
        }
        
        /// <summary> Removes a client from the list and decreases the % accordingly </summary>
        public void ClientDied(object sender)
        {
        	Console.WriteLine("A client gave up...");
            this.m_Clients.Remove((Client)sender);
            this.m_NumberOfUnsatisfied++; RaiseNumberOfUnsatisfiedChanged(this, new EventArgs());
            this.m_CurrentNumberOfClient--; RaiseCurrentNumberOfClientChanged(this, new EventArgs());
        }
        
        public void ClientPleased(object sender)
        {
        	Console.WriteLine("A client reached his destination !");
        	this.m_Clients.Remove((Client)sender);
        	this.m_NumberOfPleased++; RaiseNumberOfPleasedChanged(this, new EventArgs());
            this.m_CurrentNumberOfClient--; RaiseCurrentNumberOfClientChanged(this, new EventArgs());
        }

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
        
        /// <summary> Returns the client the closest to p or null if none exist </summary>
        public Client getAwaitingClientClosestTo(Point p)
        {
        	Client res = null;
        	foreach(Client c in AwaitingClients)
        	{
        		if(res==null) res = c;
        		else if(Util.Distance(p, c.Position) < Util.Distance(p, res.Destination))
        				res = c;
        	}
        	return res;
        }
        
        /// <summary> Returns the client located at p, or null if it doesn't exist </summary>
        public Client getClientWaitingAtPosition(Point p)
        {
        	foreach(Client c in AwaitingClients)
				if(Util.Equivalent(c.Position, p))
					return c;
			return null;
        }
        #endregion
        
        #region City's variables
        
        // Manages the randomized aspects of the app
		private RandomMethods m_random;

        // Number of mins per seconds
        private double m_RatioTime;
        public double RatioTime { get { return this.m_RatioTime; } set { this.m_RatioTime = value; } }
        
        #region Client Numbers
        private int m_NumberOfClient;
        public int NumberOfClient { get { return this.m_NumberOfClient; } }

        private int m_CurrentNumberOfClient;
        public int CurrentNumberOfClient { get { return this.m_CurrentNumberOfClient; } }
        
        private int m_NumberOfUnsatisfied;
        public int NumberOfUnsatisfied { get { return this.m_NumberOfUnsatisfied;  } }
        
        private int m_NumberOfPleased;
        public int NumberOfPleased { get { return this.m_NumberOfPleased; } }
        #endregion

        #endregion

        #region Events NumbersChanged
        private System.Timers.Timer m_TimeInApplication;
        /// <summary> Allow to register to the city timer </summary>
        public event ElapsedEventHandler TimeElapsed
        {
            add { this.m_TimeInApplication.Elapsed += value; }
            remove { this.m_TimeInApplication.Elapsed -= value; }
        }
        
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

        private EventHandler e_CurrentNumberOfClientChanged;
        public event EventHandler CurrentNumberOfClientChanged
        {
            add { e_NumberOfClientChanged += value; }
            remove { e_NumberOfClientChanged -= value; }
        }
        private void RaiseCurrentNumberOfClientChanged(object sender, EventArgs data)
        {
            if(e_CurrentNumberOfClientChanged != null)
                e_CurrentNumberOfClientChanged(sender, data);
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
        #endregion
    };
}
