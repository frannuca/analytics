using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numericsbase.montecarlo;
using MathNet.Numerics.LinearAlgebra;

namespace Analytics
{
    class Program
    {
        static UInt64 nsims = 1000000;
        static UInt64 nassets = 5;
        static UInt64 ntimes = 260;
       
      
        static void Main(string[] args)
        {

            double szsim = nsims * nassets * ntimes * sizeof(double)/1024/1024;//MB


            long s1 = GC.GetTotalMemory(false);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine(String.Format("Creating first simulation cube with dimension {0} MB, {1} number of samples", szsim,nsims*nassets*ntimes));
            var scube1 = new SimulationCube(nsims,nassets,ntimes);
            long s2 = GC.GetTotalMemory(false);
            Console.WriteLine(String.Format("Creation lasted {0}ms", watch.ElapsedMilliseconds));

            watch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine(String.Format("Creating second simulation cube with dimension {0} MB, {1} number of samples", szsim,  nsims * nassets * ntimes));
            var scube2 = new SimulationCube(nsims, nassets, ntimes);
            Console.WriteLine(String.Format("Creation lasted {0}ms", watch.ElapsedMilliseconds));

            Console.WriteLine("Object memory footprints cube1={0} MB", (s2-s1)/1024/1024);

            Console.WriteLine("Checking values");
            bool equal = scube1 == scube2;
            if (!equal)
                Console.WriteLine("Objects are different");
            else
                Console.WriteLine("Objects are the same");
        }
    }
}
