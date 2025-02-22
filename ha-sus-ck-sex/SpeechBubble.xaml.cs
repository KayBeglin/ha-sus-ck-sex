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
using System.Windows.Threading;

namespace ha_sus_ck_sex
{
    /// <summary>
    /// Interaction logic for SpeechBubble.xaml
    /// </summary>
    public partial class SpeechBubble : Window
    {
        private String fullText = "Hello! I am TODD, your Tiny Organisational Digital Deputy!";
        private int charIndex = 0;
        private DispatcherTimer textTimer;

        public SpeechBubble(double[] pos)
        {
            InitializeComponent();
            this.Left = pos[0];
            this.Top = pos[1];
            StartTextAnimation();
        }

        private void StartTextAnimation ()
        {
            textTimer = new DispatcherTimer();
            textTimer.Interval = TimeSpan.FromMilliseconds(20); // Adjust the interval as needed
            textTimer.Tick += UpdateText;
            textTimer.Start();
        }

        private void UpdateText(object sender, EventArgs e)
        {
            if (charIndex < fullText.Length)
            {
                ChatBox.Text += fullText[charIndex];
                charIndex++;
            }
            else
            {
                textTimer.Stop();
                UserInputSpeechBubble userInputSpeechBubble = new UserInputSpeechBubble(new double[] { this.Left-50, this.Top + 200 });
                userInputSpeechBubble.Show();
            }
        }
    }
}
