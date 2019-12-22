using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace Bi_31_windows
{
    static class Program
    {
        public static int[,] mquad = new int[4, 4];
        public static int[] olkey=new int[16];
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AuthWin());
        }

        public static string Encode(string pass, int[] key)
        {
            string newpass="----------------";
            for (int i=0; i< pass.Length; i++)
            {
                if(Array.IndexOf(key, i) != - 1)
                {
                    newpass=newpass.Remove(Array.IndexOf(key, i), 1);
                    newpass=newpass.Insert(Array.IndexOf(key, i), pass[i].ToString());
                }
            }
            return newpass;
        }

        public static string Decode(string pass, int[] key)
        {
            string depass="";
            for (int i=0; i<16; i++)
            {
                if (Array.IndexOf(key, i) != -1)
                {
                    char buf = pass[Array.IndexOf(key, i)];
                    if (buf != '-')
                        depass = depass + buf;
                }
            }
            return depass;
        }

        public static int CheckSquare(int[] square)
        {
            for (int i = 0; i < 4; i++)
            {
                if ((square[i] + square[i + 4] + square[i + 8] + square[i + 12]) != 30)
                {
                    return -1;
                }
                if ((square[0 + (4 * i)] + square[1 + (4 * i)] + square[2 + (4 * i)] + square[3 + (4 * i)]) != 30)
                {
                    return -1;
                }
            }
            if ((square[0] + square[5] + square[10] + square[15]) != 30)
            {
                return -1;
            }
            if ((square[3] + square[6] + square[9] + square[12]) != 30)
            {
                return -1;
            }
            return 1;
        }
    }
}
