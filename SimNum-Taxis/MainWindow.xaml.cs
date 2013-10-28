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
    public partial class MainWindow : Window
    {
        private City m_City;
        private System.Threading.Thread m_thread;
        private bool isRunning = false;

        // debug variable
        public static int a = 0;
        
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            this.m_City = new City();
            
            // Changes the speed of the application when item is selected
            c_ComboBox.SelectionChanged += c_SpeedComboBoxChanged;
            
            // Calls the redraw function when the window size change
            this.SizeChanged += new SizeChangedEventHandler((object o, SizeChangedEventArgs e) 
                                    => { this.Dispatcher.BeginInvoke((Action)(() 
                                        => { this.ReDrawCanvas(); })); });
            this.m_City.Time = DateTime.Today;
            this.c_Time_TextBlock.Text = this.m_City.Time.Hour + "H" + this.m_City.Time.Minute;
            
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
            
            this.m_thread = new System.Threading.Thread(this.run);
            if(isRunning == false)
            {	
            	isRunning = true;
            	this.m_thread.Start();
            }
        }
        #endregion
        
        #region Thread functions
        /// <summary> Executes FPS gameTicks per second and tries to render FPS times per second. </summary>
        private void run()
        {
			double secondsPerTick = 1 / ((double) City.FPS);
			int ticks = 0;
			int fps = 0;
			double delta = 0;
			long lastTime = DateTime.Now.ToFileTime();

			while(isRunning)
			{
				long now = DateTime.Now.ToFileTime();
				long elapsedTime = now - lastTime;
				lastTime = now;
	
				if(elapsedTime < 0) elapsedTime = 0;
				if(elapsedTime > 10000000) elapsedTime = 10000000;
	
				delta += elapsedTime / 10000000.0;
				bool ticked = false;
	
				while(delta > secondsPerTick)
				{
					tick();
					ticks++;
					delta -= secondsPerTick;
					ticked = true;
	
					if(ticks % City.FPS == 0)
					{
						Console.WriteLine("fps : " + fps);
						lastTime += 1000;
						fps = 0;
					}
				}
	
				if(ticked)
				{
					render();
					fps++;
				}
				else
					System.Threading.Thread.Sleep(1);
			}
			m_thread.Join();
        }        
        
        /// <summary> Is executed FPS times per second. </summary>
        private void tick()
        {
        	//Console.WriteLine(a++);
            this.m_City.gameTick();

            this.m_City.Time = this.m_City.Time.AddMinutes(this.m_City.RatioTime / City.FPS);
            // We need to use the Dispatcher :  Allows a thread
            // (here Timer from City) to access the graphical part
            this.c_Time_TextBlock.Dispatcher.Invoke((Action)(() => 
            {
            	this.c_Time_TextBlock.Text = String.Format("{0,2}H{1,2}", this.m_City.Time.Hour, this.m_City.Time.Minute);
            }));
        }
        
        /// <summary> Is executed every time the game has ticked at least once. </summary>
        private void render()
        {
        	this.ReDrawCanvas();
        }
        #endregion
        
        #region Action performed when the window is terminated
        /// <summary> Exits the program and joins the thread when quited. </summary>
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			isRunning = false;
			System.Environment.Exit(0);
		}
		#endregion

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
	            	int size = 12;
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
	            	int size = (c.MyTaxi==null?10:6);
	            	Ellipse e = new Ellipse()
	            	{
	            		Fill = new SolidColorBrush(c.Color),
	            		Width = size, Height = size,
	            		StrokeThickness = 1.0, Stroke = Brushes.Black
	            	};
	            	int inTaxi = c.placeInTaxi() * 2;
	            	addShapeToCanvas(e, getCanvasXMatching(c.Position.X) - size/2 + inTaxi, getCanvasYMatching(c.Position.Y) - size/2 + inTaxi);

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
