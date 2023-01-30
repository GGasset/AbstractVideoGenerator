using NeatNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractVideoGenerator
{
    public static class Program
    {
        public static void Main()
        {
            RunApp();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void RunApp()
        {
            Application.Run(new MainForm());
        }

        [STAThread]
        public static void PrepareUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
