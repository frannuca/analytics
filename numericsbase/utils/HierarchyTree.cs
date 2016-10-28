using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace numericsbase.utils
{
    [XmlRoot( ElementName = "Root")]

    public class Node<T>: IXmlSerializable
    {
        public Node()
        {
            uuid = Guid.NewGuid().ToString();
            children = new List<Node<T>>();                      
        }

        public Node(Node<T> parent_): this()
        {
            parent = parent_;
            children = new List<Node<T>>();
        }

        public Node(Node<T> parent, IEnumerable<Node<T>> children_) : this(parent)
        {
            children = new List<Node<T>>(children);
        }

        public string uuid { get; private set; }
        public Node<T> parent{ get; private set; }
        public List<Node<T>> children { get; set; }
        public T data { get;  set; }

        public int depth
        {
            get
            {
                int counter = 0;
                Node<T> p = parent;
                while (p != null)
                {
                    ++counter;
                    p = p.parent;
                }

                return counter;
            }
            private set { }
        }
        
        public IList<Node<T>> Breadth_first_search()
        {
            List<Node<T>> visited = new List<Node<T>>();

            Queue<Node<T>> queue = new Queue<Node<T>>();
            queue.Enqueue(this);
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (!visited.Contains(current))
                    visited.Add(current);

                foreach (var node in current.children)
                {
                    if (!visited.Contains(node))
                    {                        
                        queue.Enqueue(node);
                        visited.Add(node);
                    }
                }
            }

            return visited;
        }


        public override int GetHashCode()
        {
            return uuid.GetHashCode();
        }
        public static bool operator ==(Node<T> obj1, Node<T> obj2)
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

            return  obj1.uuid==obj2.uuid;
        }

        // this is second one '!='
        public static bool operator !=(Node<T> obj1, Node<T> obj2)
        {
            return !(obj1 == obj2);
        }


        public bool Equals(Node<T> node)
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

        public XmlSchema GetSchema()
        {
            return null;
        }

       
        public void ReadXml(XmlReader reader)
        {

            List<SNode<T>> listnodes = new List<SNode<T>>();

            reader.ReadStartElement("Root");           
            reader.MoveToAttribute("size");
            int count = 0;
            int.TryParse(reader.Value, out count);


            var ser1 = new XmlSerializer(typeof(SNode<T>));
            reader.ReadStartElement();
            for (int i = 0; i < count; ++i)
            {
                var node = new SNode<T>();
                node = (SNode<T>)ser1.Deserialize(reader);
                listnodes.Add(node);
            }


            reader.ReadEndElement();


            var tnodes = new List<Node<T>>();
            foreach(var tn in listnodes)
            {
                var x = new Node<T>();
                x.uuid = tn.uuid;
                x.data = tn.data;

                if (tn.parent != "")
                {
                    x.parent = (from y in tnodes where y.uuid == tn.parent select y).First();
                }
                
                tnodes.Add(x);
            }


            foreach(var tn in tnodes)
            {
                tn.children = (from r in tnodes where r.parent != null && r.parent.uuid == tn.uuid select r).ToArray().ToList();
            }


            this.uuid = tnodes.First().uuid;
            this.parent = tnodes.First().parent;
            this.children = tnodes.First().children;
            this.data = tnodes.First().data;
            

        }


        public void WriteXml(XmlWriter writer)
        {

            
            
            writer.WriteStartElement("node");

            var allnodes = from r in  Breadth_first_search() select new SNode<T>(r);

            writer.WriteAttributeString("size", allnodes.Count().ToString());

            var childrenSer = new XmlSerializer(typeof(SNode<T>));

            var ser1 = new XmlSerializer(typeof(SNode<T>));
            foreach (var snode in allnodes)
            {
                ser1.Serialize(writer, snode);                
            }


            writer.WriteEndElement();

         
        }
    }

}
