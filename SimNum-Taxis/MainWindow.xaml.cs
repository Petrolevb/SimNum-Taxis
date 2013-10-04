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
            this.m_Time = DateTime.Today;
            this.c_Time_TextBlock.Text = this.m_Time.Hour + "H" + this.m_Time.Minute;

            this.m_City = new City();
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
            this.c_SizeCity_TextBlock.Text = this.m_City.SizeCity.ToString();
            this.m_City.SizeCityChanged += (object sender, EventArgs e) =>
                {
                    this.c_SizeCity_TextBlock.Dispatcher.Invoke((Action)(
                        () => { this.c_SizeCity_TextBlock.Text = this.m_City.SizeCity.ToString(); }));
                };
        }
        
        private void c_FasterTime_Button_Click(object sender, RoutedEventArgs e)
        {
            this.m_City.RatioTime *= 10;
        }
        private void c_FasterTime_Button_Released(object sender, RoutedEventArgs e)
        {
            this.m_City.RatioTime /= 10;
        }

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
    }
}
