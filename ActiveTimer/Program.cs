using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Runtime.InteropServices;

namespace TestFormsApp
{
    static class Program
    {
        private static System.Timers.Timer aTimer;
        private static DateTime oldTime;
        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);
        static uint GetLastInputTime()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            return ((idleTime > 0) ? (idleTime / 1000) : 0);
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //If the last input time is 5 minutes then they went for a walk
            if (GetLastInputTime() > 300)
            {
                oldTime = DateTime.Now;
            }
            else
            {
                TimeSpan timeDIff = DateTime.Now.Subtract(oldTime);
                if (timeDIff.TotalMinutes > 30)
                {
                    oldTime = DateTime.Now;
                    MessageBox.Show("Time to get up and walk around!");
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            aTimer = new System.Timers.Timer(10000); //Every minute
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 60000;
            aTimer.Enabled = true;
            oldTime = DateTime.Now;
            Application.Run(new ActiveTimer());
        }
    }
}
