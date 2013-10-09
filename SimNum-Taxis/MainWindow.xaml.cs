using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimNum_Taxis
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime m_Time;
        private City m_City;
        public MainWindow()
        {
            InitializeComponent();
            // Calls the redraw function when the window size change
            this.SizeChanged += new SizeChangedEventHandler((object o, SizeChangedEventArgs e) 
                                    => { this.Dispatcher.BeginInvoke((Action)(() 
                                        => { this.ReDrawCanvas(); })); });
            this.m_Time = DateTime.Today;
            this.c_Time_TextBlock.Text = this.m_Time.Hour + "H" + this.m_Time.Minute;

            this.m_City = new City();
            
            #region Time Elapsed Event
            this.m_City.TimeElapsed += (object o, System.Timers.ElapsedEventArgs e) =>
            {
                this.m_Time = this.m_Time.AddMinutes(this.m_City.RatioTime);
                // Need to use the Dispatcher :  Allow a thread
                // (here Timer from City) to access the graphical part
                this.c_Time_TextBlock.Dispatcher.Invoke((Action)(
                    () => 
                    { 
                        this.c_Time_TextBlock.Text = 
                            String.Format("{0,2}H{1,2}",
                                this.m_Time.Hour, this.m_Time.Minute);
                    }));
            };
            #endregion
            
            #region TimeTick Event
            this.m_City.TimeTicked += (object o, System.Timers.ElapsedEventArgs e) =>
            {
            	 this.ReDrawCanvas();
            };
            #endregion
            
            
            this.c_SizeCity_TextBlock.Text = this.m_City.SizeCity.ToString();

            this.m_City.SizeCityChanged += (object sender, EventArgs e) =>
            #region Size City changed event
                {
                    this.c_SizeCity_TextBlock.Dispatcher.Invoke((Action)(
                        () => { this.c_SizeCity_TextBlock.Text = this.m_City.SizeCity.ToString(); }));
                    this.ReDrawCanvas();
                };
            #endregion

            #region Links events in city with board informations
            this.m_City.NumberOfClientChanged += new EventHandler(
                (object sender, EventArgs e) => { 
                    this.c_ClientsNumber_TextBlock.Dispatcher.BeginInvoke((Action)(
                        () => { this.c_ClientsNumber_TextBlock.Text = this.m_City.NumberOfClient.ToString(); }));
                    this.UpdatePercentageInformations();
                });
            this.m_City.CurrentNumberOfClientChanged += new EventHandler(
                (object sender, EventArgs e) => {
                    this.c_CurrentClientNumber_TextBlock.Dispatcher.BeginInvoke((Action)(
                        () => { this.c_CurrentClientNumber_TextBlock.Text = this.m_City.CurrentNumberOfClient.ToString();}));
                    this.UpdatePercentageInformations();
                });
            this.m_City.NumberOfUnsatisfiedChanged += new EventHandler(
                (object sender, EventArgs e) => { 
                    this.c_ClientLost_TextBlock.Dispatcher.BeginInvoke((Action)(
                        () => { this.c_ClientLost_TextBlock.Text = this.m_City.NumberOfUnsatisfied.ToString();}));
                    this.UpdatePercentageInformations();
                });
            #endregion

            // this.ReDrawCanvas();
        }

        private void UpdatePercentageInformations()
        {
            this.c_CurrentClientNumberPercent_TextBlock.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.c_CurrentClientNumberPercent_TextBlock.Text =
                    100*this.m_City.CurrentNumberOfClient/this.m_City.NumberOfClient + "%";
            }));
            this.c_ClientLostPercent_TextBlock.Dispatcher.BeginInvoke((Action)(() => 
            { 
                this.c_ClientLostPercent_TextBlock.Text = 
                    100*this.m_City.NumberOfUnsatisfied/this.m_City.NumberOfClient + "%"; 
            }));
        }
        
        private void ReDrawCanvas()
        {
            double height = this.c_City.ActualHeight,
                   width = this.c_City.ActualWidth;
           this.c_City.Dispatcher.BeginInvoke((Action)(() => 
           {
	          	this.c_City.Children.Clear();
	            Ellipse el = new Ellipse()
	            {
	                Width = 25*this.m_City.SizeCity, Height = 25*this.m_City.SizeCity,
	                StrokeThickness = 2.0, Stroke = Brushes.Black
	            };
	
	            Canvas.SetTop(el, height/2.0 - el.Height/2.0);
	            Canvas.SetLeft(el, width/2.0- el.Width/2.0);
	            this.c_City.Children.Add(el);
	
	            foreach(Point clientPosition in this.m_City.ClientPositions)
	            {
	            	TextBlock tb = new TextBlock() { Text = "C" };
	                Canvas.SetLeft(tb, getCanvasXMatching(clientPosition.X));
	                Canvas.SetTop(tb, getCanvasYMatching(clientPosition.Y));
	                this.c_City.Children.Add(tb);
	            }
	            
	            foreach(Point taxisPosition in this.m_City.TaxisPosition)
	            {
	            	TextBlock tb = new TextBlock() { Text = "T" };
	            	Canvas.SetLeft(tb, getCanvasXMatching(taxisPosition.X));
	            	Canvas.SetTop(tb, getCanvasYMatching(taxisPosition.Y));
	            	this.c_City.Children.Add(tb);
	            }
              })); // End Dispatcher calling
        }
        
        #region Methods giving the matching position of a Point for the Canvas to draw it  
        public double getCanvasXMatching(double x)
        {
        	double res = x;
        	res /= this.m_City.SizeCity * 1000;
			res *= 25*this.m_City.SizeCity/2;
  	      	res	+= this.c_City.ActualWidth/2;
        	
        	return res;
        }
        
        public double getCanvasYMatching(double y)
        {
        	double res = y;
        	res /= this.m_City.SizeCity * 1000;
			res *= 25*this.m_City.SizeCity/2;
  	      	res	+= this.c_City.ActualHeight/2;
        	
        	return res;
        }
        #endregion
        

        #region Manage the Faster Time Button
        private void c_FasterTime_Button_Click(object sender, RoutedEventArgs e)
        { this.m_City.RatioTime *= 10; }
        private void c_FasterTime_Button_Released(object sender, RoutedEventArgs e)
        { this.m_City.RatioTime /= 10; }
        #endregion

        #region Add or Remove a Taxi
        private void c_TaxisNumberMinus_Button_Click(object sender, RoutedEventArgs e)
        {
            this.m_City.RemovesTaxi();
            this.c_TaxisNumber_TextBlock.Text = this.m_City.NumberOfTaxis.ToString();
        }
        private void c_TaxisNumberPlus_Button_Click(object sender, RoutedEventArgs e)
        {
            this.m_City.AddsTaxi();
            this.c_TaxisNumber_TextBlock.Text = this.m_City.NumberOfTaxis.ToString();
        }
        #endregion

        #region Events that manage the size of the city
        private void c_SizeCityMinus_Button_Click(object sender, RoutedEventArgs e)
        { this.m_City.SizeCity--; }
        private void c_SizeCityPlus_Button_Click(object sender, RoutedEventArgs e)
        { this.m_City.SizeCity++; }
        #endregion

        /// <summary> Adds a client to the city at mouse position </summary>
        private void c_City_MouseDown(object sender, MouseButtonEventArgs e)
        {
        	Point p = e.GetPosition(this.c_City);
        	p.X -= this.c_City.ActualWidth/2;
        	p.Y -= this.c_City.ActualHeight/2;
        	// p € -actualSize .. actualSize
        	
    		p.X /= 25*this.m_City.SizeCity/2;
    		p.Y /= 25*this.m_City.SizeCity/2;
        	// p € -1 .. 1
        	
        	p.X *= this.m_City.SizeCity * 1000;
        	p.Y *= this.m_City.SizeCity * 1000;
        	// p € -10000 .. 10000
        	
            this.m_City.SpawnClient(p);
            this.UpdatePercentageInformations();
            this.ReDrawCanvas();
        }
    }
}
