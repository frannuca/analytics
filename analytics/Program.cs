using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numericsbase;
using MathNet.Numerics.LinearAlgebra;

namespace Analytics
{
    class Program
    {
        static int nsims = 30000;
        static int nassets = 10;
        static int ntimes = 250 * 10;
       
      
        static void Main(string[] args)
        {
            var scube = new simcube(nassets, nsims, ntimes);

            var builder = new montecarloBuild()
                .withSimulationCube(scube)
                .withcorrelationMatrix(Matrix<float>.Build.DiagonalIdentity(nassets))
                ;
            for (int i = 0; i < nassets; ++i) builder.withdrift(i, 0.05f).withvolatility(i, 0.15f);

            var montecarloengine = builder.build();
        }
    }
}
