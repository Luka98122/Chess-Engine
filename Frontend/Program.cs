using StockfishV0;
using System;
using System.Windows.Forms;

namespace StockfishV0
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChessForm());
        }
    }
}