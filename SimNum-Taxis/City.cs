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
        /// <summary> Constructor </summary>
        public City()
        {
            this.m_TimeInApplication = new System.Timers.Timer(1000);
            this.m_TimeInApplication.Start();
            this.m_timeTick = new System.Timers.Timer(20);
            this.m_timeTick.Start();
            this.m_RatioTime = 2;
            this.m_Taxis = new List<Taxi>();
            this.m_Clients = new List<Client>();
            this.m_SizeCity = 10;
            this.m_NumberOfClient = 0;
            this.m_CurrentNumberOfClient = 0;
            this.m_NumberOfUnsatisfied = 0;
        }
        
        #region Random uniform distribution methods
        /// <summary> Chooses a random, evenly probable, position inside the city (circle) </summary>
        private Point CalculateInitialTaxiPos()
        {
    		Random r = new Random();
    		double u1 = r.NextDouble();
    		double u2 = r.NextDouble();

    		Point pos = new Point();    		
    		pos.X = Math.Sqrt(u2) * Math.Cos(2 * Math.PI * u1) * m_SizeCity * 1000;
    		pos.Y  = Math.Sqrt(u2) * Math.Sin(2 * Math.PI * u1) * m_SizeCity * 1000;

    		return pos;
        }
        
        // TODO Gaussian random picker
        /// <summary> Chooses a destination inside the city </summary>
        private Point CalculateClientDestination()
        {
        	Random r = new Random();
    		double u1 = r.NextDouble();
    		double u2 = r.NextDouble();

    		Point pos = new Point();    		
    		pos.X = Math.Sqrt(u2) * Math.Cos(2 * Math.PI * u1) * m_SizeCity * 1000;
    		pos.Y  = Math.Sqrt(u2) * Math.Sin(2 * Math.PI * u1) * m_SizeCity * 1000;

    		return pos;
        }        
        #endregion
        
        #region Game ticks loop. There is 50 ticks per second.
        public void tick()
        {
        	foreach(Taxi t in m_Taxis)
        		t.move();
        }
        
        #endregion
        

        #region Number of Taxis
        /// <summary> Adds a new Taxi </summary>
        public void AddsTaxi() 
        {
        	// Speed of the taxi
        	double  speed = 50; 		  // 50 km/h
	    			speed *= 1000 / 60;   // 833.33 m/min
	    			speed /= 50;	 	  // 16.66 m/min every tick
        	
        	this.m_Taxis.Add(new Taxi(CalculateInitialTaxiPos(), speed, this));
        }
        /// <summary> Removes a new Taxi </summary>
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

        #region Client Management
        /// <summary>
        /// Add a new client at the given position
        /// Has to be private (or at least protected)
        /// </summary>
        /// <param name="position">Position of the new client</param>
        public void SpawnClient(Point position)
        {
            Client c = new Client(position);
            c.Destination = CalculateClientDestination();
            this.m_Clients.Add(c);
            System.Threading.Timer t = new System.Threading.Timer(new TimerCallback(clientDied), c, 5000, Timeout.Infinite);

            this.m_NumberOfClient++; RaiseNumberOfClientChanged(this, new EventArgs());
            this.m_CurrentNumberOfClient++; RaiseCurrentNumberOfClientChanged(this, new EventArgs());
        }
        
        private void clientDied(object sender)
        {
            this.m_Clients.Remove((Client)sender);
            this.m_NumberOfUnsatisfied++; RaiseNumberOfUnsatisfiedChanged(this, new EventArgs());
            this.m_CurrentNumberOfClient--; RaiseCurrentNumberOfClientChanged(this, new EventArgs());
        }

        private List<Client> m_Clients;
        ///<return>List of all client position</return>
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
        
        ///<return>List of all clients</return>
        public List<Client> Clients
        {
        	get
        	{
        		return m_Clients;
        	}
        }
        #endregion
        
        #region Ticks timer management
        public event ElapsedEventHandler TimeTicked
        {
        	add { this.m_timeTick.Elapsed += value; }
        	remove { this.m_timeTick.Elapsed -= value; }
        }
        private System.Timers.Timer m_timeTick;
        #endregion
        

        /// <summary> Allow to register to the city timer </summary>
        public event ElapsedEventHandler TimeElapsed
        {
            add { this.m_TimeInApplication.Elapsed += value; }
            remove { this.m_TimeInApplication.Elapsed -= value; }
        }
        private System.Timers.Timer m_TimeInApplication;

        // Number of min. per seconds
        private double m_RatioTime;
        public double RatioTime { get { return this.m_RatioTime; } set { this.m_RatioTime = value; } }

        #region Client Numbers
        public int NumberOfClient { get { return this.m_NumberOfClient; } }
        private int m_NumberOfClient;
        public int CurrentNumberOfClient { get { return this.m_CurrentNumberOfClient; } }
        private int m_CurrentNumberOfClient;
        public int NumberOfUnsatisfied { get { return this.m_NumberOfUnsatisfied;  } }
        private int m_NumberOfUnsatisfied;
        #endregion

        #region events NumbersChanged
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
        #endregion
    };
}
