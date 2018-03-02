using System;
using System.Collections.Generic;
using System.Linq;

namespace BeartrkrClient
{
    class ProcessList
    {
        List<ProcessItem> internalProcs = new List<ProcessItem>();

        public void AddOrIncrement(string name)
        {
            try
            {
                if ((from p in internalProcs where p.Name == name select p).Any()) (from p in internalProcs where p.Name == name select p).First().IncTime();
                else internalProcs.Add(new ProcessItem(name, 1));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in AddOrIncrement: " + ex.Message);
            }
        }

        public void AddOrIncrement(string name, bool web)
        {
            try
            {
                if (!web) AddOrIncrement(name);
                else
                {
                    if ((from p in internalProcs where p.Name == name select p).Any()) (from p in internalProcs where p.Name == name select p).Single().SetWebTime();
                    else internalProcs.Add(new ProcessItem(name, 60, true));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in AddOrIncrement: " + ex.Message);
            }
        }

        public void Clear()
        {
            internalProcs.Clear();
        }

        public ProcessItem[] GetProcesses()
        {
            return internalProcs.ToArray();
        }
    }
}
