//Author: Jordy A. Larrea Rodriguez (03/18/2021)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public class SpreadsheetApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int formCount = 0;

        // Singleton ApplicationContext
        private static SpreadsheetApplicationContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private SpreadsheetApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one DemoApplicationContext.
        /// </summary>
        public static SpreadsheetApplicationContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new SpreadsheetApplicationContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Spreadsheet1 spreadsheet)
        {
            // One more form is running
            formCount++;

            // When this form closes, we want to find out
            spreadsheet.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            spreadsheet.Show();
        }

    }
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
            // Start an application context and run one form inside it
            SpreadsheetApplicationContext spreadsheetAppContext = SpreadsheetApplicationContext.getAppContext();
            spreadsheetAppContext.RunForm(new Spreadsheet1());
            Application.Run(spreadsheetAppContext);
        }
    }
}
