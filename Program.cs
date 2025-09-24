using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
// Models & security classes are embedded in FormLogin.cs now

namespace QuanLyNhanVien
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Seeding admin account handled inside FormLogin constructor
            UserSession session = null;
            using (var login = new FormLogin())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    session = login.Session;
                }
            }

            if (session == null)
            {
                // User cancelled login
                return;
            }

            Application.Run(new Form1(session));
        }
    }
}
