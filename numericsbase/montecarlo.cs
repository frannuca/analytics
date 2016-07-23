using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace numericsbase
{
    public class montecarlo
    {
        protected internal montecarlo(Dictionary<int,Matrix<float>> scubes_, Dictionary<int, Func<float, float, float>> volatilitysuface_, Dictionary<int, Func<float, float, float>> drift_)
        {
            scube = scubes_;           
            volatilitysuface = volatilitysuface_;
            drift = drift_;
        }
        public Dictionary<int, Matrix<float>> scube { get; protected set; }        
        public Dictionary<int,Func<float, float, float>> volatilitysuface { get; protected  set; }
        public Dictionary<int,Func<float, float, float>> drift { get; protected set; }
        
    }

    /// <summary>
    /// Monte Carlo engine builder. Use this class to create a montecarlo simulation.
    /// This class follows the builder template, where the final engine is composed with calls to <c>withSimulationCube</c>, <c>withcorrelationMatrix</c>,
    /// <c>withvolatilitySurface</c> or <c>withvolatility</c>, <c>withdrift</c>. After filling of required information call <c>build</c> to obtained the built engine.
    /// Example:
    /// <code>
    /// var builder = new montecarloBuild().withSimulationCube(s)
    /// .withcorrelationMatrix(r)
    /// .withvolatility(0,0.15)
    /// .withvolatilitySurface(1,volatitulySurfaceFunction)
    /// .withdrift((S,T)=>{return ?;}}
    /// .build()
    /// </code>
    /// </summary>
    public class montecarloBuild
    {
       
       public montecarloBuild withSimulationCube(simcube s)
        {
            scube = s;
            return this;

        }
        public montecarloBuild withcorrelationMatrix(Matrix<float> r)
        {
            rhoMatrix = r;
            return this;
        }

        public montecarloBuild withvolatilitySurface(int index, Func<float, float, float> volsurface)
        {
            if (volatilitysuface == null)
                volatilitysuface = new Dictionary<int, Func<float, float, float>>();
            volatilitysuface[index] = volsurface;
            return this;
        }

        public montecarloBuild withvolatility(int index, float sigma)
        {
            withvolatilitySurface(index, (x,y) => { return sigma; });
            return this;
        }

        public montecarloBuild withdrift(int index, float mu)
        {
            withdrift(index, (x, y) => { return mu; });
            return this;
        }
        public montecarloBuild withdrift(int index, Func<float, float, float> shiftfunctor)
        {
            if (drift == null)
                drift = new Dictionary<int, Func<float, float, float>>();

            drift[index] = shiftfunctor;
            return this;
        }

        public montecarlo build()
        {
            if(scube == null || rhoMatrix  == null || volatilitysuface == null)
            {
                throw new ArgumentException("In order to build a montecarlo engine you need to provide the following data:");
            }
            return new montecarlo(transformsimcube(), volatilitysuface, drift);
        }
        private simcube scube=null;       
        private Matrix<float> rhoMatrix=null;
        private Dictionary<int, Func<float, float, float>> volatilitysuface=null;
        private Dictionary<int, Func<float, float, float>> drift=null;

        private Dictionary<int, Matrix<float>> transformsimcube()
        {
            if (scube == null)
            {
                throw new InvalidOperationException("Trying to generate a montecarlo simulation engine without simulation cube");
            }
            else
            {               
                Matrix<float> C =  rhoMatrix.Cholesky().Solve(rhoMatrix);
                return scube.transform(C);
            }
        }
    }
}
