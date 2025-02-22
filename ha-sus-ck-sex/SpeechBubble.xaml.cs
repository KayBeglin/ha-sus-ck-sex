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
    /// Interaction logic for SpeechBubble.xaml
    /// </summary>
    public partial class SpeechBubble : Window
    {
        public SpeechBubble(double[] pos)
        {
            InitializeComponent();
            this.Left = pos[0];
            this.Top = pos[1];
        }
    }
}
