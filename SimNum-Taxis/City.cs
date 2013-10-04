using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
