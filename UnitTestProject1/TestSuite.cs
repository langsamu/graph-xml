namespace UnitTestProject1
{
    using ClassLibrary1;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Writing;

    [TestClass]
    public class TestSuite
    {
        private static TestStore store = new TestStore(UriFactory.Create("https://www.w3.org/2013/RDFXMLTests/manifest.ttl"));

        public static IEnumerable<object[]> Tests
        {
            get
            {
                return store
                    .Tests
                    .Where(test => test.Name != "rdfms-rdf-names-use-test-028") // dotNetRDF Extensions.GetListItems can't handle rdf:first without rdf:rest
                    .Select(test => new[] { test });
            }
        }

        [TestMethod]
        [DynamicData(nameof(Tests))]
        public void SuiteTest(Test test)
        {
            AreEqual(test.Result, Load(test.Result));
        }

        private static void AreEqual(IGraph expected, IGraph actual)
        {
            Console.WriteLine("XML");
            Console.WriteLine("---------------------------------");

            Process(expected, Console.Out);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("EXPECTED");
            Console.WriteLine("---------------------------------");

            new CompressingTurtleWriter().Save(expected, Console.Out, true);

            Console.WriteLine();
            Console.WriteLine("ACTUAL");
            Console.WriteLine("---------------------------------");

            new CompressingTurtleWriter().Save(actual, Console.Out, true);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Empty()
        {
            var g = new Graph();

            AreEqual(g, Load(g));
        }


        [TestMethod]
        public void Multiple()
        {
            var g = new Graph();
            g.LoadFromString(@"
@prefix :<urn:/> .

:s1 :p :o .
:s2 :p :o .
");

            AreEqual(g, Load(g));
        }

        private static IGraph Load(IGraph g)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    Process(g, writer);

                    stream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(stream))
                    {
                        var g2 = new Graph();
                        new RdfXmlParser().Load(g2, reader);
                        return g2;
                    }
                }
            }
        }

        private static void Process(IGraph g, TextWriter stream)
        {
            using (var reader = new Reader(g))
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
                {
                    writer.WriteNode(reader, false);
                }
            }
        }
    }

    public class TestStore : WrapperTripleStore
    {
        private IGraph defaultGraph;

        public TestStore(Uri defaultGraphUri)
            : base(new WebDemandTripleStore(defaultGraphUri))
        {
            this.defaultGraph = this[defaultGraphUri];
        }

        public IEnumerable<Test> Tests
        {
            get
            {
                return defaultGraph.GetListItems(defaultGraph.GetTriplesWithSubjectPredicate(defaultGraph.GetUriNode(defaultGraph.BaseUri), defaultGraph.GetUriNode("mf:entries")).Single().Object).Select(xx => new Test(xx, this)).Where(xx => xx.Type == "rdft:TestXMLEval");
            }
        }
    }

    public class Test
    {
        private readonly INode node;
        private readonly ITripleStore store;

        public Test(INode node, ITripleStore store)
        {
            this.node = node;
            this.store = store;
        }

        public string Type
        {
            get
            {
                var type = node.Graph.GetTriplesWithSubjectPredicate(node, node.Graph.GetUriNode("rdf:type")).Select(t => ((IUriNode)t.Object).Uri.AbsoluteUri).Single();
                node.Graph.NamespaceMap.ReduceToQName(type, out var qname);
                return qname;
            }
        }

        public string Name
        {
            get
            {
                return node.Graph.GetTriplesWithSubjectPredicate(node, node.Graph.GetUriNode("mf:name")).Select(t => ((ILiteralNode)t.Object).Value).Single();
            }
        }

        public IGraph Result
        {
            get
            {
                var uri = node.Graph.GetTriplesWithSubjectPredicate(node, node.Graph.GetUriNode("mf:result")).Select(t => ((IUriNode)t.Object).Uri).Single();

                if (!this.store.HasGraph(uri))
                {
                    var g = new Graph();
                    g.LoadFromUri(uri, new NTriplesParser());
                    this.store.Add(g);
                }

                return this.store[uri];
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
