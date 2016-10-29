using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace numericsbase.utils
{
    /// <summary>
    /// Node base structure to construct tree graph.
    /// This graph associated to each node a parent, children and a generic data structure.
    /// The associated generic data type, must be support by xml serialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [XmlRoot( ElementName = "Root")]
    public class Node<T>: IXmlSerializable, ICloneable
    {

        public Node() { }
        /// <summary>
        /// Base ctor. Create an object with automatic uuid and empty list of children and null parent
        /// </summary>
        public Node(string name_)
        {
            uuid = Guid.NewGuid().ToString();
            children = new List<Node<T>>();
            name = name_;                
        }

        /// <summary>
        /// Create a node with a given parent, uuuid and empty list of children
        /// </summary>
        /// <param name="parent_">Parent Node</param>
        public Node(string name,Node<T> parent_): this(name)
        {
            parent = parent_;
            children = new List<Node<T>>();
        }

        /// <summary>
        /// Creates a Node with given parent and children uuid.
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="children_">Children list</param>
        public Node(string name,Node<T> parent, IEnumerable<Node<T>> children_) : this(name,parent)
        {
            children = new List<Node<T>>(children);
        }

        /// <summary>
        /// Unique identifier of the node.
        /// </summary>
        public string uuid { get; private set; }


        public string name { get; private set; }
        /// <summary>
        /// Parent node
        /// </summary>
        public Node<T> parent{ get; private set; }

        /// <summary>
        /// Children list
        /// </summary>
        public List<Node<T>> children { get; set; }

        /// <summary>
        /// Custom Data object
        /// </summary>
        public T data { get;  set; }

        /// <summary>
        /// 0-based node level starting from the root node
        /// </summary>
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

        /// <summary>
        /// Equality for two nodes is perform by comparison of their uuid
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Equality of two nodes is resolver using their internal uuids
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool operator !=(Node<T> obj1, Node<T> obj2)
        {
            return !(obj1 == obj2);
        }

        /// <summary>
        /// Equality of two nodes is resolverd using their internal uuids.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

       /// <summary>
       /// Serializes this object into xml.
       /// The underlying data structure is a flat tree, using the type SNode
       /// </summary>
       /// <param name="reader"></param>
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
                var x = new Node<T>(tn.name);
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

            this.name = tnodes.First().name;
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


        public bool HierarchyEquals(Node<T> obj, Func<T,T,bool> dataComparator)
        {
            var thislist = from r in  Breadth_first_search() select new SNode<T>(r);
            var thatlist = from r in obj.Breadth_first_search() select new SNode<T>(r);

            foreach (var o in thislist.Zip(thatlist,(a,b)=>new KeyValuePair<SNode<T>,SNode<T>>(a,b)))
            {
                if((o.Key.uuid != o.Value.uuid) ||
                   (o.Key.parent != o.Value.parent) ||
                   (o.Key.name != o.Value.name) ||
                   !dataComparator(o.Key.data,o.Value.data)
                   )
                {
                    return false;
                }
                foreach(var n in o.Key.children.Zip(o.Value.children,(a,b)=> new KeyValuePair<string, string>(a, b))){
                    if(n.Key != n.Value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        
        public object Clone()
        {
            var root = this;
            XmlSerializer x = new XmlSerializer(typeof(Node<T>));
            StringWriter stringwriter = new StringWriter();
            x.Serialize(stringwriter, root);
            var xmlstr = stringwriter.ToString();
            return x.Deserialize(new StringReader(xmlstr));
        }
    }

}
