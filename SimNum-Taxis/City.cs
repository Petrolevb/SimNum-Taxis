using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace SimNum_Taxis
{
    class City
    {
        /// <summary> Ctor </summary>
        public City()
        {
            this.m_TimeInApplication = new Timer(1000);
            this.m_TimeInApplication.Start();
            this.m_RatioTime = 5;
            this.m_Taxis = new List<Taxi>();
            this.m_Clients = new List<Client>();
            this.m_SizeCity = 10;
        }

        #region Number of Taxis
        /// <summary> Adds a new Taxi </summary>
        public void AddsTaxi() 
        { 
            this.m_Taxis.Add(new Taxi()); 
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

        /// <summary>
        /// Add a new client at the given position
        /// Has to be private (or at least protected)
        /// </summary>
        /// <param name="position">Position of the new client</param>
        public void SpawnClient(Point position)
        {
            this.m_Clients.Add(new Client(position));
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

        /// <summary> Allow to register to the city timer </summary>
        public event ElapsedEventHandler TimeElapsed
        {
            add { this.m_TimeInApplication.Elapsed += value; }
            remove { this.m_TimeInApplication.Elapsed -= value; }
        }
        private Timer m_TimeInApplication;

        // Number of min. per seconds
        private double m_RatioTime;
        public double RatioTime { get { return this.m_RatioTime; } set { this.m_RatioTime = value; } }
    };
}
