using App.Read;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App.Logic
{
    internal class LogicFine
    {
        
        
        static public List<int> CalcFines(List<string[]> data, int reportDay)
        {
            List<int> Fines = new List<int>();
            for (int i = 0; i < data.Count; i++)
            {
                int LoanDays = 0;
                for (int j = 0; j < data[i].Length; j++)
                {
                    if (data[i][5] == "")
                        data[i][5] = reportDay.ToString();
                    LoanDays = int.Parse(data[i][5]) - int.Parse(data[i][4]);

                    if (LoanDays > 14)
                    {
                        Fines.Add(LoanDays - 14);
                    }
                    else
                        Fines.Add(0);
                }
            }
            return Fines;
        }

    }
}
