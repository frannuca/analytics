using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace numericsbase.population
{

    public class PState
    {
        public readonly string fromstate;
        public readonly string tostate;

        private PState()
        {
            fromstate = "";
            tostate = "";
        }

        public PState(string fromState,string toState)
        {
            var ordered = new List<string>() { fromState.ToLower(), toState.ToLower() };
            ordered.Sort();
            fromState = ordered[0];
            tostate = ordered[1];
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(obj, null))
            {
                return false;
            }
            else if (ReferenceEquals(this, obj))
            {
                return true;
            }
            else if(obj is PState)
            {
                return this.Equals((PState)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (fromstate+tostate).GetHashCode();
        }

        public bool Equals(PState obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            else
            {
                return fromstate == obj.fromstate && tostate == obj.tostate;
            }
        }

        public static bool operator ==(PState o1,PState o2)
        {
            if (ReferenceEquals(o1, o2))
            {
                return true;
            }
            else if(ReferenceEquals(o1,null) || ReferenceEquals(o2, null))
            {
                return false;
            }
            else
            {
                return o1.Equals(o2);
            }
        }

        public static bool operator !=(PState o1,PState o2)
        {
            return !(o1 == o2);
        }
    }
    public class PopulationPMatrixBuilder
    {
               
        public PopulationPMatrixBuilder WithTransitionProbability(PState s, double p)
        {           
            transitions[s] = p;
            return this;            
        }

        public PopulationPMatrixBuilder WithInitialState(Matrix<double> p0)
        {
            Po = p0;
            return this;
        }
        
        PopulationMatrix Build()
        {
            var listofstates = (from s in transitions select new List<string>() { s.Key.fromstate, s.Key.tostate })
                .SelectMany(x => x)
                .Distinct()
                .ToList();

            Matrix<double> M = Matrix<double>.Build.Dense(listofstates.Count(), listofstates.Count());
            for(int i = 0; i < M.RowCount; ++i)
            {
                
                for(int j = i; j < M.ColumnCount; ++j)
                {
                    var state = new PState(listofstates[i], listofstates[j]);
                    if (transitions.Keys.Contains(state))
                    {
                        M[i, j] = transitions[state];
                        M[j, i] = M[j, i];
                    }
                    else
                    {
                        M[i, j] = M[j, i] = 0.0;
                    }                    
                }
                if(M.Row(i).Sum() != 1.0)
                {
                    throw new Exception(String.Format("Invalid transtion matrix for P(x/{0})", listofstates[i]));
                }
            }

            if(Po == null)
            {
                throw new Exception(String.Format("No initial State matrix has been provided"));
            }
            return new PopulationMatrix(M, Po, listofstates.ToArray());
        }
        
        private Dictionary<PState, double> transitions = new Dictionary<PState, double>();
        private Matrix<double> Po = null;
    }


    public class PopulationMatrix
    {
        public PopulationMatrix(Matrix<double> ptrans,Matrix<double> Po,string[] listofstates)
        {
            P = ptrans;
            P_init = Po;
            states = listofstates;
        }

        public  Matrix<double> P { get; private set; }
        public Matrix<double> P_init { get; private set; }
        public string[] states { get; private set; }

    }
}
