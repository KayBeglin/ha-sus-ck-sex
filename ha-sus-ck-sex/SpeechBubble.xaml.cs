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
using WpfAnimatedGif;

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
        private Image CatGif;
        private BitmapImage idleImage;
        private BitmapImage talkingImage;
        UserInputSpeechBubble userInputSpeechBubble;

        private Queue<String> textQueue = new Queue<String>();
        private bool textPlaying = false;

        public SpeechBubble(double[] pos,Image CatGif)
        {
            InitializeComponent();
            this.Left = pos[0];
            this.Top = pos[1];
            this.CatGif = CatGif;

            idleImage = new BitmapImage();
            idleImage.BeginInit();
            idleImage.UriSource = new Uri("pack://application:,,,/Resources/idle-cat.gif");
            idleImage.EndInit();

            talkingImage = new BitmapImage();
            talkingImage.BeginInit();
            talkingImage.UriSource = new Uri("pack://application:,,,/Resources/speaking-cat.gif");
            talkingImage.EndInit();

            userInputSpeechBubble = new UserInputSpeechBubble(new double[] { this.Left - 50, this.Top + 200 });

            StartTextAnimation(fullText);
        }

        private void StartTextAnimation(String FullText)
        {
            if (textPlaying)
            {
                textQueue.Enqueue(FullText);
                return;
            }
            textPlaying = true;
            ChatBox.Text = "";
            charIndex = 0;
            userInputSpeechBubble.Hide();
            this.fullText = FullText;
            textTimer = new DispatcherTimer();
            textTimer.Interval = TimeSpan.FromMilliseconds(20); // Adjust the interval as needed
            textTimer.Tick += UpdateText;
            textTimer.Start();
            ImageBehavior.SetAnimatedSource(CatGif, talkingImage);
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
                
                userInputSpeechBubble.Show();
                ImageBehavior.SetAnimatedSource(CatGif, idleImage);

                textPlaying = false;
                if (textQueue.Count > 0)
                {
                    StartTextAnimation(textQueue.Dequeue());
                }
            }
        }
    }
}
