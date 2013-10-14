using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
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
        
        // debug variable
        public static int a = 0;
        
        public MainWindow()
        {
            InitializeComponent();
            
            // Changes the speed of the application when item is selected
            c_ComboBox.SelectionChanged += c_SpeedComboBoxChanged;
            
            // Calls the redraw function when the window size change
            this.SizeChanged += new SizeChangedEventHandler((object o, SizeChangedEventArgs e) 
                                    => { this.Dispatcher.BeginInvoke((Action)(() 
                                        => { this.ReDrawCanvas(); })); });
            this.m_Time = DateTime.Today;
            this.c_Time_TextBlock.Text = this.m_Time.Hour + "H" + this.m_Time.Minute;
            this.m_City = new City();
            
            #region Time events
            this.m_City.TimeElapsed += (object o, System.Timers.ElapsedEventArgs e) =>
            {
            	a = 0;
            	
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
            
            this.m_City.TimeTicked += (object o, System.Timers.ElapsedEventArgs e) =>
            {
            	//Console.WriteLine(a++);
            	this.m_City.gameTick();
				this.ReDrawCanvas();
            };
            #endregion
            
            #region Size City changed event
            this.c_SizeCity_TextBlock.Text = this.m_City.SizeCity.ToString();
            this.m_City.SizeCityChanged += (object sender, EventArgs e) =>
            
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
            this.m_City.NumberOfAwaitingChanged += new EventHandler(
                (object sender, EventArgs e) => {
                    this.c_ClientAwaiting_TextBlock.Dispatcher.BeginInvoke((Action)(
                        () => { this.c_ClientAwaiting_TextBlock.Text = this.m_City.NumberOfAwaiting.ToString();}));
                    this.UpdatePercentageInformations();
                });
            this.m_City.NumberOfUnsatisfiedChanged += new EventHandler(
                (object sender, EventArgs e) => { 
                    this.c_ClientLost_TextBlock.Dispatcher.BeginInvoke((Action)(
                        () => { this.c_ClientLost_TextBlock.Text = this.m_City.NumberOfUnsatisfied.ToString();}));
                    this.UpdatePercentageInformations();
                });
            this.m_City.NumberOfPleasedChanged += new EventHandler(
                (object sender, EventArgs e) => { 
                    this.c_ClientManaged_TextBlock.Dispatcher.BeginInvoke((Action)(
                        () => { this.c_ClientManaged_TextBlock.Text = this.m_City.NumberOfPleased.ToString();}));
                    this.UpdatePercentageInformations();
                });
            
            this.m_City.NumberOfInsideTaxisChanged += new EventHandler(
                (object sender, EventArgs e) => { 
                    this.c_ClientsInsideTaxiPercent_TextBlock.Dispatcher.BeginInvoke((Action)(
                        () => { this.c_ClientsInsideTaxis_TextBlock.Text = this.m_City.NumberInsideTaxis.ToString();}));
                    this.UpdatePercentageInformations();
                });
            #endregion
        }

        #region Displays statistics
        private void UpdatePercentageInformations()
        {
            this.c_ClientAwaitingPercent_TextBlock.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.c_ClientAwaitingPercent_TextBlock.Text =
                	100*Math.Round((double) this.m_City.NumberOfAwaiting/this.m_City.NumberOfClient, 4) + "%";
            }));
            this.c_ClientLostPercent_TextBlock.Dispatcher.BeginInvoke((Action)(() => 
            { 
                this.c_ClientLostPercent_TextBlock.Text =
                    100*Math.Round((double) this.m_City.NumberOfUnsatisfied/this.m_City.NumberOfClient, 4) + "%"; 
            }));
        	this.c_ClientManagedPercent_TextBlock.Dispatcher.BeginInvoke((Action)(() => 
            { 
                this.c_ClientManagedPercent_TextBlock.Text =
                    100*Math.Round((double) this.m_City.NumberOfPleased/this.m_City.NumberOfClient, 4) + "%"; 
            }));
        	this.c_ClientsInsideTaxiPercent_TextBlock.Dispatcher.BeginInvoke((Action)(() => 
            { 
                this.c_ClientsInsideTaxiPercent_TextBlock.Text =
                    100*Math.Round((double) this.m_City.NumberInsideTaxis/this.m_City.NumberOfClient, 4) + "%"; 
            }));
        }
        #endregion
        
        #region City's content drawing functions
        /// <summary> Main city drawing method </summary>
        private void ReDrawCanvas()
        {
            double width = this.c_City.ActualWidth,
            	   height = this.c_City.ActualHeight;
           this.c_City.Dispatcher.BeginInvoke((Action)(() => 
           {
	          	this.c_City.Children.Clear();
	          	
				#region City display
				int diameter = (int) Math.Min(width, height);
	            Ellipse el = new Ellipse()
	            {
	            	Fill = new RadialGradientBrush(Colors.LightGray, Colors.DarkGray),
	                Width = diameter, Height = diameter,
	                StrokeThickness = 2.0, Stroke = Brushes.Black
	            };
	            addShapeToCanvas(el, width/2.0 - el.Width/2.0, height/2.0 - el.Height/2.0);
				#endregion
	            
	            #region Taxis display
	            foreach(Point taxisPosition in this.m_City.TaxisPosition)
	            {
	            	int size = 10;
	            	Rectangle r = new Rectangle()
	            	{
	            		Fill = new SolidColorBrush(Colors.Yellow),
	            		Width = size, Height = size,
	            		StrokeThickness = 1.0, Stroke = Brushes.Black
	            	};
	            	addShapeToCanvas(r, getCanvasXMatching(taxisPosition.X) - size/2, getCanvasYMatching(taxisPosition.Y) - size/2);
	            }
	            #endregion
	            
	            #region Clients display
	            foreach(Client c in m_City.Clients)
	            {
	            	// Clients looks smaller when inside a taxi
	            	int size = (c.MyTaxi==null?10:5);
	            	Ellipse e = new Ellipse()
	            	{
	            		Fill = new SolidColorBrush(c.Color),
	            		Width = size, Height = size,
	            		StrokeThickness = 1.0, Stroke = Brushes.Black
	            	};
	            	addShapeToCanvas(e, getCanvasXMatching(c.Position.X) - size/2, getCanvasYMatching(c.Position.Y) - size/2);

	            	// Cross for client's destination
	            	size = 10;
	            	Line l1 = new Line() { StrokeThickness = 1.0, Stroke = new SolidColorBrush(c.Color), X1 = 0 , X2 = size, Y1 = 0, Y2 = size };
	            	Line l2 = new Line() { StrokeThickness = 1.0, Stroke = new SolidColorBrush(c.Color), X1 = size , X2 = 0, Y1 = 0, Y2 = size };
	            	addShapeToCanvas(l1, getCanvasXMatching(c.Destination.X) - size/2, getCanvasYMatching(c.Destination.Y) - size/2);
	            	addShapeToCanvas(l2, getCanvasXMatching(c.Destination.X) - size/2, getCanvasYMatching(c.Destination.Y) - size/2);
	            }
	            #endregion
              })); // End Dispatcher calling
        }
        
         /// <summary> This methods adds a shape to the canvas at the given position </summary>
        private void addShapeToCanvas(Shape sh, double left, double top)
        {
        	Canvas.SetLeft(sh, left);
        	Canvas.SetTop(sh, top);
        	this.c_City.Children.Add(sh);
        }
        
        #region Methods giving the matching position of a Point for the Canvas to draw it. eg : x = 7000. It must return sthing like 400
        public double getCanvasXMatching(double x)
        {
        	double res = x;
        	res /= this.m_City.SizeCity * 1000;
        	res *=  ((int) Math.Min(this.c_City.ActualWidth, this.c_City.ActualHeight)) /2;
  	      	res	+= this.c_City.ActualWidth/2;
        	
        	return res;
        }
        
        public double getCanvasYMatching(double y)
        {
        	double res = y;
        	res /= this.m_City.SizeCity * 1000;
			res *= ((int) Math.Min(this.c_City.ActualWidth, this.c_City.ActualHeight)) /2;
  	      	res	+= this.c_City.ActualHeight/2;
        	
        	return res;
        }
        #endregion
        
        #endregion
        
        #region Combo Box setting the speed ratio
        private void c_SpeedComboBoxChanged(object sender, RoutedEventArgs e)
        {        	
        	String s = ((ComboBox) sender).SelectedItem.ToString();
        	String subs = s.Split(new Char[] {'x', ' '})[2];
        	
        	Console.WriteLine(s);
        	Console.WriteLine(subs);
        	
        	this.m_City.RatioTime = int.Parse(subs);
        }
		#endregion

        #region Buttons to add or remove a Taxi
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

        #region Buttons that manage the size of the city
        private void c_SizeCityMinus_Button_Click(object sender, RoutedEventArgs e)
        { this.m_City.SizeCity--; }
        private void c_SizeCityPlus_Button_Click(object sender, RoutedEventArgs e)
        { this.m_City.SizeCity++; }
        #endregion

        #region MouseClickListener that adds clients when Canvas is clicked
        /// <summary> Adds a client to the city at mouse position </summary>
        private void c_City_MouseDown(object sender, MouseButtonEventArgs e)
        {
        	Point p = e.GetPosition(this.c_City);
        	p.X -= this.c_City.ActualWidth/2;
        	p.Y -= this.c_City.ActualHeight/2;
        	// p € -actualSize .. actualSize      p = mousePosition in Canvas
        	
    		p.X /= ((int) Math.Min(this.c_City.ActualWidth, this.c_City.ActualHeight)) /2;
    		p.Y /= ((int) Math.Min(this.c_City.ActualWidth, this.c_City.ActualHeight)) /2;
        	// p € -1 .. 1
        	
        	p.X *= this.m_City.SizeCity * 1000;
        	p.Y *= this.m_City.SizeCity * 1000;
        	// p € -10000 .. 10000         if r = 10...
        	
            this.m_City.SpawnClient(p);
        }
        #endregion
    }
}
