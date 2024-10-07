using Komotray;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace piratenbar
{
    public partial class MainForm : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern bool DeleteObject(System.IntPtr hObject);

        // The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
        // Copied from dwmapi.h
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        // The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        // what value of the enum to set.
        // Copied from dwmapi.h
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        // Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);
        Label lbl1 = new Label();
        Label lbl2 = new Label();
        Label lbl3 = new Label();

        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        public MainForm()
        {
            InitializeComponent();
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            this.Height = 16;
            this.BackColor = Color.FromArgb(255, 0, 0, 0);


            BlurHelper.EnableAcrylicBlur(this.Handle, 64, Color.FromArgb(0x333333));


            var displays = DisplayDevices.GetAll(1);
            ;

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Font = new Font("FiraCode Nerd Font", 10);
            this.Height = 24;

            lbl1.Text = "foo";
            lbl1.AutoSize = false;
            lbl1.Top = 0;
            lbl1.Left = 0;
            lbl1.TextAlign = ContentAlignment.MiddleLeft;
            lbl1.ForeColor = Color.LightGray;
            this.Controls.Add(lbl1);


            lbl2.Text = "\ue0c0";
            lbl2.AutoSize = false;
            lbl2.Top = lbl1.Top;
            lbl2.Left = this.Width - 2;
            lbl2.TextAlign = ContentAlignment.MiddleCenter;
            lbl2.ForeColor = lbl1.ForeColor;
            this.Controls.Add(lbl2);

            lbl3.Text = "\ue0c0";
            lbl3.AutoSize = false;
            lbl3.Top = lbl1.Top;
            lbl3.Left = this.Width - 2;
            lbl3.TextAlign = ContentAlignment.MiddleRight;
            lbl3.ForeColor = lbl1.ForeColor;
            this.Controls.Add(lbl3);

            //this.Height = lbl1.Top + lbl1.Height + 2;

            // Setup
            AppBarHelper.AppBarMessage = "piratenbar";

            // Make the window an AppBar and dock to the right of the screen:
            AppBarHelper.SetAppBar(this, AppBarEdge.Top, 2);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Win32Helper.GetAccentColor(), 1.0f), this.ClientRectangle.Left, this.ClientRectangle.Height - 1, this.ClientRectangle.Width, this.ClientRectangle.Height - 1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //lbl2.Text = " " + DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();
            lbl1.Text = "󰝚 " + GetSpotifyTrackInfo();
            lbl2.Text = Environment.UserName.ToLower() + "@" + Environment.MachineName.ToLower();
            lbl3.Text = " " + ((int)cpuCounter.NextValue()).ToString().PadLeft(2) + "% " + (ramCounter.NextValue()).ToString().PadLeft(6) + "MB";

            lbl1.Width = this.Width / 3;
            lbl2.Width = this.Width / 3;
            lbl3.Width = this.Width / 3;

            lbl2.Left = lbl1.Width + lbl1.Left + 2;
            lbl3.Left = this.Width - lbl2.Width - 2;

        }

        public string GetSpotifyTrackInfo()
        {
            var proc = Process.GetProcessesByName("Spotify").FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle));

            if (proc == null)
            {
                return "Spotify is not running!";
            }

            if (string.Equals(proc.MainWindowTitle, "Spotify", StringComparison.InvariantCultureIgnoreCase))
            {
                return "No track is playing";
            }

            return proc.MainWindowTitle;
        }
    }
}
