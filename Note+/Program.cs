using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Windows.Forms;

namespace Note_
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            if (!IsUserAdministrator())
            {
                MessageBox.Show("Please run program as admin permission!");
                return;
            }
            SingleInstance.SingleApplication.Run(new NotePad());
            //Application.Run(new Form1());
        }
        public static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                var user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
    }
}
