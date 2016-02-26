using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(2000);
                    // Get element under pointer. You can also get an AutomationElement from a
                    // HWND handle, or by navigating the UI tree.
                    var pt = Cursor.Position;
                    AutomationElement el = AutomationElement.FromPoint(new System.Windows.Point(pt.X, pt.Y));
                    // Prints its name - often the context, but would be corresponding label text for editable controls. Can also get the type of control, location, and other properties.
                    Console.WriteLine(el.Current.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
