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

    public class Node<T>
    {
        public Node()
        {
            uuid = Guid.NewGuid().ToString();                       
        }

        public Node(Node<T> parent_): this()
        {
            parent = parent_;
        }

        public Node(Node<T> parent,IEnumerable<Node<T>> children_) : this(parent)
        {
            children = new List<Node<T>>(children);
        }

        readonly public string uuid;
        public Node<T> parent;
        public List<Node<T>> children;
        T data;

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
                visited.Add(current);

                foreach (var node in current.children)
                {
                    if (!visited.Contains(current))
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
    }

}
