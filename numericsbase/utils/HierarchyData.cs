using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace numericsbase.utils
{

    
    /// <summary>
    /// Data container containing:
    /// -A dictionary from key string to a tree graph with associated data for each node.
    /// - A reverse relation calculation, which for a given node calculates all the associated keys
    /// </summary>
    public class HierarchyData
    {

        public HierarchyData(Node<XmlSerializableDictionary<ERCVALUE, double?>> tree)
        {
            tbo = tree;

            foreach (var node in tbo.Breadth_first_search())
            {
                node.data = new XmlSerializableDictionary<ERCVALUE, double?>();
                foreach (var v in Enum.GetValues(typeof(ERCVALUE)).Cast<ERCVALUE>())
                {
                    node.data[v] = null;
                }
            }


        }

        public enum ERCVALUE { KEY1=1001,KEY2,KEY3,KEY4,KEY5};

        protected Node<XmlSerializableDictionary<ERCVALUE, double?>> tbo;
        
        protected Dictionary<string,Node<XmlSerializableDictionary<ERCVALUE, double?>>> ercdata;
        

       
        HierarchyData AddRisk(params string[] mnemonics)
        {
            foreach(var mnemonic in mnemonics)
            {
                if (!ercdata.Keys.Contains(mnemonic))
                {
                    ercdata[mnemonic] = (Node<XmlSerializableDictionary<ERCVALUE, double?>>)tbo.Clone();
                    
                }
            }
            
            return this;
        }

        public Node<XmlSerializableDictionary<ERCVALUE, double?>> this[string risktype]
        {
            get
            {
                return ercdata.Keys.Contains(risktype) ? ercdata[risktype] : null;
            }
        }

        public Node<XmlSerializableDictionary<ERCVALUE, double?>> this[string risktype,string nodeName]
        {
            get
            {
                var root = this[risktype];
                if(root != null)
                {
                    return (from n in root.Breadth_first_search() where n.name == nodeName select n).First();
                }
                else
                {
                    return null;
                }            
                               
                    
                
            }
        }

        XmlSerializableDictionary<ERCVALUE, double?> getERCValues(string risktype, string nodeName)
        {
            var node = this[risktype, nodeName];
            return node != null ? node.data : null;
        }

        List<XmlSerializableDictionary<ERCVALUE, double?>> getRisksForNode(string nodeName)
        {
            return (from risk in ercdata.Keys where this[risk, nodeName] != null select this[risk, nodeName].data).ToList(); 
        }

    }
}
