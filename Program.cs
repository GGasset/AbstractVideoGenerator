using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstractVideoGenerator
{
    internal static class Program
    {
        static void Main()
        {
            if (MessageBox.Show("Do you wish to train a network?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                NetworkTrainer.Program.Main(null);
            }

            RunApp();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void RunApp()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
