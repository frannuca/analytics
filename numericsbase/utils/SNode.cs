using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace numericsbase.utils
{

    [XmlRoot(ElementName = "Root")]
    public class SNode<T>
    {
        public SNode() { }
        public SNode(Node<T> xmlnode)
        {
            

            uuid = xmlnode.uuid;
            data = xmlnode.data;
            parent = xmlnode.parent != null?xmlnode.parent.uuid:"";

            foreach(var node in xmlnode.children)
            {
                children.Add(node.uuid);               
            }

        }

        public string uuid;
        public string parent;
        public T data;
        public List<string> children = new List<string>();
    }
}
