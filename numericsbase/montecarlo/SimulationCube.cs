using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace numericsbase.montecarlo
{
    /// <summary>
    ///  Use for generating a read only simulation cube of uniformly random distributed numbers
    /// </summary>
    public class SimulationCube
    {

        public enum DISTRIBUTION { UNIFORM=0,NORMAL}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nsims"> total number of simulations to be generated</param>
        /// <param name="dimension"> Dimensionality of the random vectos to be used</param>
        /// <param name="nsteps">Number of samples per dimension that need to be generated per simulation</param>       

        public SimulationCube(UInt64 nsims,UInt64 dimension,UInt64 nsteps, DISTRIBUTION distribution=DISTRIBUTION.NORMAL)
        {
            number_of_sims = nsims;
            number_of_dims = dimension;
            number_of_steps = nsteps;
            simulation_vector_length = number_of_dims * number_of_steps;
            
            GenerateUCube(distribution);
            
        }

        readonly public UInt64 number_of_sims;
        readonly public UInt64 number_of_dims;
        readonly public UInt64 number_of_steps;
        readonly public UInt64 simulation_vector_length;

        const int seed = 42;

        private void GenerateUCube(DISTRIBUTION d)
        {
            if (array != null) return;
            array = new double[number_of_sims][][];

            var uniform = new MathNet.Numerics.Random.MersenneTwister(seed);
            var normal = new MathNet.Numerics.Distributions.Normal(0, 1,uniform);

            Func<double[]> rgn = null;
            switch(d)
            {
                case DISTRIBUTION.UNIFORM:
                    rgn = () =>
                    {
                        return uniform.NextDoubles((int)number_of_steps);
                    };
                    break;
                case DISTRIBUTION.NORMAL:
                    rgn = () =>
                    {
                        double[] r = new double[number_of_steps];
                        normal.Samples(r);
                        return r;
                    };
                    break;

            }           

            for(UInt64 i = 0; i < number_of_sims; ++i)
            {
                array[i] = new double[number_of_dims][];
                for (UInt64 j = 0; j < number_of_dims; ++j)
                {
                    array[i][j] = rgn();
                }
            }

        }


        public void CorrelateSimulationCube(MathNet.Numerics.LinearAlgebra.Matrix<double> Rho)
        {
            var cholesky = Rho.Cholesky().Factor;


        }

        public double[] this[UInt64 simulation,UInt64 dimension]
        {
            get
            {                                         
                return array[simulation][dimension];                                   
            }
            private set { }
        }

        public double[][] this[UInt64 simulation]
        {
            get
            {               
                return array[simulation];
            }
            private set { }
        }

        public double this[UInt64 simulation,UInt64 dimension,UInt64 step]
        {
            get
            {
                return array[simulation][dimension][step];
            }
            private set { }
        }


        public override bool Equals(object Obj)
        {
            SimulationCube other = (SimulationCube)Obj;
            if(other == null)
            {
                return false;
            }
            else if(other.number_of_dims != number_of_dims || other.number_of_sims != number_of_sims || other.number_of_steps != number_of_steps)
            {                
               return false;                
            }
            else
            {
                for(UInt64 i = 0; i < number_of_sims; ++i)
                {
                    for(UInt64 j = 0; j < number_of_dims; ++j)
                    {
                        for(UInt64 k = 0; k < number_of_steps; ++k)
                        {
                            if(array[i][j][k] != other[i, j, k])
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator!=(SimulationCube person1, SimulationCube person2)
        {
            return !person1.Equals(person2);
        }


        public static bool operator==(SimulationCube cube1, SimulationCube cube2)
        {
            if (Object.ReferenceEquals(cube1,null))
            {
                return Object.ReferenceEquals(cube2,null)? true:false;
                
            }
            else
            {
                return cube1.Equals(cube2);
            }
            
        }
        private double[][][] array = null;
    }
}
