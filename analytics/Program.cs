using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numericsbase.montecarlo;
using MathNet.Numerics.LinearAlgebra;
using numericsbase.utils;
using System.IO;

namespace Analytics
{
    class Program
    {
        static UInt64 nsims = 100000;
        static UInt64 nassets = 10;
        static UInt64 ntimes = 30;

       
        static void Main(string[] args)
        {

        
            Node<int> root = new Node<int>(null);

            root.data = 1;
            root.children.Add(new Node<int>(root));
            root.children.Last().data = 2;

            root.children.First().children.Add(new Node<int>(root.children.First()));
            root.children.First().children.Last().data = 3;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Node<int>));
            StringWriter stringwriter = new StringWriter();

            x.Serialize(stringwriter, root);
            var xmlstr = stringwriter.ToString();


            var a = x.Deserialize(new StringReader(xmlstr));


            double szsim = nsims * nassets * ntimes * sizeof(double)/1024/1024;//MB


            long s1 = GC.GetTotalMemory(false);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine(String.Format("Creating first simulation cube with dimension {0} MB, {1} number of samples", szsim,nsims*nassets*ntimes));
            var scube1 = new SimulationCube(nsims,nassets,ntimes, SimulationCube.DISTRIBUTION.NORMAL);
            long s2 = GC.GetTotalMemory(false);
            Console.WriteLine(String.Format("Creation lasted {0}ms", watch.ElapsedMilliseconds));

            watch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine(String.Format("Creating second simulation cube with dimension {0} MB, {1} number of samples", szsim,  nsims * nassets * ntimes));
            var scube2 = new SimulationCube(nsims, nassets, ntimes,SimulationCube.DISTRIBUTION.NORMAL);
            Console.WriteLine(String.Format("Creation lasted {0}ms", watch.ElapsedMilliseconds));

            Console.WriteLine("Object memory footprints cube1={0} MB", (s2-s1)/1024/1024);

            Console.WriteLine("Checking values");
            bool equal = scube1 == scube2;
            if (!equal)
                Console.WriteLine("Objects are different");
            else
                Console.WriteLine("Objects are the same");


            Console.WriteLine("Correlation of the simulaiton cube");
            watch = new System.Diagnostics.Stopwatch();
            Matrix<double> rho = Matrix<double>.Build.Dense((int)nassets, (int)nassets);
            for(int i=0;i<rho.RowCount;++i)
            {
                rho[i, i] = 1.0;              
            }
            scube1.CorrelateSimulationCube(rho);

            equal = scube1 == scube2;
            if (!equal)
                Console.WriteLine("ERROR: Objects are different after correlating with identity correlation matrix");
            else
                Console.WriteLine("OK: Objects are the same after correlating with identity matrix");


            for (int i = 0; i < rho.RowCount; ++i)
            {
                rho[i, i] = 1.0;
                for (int j = i + 1; j < rho.RowCount; ++j)
                {
                    rho[i, j] = 0.25;
                    rho[j, i] = 0.25;
                }
            }

            scube1.CorrelateSimulationCube(rho);

            equal = scube1 == scube2;
            if (!equal)
                Console.WriteLine("OK, Objects are different after correlating");
            else
                Console.WriteLine("ERROR: Objects are the same after correlating");

        }
    }
}
