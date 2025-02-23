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
using System.Windows.Shapes;

namespace ha_sus_ck_sex
{
    /// <summary>
    /// Interaction logic for UserInputSpeechBubble.xaml
    /// </summary>
    public partial class UserInputSpeechBubble : Window
    {
        SpeechBubble speechBubble;
        WeatherHandler weatherHandler;
        public UserInputSpeechBubble(double[] pos, SpeechBubble speechBubble)
        {
            InitializeComponent();
            this.Left = pos[0];
            this.Top = pos[1];
            this.speechBubble = speechBubble;
            weatherHandler = new WeatherHandler(speechBubble);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.KeyUpEvent,
                new System.Windows.Input.KeyEventHandler(TextBox_KeyUp));

            base.OnContentRendered(e);
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = (TextBox)sender;
                string input = textBox.Text;
                textBox.Text = "";
                // Do something with the input
                //Console.WriteLine(input);
                string weatherPhrase = "What's the weather in ";
                if (input.StartsWith(weatherPhrase)){
                   weatherHandler.OutputData(input.Substring(weatherPhrase.Length));
                }
            }
        }
    }
}
