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

            name = xmlnode.name;
            uuid = xmlnode.uuid;
            data = xmlnode.data;
            parent = xmlnode.parent != null?xmlnode.parent.uuid:"";

            foreach(var node in xmlnode.children)
            {
                children.Add(node.uuid);               
            }

        }

        public string name { get; set; }
        public string uuid { get; set; }
        public string parent { get; set; }
        public T data { get; set; }
        public List<string> children = new List<string>();

        public override int GetHashCode()
        {
            return uuid.GetHashCode();
        }
        public static bool operator ==(SNode<T> obj1, SNode<T> obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }

            return obj1.uuid == obj2.uuid;
        }

        // this is second one '!='
        public static bool operator !=(SNode<T> obj1, SNode<T> obj2)
        {
            return !(obj1 == obj2);
        }


        public bool Equals(SNode<T> node)
        {
            if (ReferenceEquals(node, null))
            {
                return false;
            }
            if (ReferenceEquals(this, node))
            {
                return true;
            }

            return this.uuid == node.uuid;

        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Node<T>)obj);

        }
    }
}
