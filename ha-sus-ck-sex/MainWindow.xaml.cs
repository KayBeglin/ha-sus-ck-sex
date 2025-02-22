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
    }
}
