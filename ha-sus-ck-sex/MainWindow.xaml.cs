using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using static ha_sus_ck_sex.WeatherHandler;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace ha_sus_ck_sex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Get the primary screen's working area
            var screenWidth = SystemParameters.WorkArea.Width;
            var screenHeight = SystemParameters.WorkArea.Height;

            // Set the window's position
            this.Left = screenWidth - this.Width - 20;
            this.Top = screenHeight - this.Height - 20;

        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            SpeechBubble speechBubble = new SpeechBubble(new double[] { this.Left - 80, this.Top - 250 }, GifBackground);
            speechBubble.Show();
        }
    }
}
        
    

