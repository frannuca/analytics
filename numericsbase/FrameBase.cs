using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace numericsbase
{
    class FrameBase<TIndex,TData> where TIndex:IComparable
    {
        
        public FrameBase(List<string> columns)
        {
            tdata.Columns.Add("index", typeof(TIndex));

            foreach (var c in columns)
                tdata.Columns.Add(c, typeof(TData));
        }

        public FrameBase(FrameBase<TIndex,TData> that)
        {
            tdata = that.tdata.Copy();            
        }
        public TData this[TIndex i,string column]
        {
            get
            {
                return
                    (from r in tdata.Rows.Cast<DataRow>() where r.Field<TIndex>("index").Equals(i) select r.Field<TData>(column))
                    .ToList<TData>().FirstOrDefault<TData>();
            }
            set
            {

                var row = (from r in tdata.AsEnumerable()
                          where r.Field<TIndex>("index").Equals(i)
                          select r).ToList<DataRow>();

                row.First()[column] = value;                
            }
        }
        
        DataRow newrow()
        {
            return tdata.NewRow();
        }
        
        protected DataTable tdata;    
        private IEnumerable<DataRow> getRange(Func<TIndex,bool> pred, IEnumerable<string> columns)
        {
            return from r in tdata.AsEnumerable() where pred(r.Field<TIndex>("index")) select r;
        }    
    }
}
