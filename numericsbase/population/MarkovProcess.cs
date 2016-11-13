using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace numericsbase.population
{
    public class MarkovProcess
    {
        public MarkovProcess(Matrix<double> initialConditionMatrix,Matrix<double> Ptrans)
        {
            initialConditionMatrix.CopyTo(Mo);
            Ptrans.CopyTo(P);
            Ptrans.CopyTo(P_n);
            niters=0;
        }


        public MarkovProcess  next(int iterationnumber)
        {            
            P_n = iterationnumber==0? Mo*P:P_n*P;
            ++niters;
            return this;
        }


        public Matrix<double> Mo { get; private set; }
        public Matrix<double> P { get; private set; }
        public Matrix<double> P_n { get; private set; }

        public int niters { get; private set; }
    }
}
