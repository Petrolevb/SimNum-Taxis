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
        public MainWindow()
        {
            InitializeComponent();
            this.m_Time = DateTime.Today;
            this.c_Time_TextBlock.Text = this.m_Time.Hour + ":" + this.m_Time.Minute;
            City v = new City();
            v.TimeElapsed += (object o, System.Timers.ElapsedEventArgs e) =>
                {
                    this.m_Time = this.m_Time.AddMinutes(v.RatioTime);
                    // Need to use the Dispatcher : 
                    // Allow a thread (here Timer from City) 
                    // to access the graphical part
                    this.c_Time_TextBlock.Dispatcher.Invoke((Action)(()
                        => { this.c_Time_TextBlock.Text = this.m_Time.Hour + "H" + this.m_Time.Minute; }));
                };
        }

        private void c_FasterTime_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void c_TaxisNumberMinus_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void c_SizeCityMinus_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void c_TaxisNumberPlus_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void c_SizeCityPlus_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
