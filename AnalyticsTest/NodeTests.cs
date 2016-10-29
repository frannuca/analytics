using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using numericsbase.utils;
using System.Collections.Generic;

namespace AnalyticsTest
{
    using System.IO;
    using DNode = Node<XmlSerializableDictionary<string, double?>>;
    [TestClass]
    public class NodeTests
    {

        
        static DNode GetTestTree()
        {
            DNode root = new DNode("Top",null);

           

            Func<string,DNode,double?,double?,DNode> newnode = (name,parent, k1, k2) =>
            {
                var child1 = new DNode(name,parent);
                child1.data = new XmlSerializableDictionary<string, double?>()
                {
                    {"key11",k1},
                    {"key12",k2}
                };

                return child1;
            };




            var child11 = newnode("child11",root, 1, 1);
            var child12 = newnode("child12", root, 1, 2);
            var child13 = newnode("child13", root, 1, 3);
            root.children.AddRange( new DNode[]{ child11,child12,child13});

            var child11_1 = newnode("child11_1", child11, 11, 1);
            var child11_2 = newnode("child11_2", child11, 11, 2);
            var child11_3 = newnode("child11_3", child11, 11, 3);
            child11.children.AddRange(new DNode[] { child11_1, child11_2, child11_3 });


            var child12_1 = newnode("child12_1", child11, 12, 1);
            var child12_2 = newnode("child12_2", child11, 12, 2);
            var child12_3 = newnode("child12_3", child11, 12, 3);
            child12.children.AddRange(new DNode[] { child12_1, child12_2, child12_3 });


            var child12_3_1 = newnode("child12_3",child12_3, 12, 31);
            child11_3.children.Add(child12_3_1);

            return root;
        }
        [TestMethod]
        public void Test01_Node()
        {



            var root = GetTestTree();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(DNode));
            StringWriter stringwriter = new StringWriter();

            x.Serialize(stringwriter, root);
            var xmlstr = stringwriter.ToString();


            var a = (DNode)x.Deserialize(new StringReader(xmlstr));

            Func<XmlSerializableDictionary<string, double?>, XmlSerializableDictionary<string, double?>, bool> dictionarycomparator =
                (p1, p2) =>
                {
                    if(p1==null || p2 == null)
                    {
                        return p1 == p2;
                    }
                    else
                    {
                        foreach (var k in p1.Keys)
                        {
                            if (p1[k] != p2[k])
                            {
                                return false;
                            }
                        }
                        return true;

                    }
                };

            Assert.AreEqual(root, a);
            var eq = a.HierarchyEquals(root, dictionarycomparator);
        }
    }
}
