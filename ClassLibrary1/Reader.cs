namespace ClassLibrary1
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Query;
    using GraphNodeType = VDS.RDF.NodeType;

    public partial class Reader : XmlReader
    {
        private readonly INode root;
        private readonly bool rdf;
        private readonly ISet<INode> seen;
        private ReaderState state;

        private Reader @object;
        private Queue<Triple> predicateTriples;
        private Queue<Reader> listItems;
        private Queue<Triple> collectionTriples;

        // TODO: Test
        public Reader(IGraph g)
            : this(g.Triples.SubjectNodes)
        {
        }

        // TODO: Test
        public Reader(IGraph g, params Uri[] uris)
            : this(g, (IEnumerable<Uri>)uris)
        {
        }

        // TODO: Test
        public Reader(IGraph g, IEnumerable<Uri> uris)
            : this(uris.Select(u => g.GetUriNode(u)))
        {
        }

        // TODO: Test
        public Reader(IGraph g, string query)
            : this(g, new SparqlQueryParser().ParseFromString(query))
        {
        }

        // TODO: Test
        public Reader(IGraph g, SparqlQuery query)
            : this(((SparqlResultSet)g.ExecuteQuery(Validate(query))).Select(r => r["root"]))
        {
        }

        public Reader(params INode[] roots)
            : this((IEnumerable<INode>)roots)
        {
        }

        public Reader(IEnumerable<INode> roots)
            : this(roots, true, new HashSet<INode>())
        {
        }

        private Reader(INode root, bool rdf, ISet<INode> seen)
            : this(root.AsEnumerable(), rdf, seen)
        {
        }

        private Reader(IEnumerable<INode> roots, bool rdf, ISet<INode> seen)
        {
            // TODO: Implement
            var root = roots.Single();
            this.root = root ?? throw new ArgumentNullException(nameof(root));
            this.rdf = rdf;
            this.seen = seen;
        }

        private static SparqlQuery Validate(SparqlQuery query)
        {
            // TODO: Validate
            return query;
        }

        private Triple Predicate => predicateTriples.Peek();

        private Reader ListItem => listItems.Peek();

        // TODO: Finish xml:base
        public override string BaseURI => "urn:";

        // TODO: Implement
        public override int Depth => 0;

        public override bool IsEmptyElement
        {
            get
            {
                switch (state)
                {
                    case ReaderState.Object:
                    case ReaderState.CollectionObject:
                        return @object.IsEmptyElement;

                    case ReaderState.List:
                        return ListItem.IsEmptyElement;

                    case ReaderState.Subject:
                        return seen.Contains(root) || !root.Graph.GetTriplesWithSubject(root).Any();

                    default:
                        return false;
                }
            }
        }

        public override string LocalName
        {
            get
            {
                switch (state)
                {
                    case ReaderState.Object:
                    case ReaderState.CollectionObject:
                        return @object.LocalName;

                    case ReaderState.List:
                        return ListItem.LocalName;

                    case ReaderState.Predicate:
                        return GetLocalName(Predicate.Predicate as UriNode);

                    case ReaderState.CollectionPredicate:
                        return GetLocalName(collectionTriples.Peek().Predicate as UriNode);

                    case ReaderState.Prefix:
                        var prefix = prefixes.Peek();
                        if (string.IsNullOrEmpty(prefix))
                        {
                            return "xmlns";
                        }

                        return prefix;

                    case ReaderState.RDF:
                        return "RDF";

                    case ReaderState.Subject:
                        return "Description";

                    case ReaderState.About:
                        return "about";

                    case ReaderState.NodeId:
                        return "nodeID";

                    case ReaderState.ParseType:
                        return "parseType";

                    case ReaderState.Datatype:
                        return "datatype";

                    case ReaderState.Language:
                        return "lang";

                    default:
                        throw new Exception(state.ToString());
                }
            }
        }

        public override string NamespaceURI
        {
            get
            {
                switch (state)
                {
                    case ReaderState.Object:
                    case ReaderState.CollectionObject:
                        return @object.NamespaceURI;

                    case ReaderState.List:
                        return ListItem.NamespaceURI;

                    case ReaderState.RDF:
                    case ReaderState.Subject:
                    case ReaderState.About:
                    case ReaderState.ParseType:
                    case ReaderState.NodeId:
                    case ReaderState.Datatype:
                        return "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

                    case ReaderState.Predicate:
                        return GetNamespaceUri(Predicate.Predicate as IUriNode);

                    case ReaderState.CollectionPredicate:
                        return GetNamespaceUri(collectionTriples.Peek().Predicate as IUriNode);

                    case ReaderState.Language:
                        return XmlSpecsHelper.NamespaceXml;

                    case ReaderState.Prefix:
                        if (string.IsNullOrEmpty(prefixes.Peek()))
                        {
                            return null;
                        }

                        return XmlSpecsHelper.NamespaceXmlNamespaces;

                    default:
                        throw new Exception(state.ToString());
                }
            }
        }

        public override XmlNodeType NodeType
        {
            get
            {
                switch (state)
                {
                    case ReaderState.Initial:
                        return XmlNodeType.None;

                    case ReaderState.Object:
                    case ReaderState.CollectionObject:
                        return @object.NodeType;

                    case ReaderState.List:
                        return ListItem.NodeType;

                    case ReaderState.Literal:
                        return XmlNodeType.Text;

                    case ReaderState.RDF:
                    case ReaderState.Subject:
                    case ReaderState.Predicate:
                    case ReaderState.CollectionPredicate:
                        return XmlNodeType.Element;

                    case ReaderState.About:
                    case ReaderState.ParseType:
                    case ReaderState.NodeId:
                    case ReaderState.Datatype:
                    case ReaderState.Language:
                    case ReaderState.Prefix:
                        return XmlNodeType.Attribute;

                    case ReaderState.AboutValue:
                    case ReaderState.ParseTypeValue:
                    case ReaderState.NodeIdValue:
                    case ReaderState.DatatypeValue:
                    case ReaderState.LanguageValue:
                    case ReaderState.PrefixValue:
                        return XmlNodeType.Text;

                    case ReaderState.EndObject:
                    case ReaderState.EndPredicate:
                    case ReaderState.EndCollectionPredicate:
                    case ReaderState.EndSubject:
                        return XmlNodeType.EndElement;

                    default:
                        throw new Exception(state.ToString());
                }
            }
        }

        public override string Prefix
        {
            get
            {
                switch (state)
                {
                    case ReaderState.Object:
                    case ReaderState.CollectionObject:
                        return @object.Prefix;

                    case ReaderState.List:
                        return ListItem.Prefix;

                    case ReaderState.RDF:
                    case ReaderState.Subject:
                    case ReaderState.About:
                    case ReaderState.ParseType:
                    case ReaderState.NodeId:
                    case ReaderState.Datatype:
                        return "rdf";

                    case ReaderState.Language:
                        return "xml";

                    case ReaderState.Predicate:
                        if (root.Graph.NamespaceMap.ReduceToQName(((IUriNode)Predicate.Predicate).Uri.AbsoluteUri, out var qname))
                        {
                            return qname.Substring(0, qname.IndexOf(':'));
                        }

                        return null;

                    case ReaderState.CollectionPredicate:
                        return null;

                    case ReaderState.Prefix:
                        if (string.IsNullOrEmpty(prefixes.Peek()))
                        {
                            return null;
                        }

                        return "xmlns";

                    default:
                        throw new Exception(state.ToString());
                }
            }
        }

        public override ReadState ReadState => ReadState.Initial;

        public override string Value
        {
            get
            {
                switch (state)
                {
                    case ReaderState.Object:
                    case ReaderState.CollectionObject:
                        return @object.Value;

                    case ReaderState.List:
                        return ListItem.Value;

                    case ReaderState.Literal:
                        return ((ILiteralNode)root).Value;

                    case ReaderState.AboutValue:
                        return root.ToString();

                    case ReaderState.NodeIdValue:
                        return ((IBlankNode)root).InternalID;

                    case ReaderState.ParseTypeValue:
                        return "Collection";

                    case ReaderState.DatatypeValue:
                        return ((ILiteralNode)Predicate.Object).DataType.AbsoluteUri;

                    case ReaderState.LanguageValue:
                        return ((ILiteralNode)Predicate.Object).Language;

                    case ReaderState.PrefixValue:
                        return root.Graph.NamespaceMap.GetNamespaceUri(prefixes.Peek()).AbsoluteUri;

                    default:
                        throw new Exception(state.ToString());
                }
            }
        }

        public override bool Read()
        {
            switch (state)
            {
                case ReaderState.Initial:
                    if (rdf)
                    {
                        return TransitionTo(ReaderState.RDF);
                    }

                    goto case ReaderState.RDF;

                case ReaderState.RDF:
                    if (root.NodeType == GraphNodeType.Literal)
                    {
                        return TransitionTo(ReaderState.Literal);
                    }

                    if (root.IsListRoot(root.Graph) && !root.Graph.GetListItems(root).Any(i => i.NodeType == GraphNodeType.Literal))
                    {
                        listItems = new Queue<Reader>();

                        foreach (var item in root.Graph.GetListItems(root))
                        {
                            var listReader = new Reader(item, false, seen);
                            listReader.Read();
                            listItems.Enqueue(listReader);
                        }

                        return TransitionTo(ReaderState.List);
                    }

                    return TransitionTo(ReaderState.Subject);

                case ReaderState.Literal:
                    return false;

                case ReaderState.List:
                    if (ListItem.Read())
                    {
                        return true;
                    }

                    listItems.Dequeue();

                    return listItems.Any();

                case ReaderState.Subject:
                    if (!seen.Add(root) || !root.Graph.GetTriplesWithSubject(root).Any())
                    {
                        goto case ReaderState.EndSubject;
                    }

                    collectionTriples = new Queue<Triple>(
                        from t in root.Graph.GetTriplesWithSubject(root)
                        where t.Object.IsListRoot(root.Graph) && !root.Graph.GetListItems(t.Object).Any(i => i.NodeType == GraphNodeType.Literal)
                        select t);

                    predicateTriples = new Queue<Triple>(
                        from t in root.Graph.GetTriplesWithSubject(root)
                        where !t.Object.IsListRoot(root.Graph) || root.Graph.GetListItems(t.Object).Any(i => i.NodeType == GraphNodeType.Literal)
                        select t);

                    if (collectionTriples.Any())
                    {
                        return TransitionTo(ReaderState.CollectionPredicate);
                    }

                    return TransitionTo(ReaderState.Predicate);

                case ReaderState.CollectionPredicate:
                    @object = new Reader(collectionTriples.Peek().Object, false, seen);
                    @object.Read();

                    return TransitionTo(ReaderState.CollectionObject);

                case ReaderState.CollectionObject:
                    if (@object.Read())
                    {
                        return true;
                    }

                    return TransitionTo(ReaderState.EndCollectionPredicate);

                case ReaderState.EndCollectionPredicate:
                    collectionTriples.Dequeue();

                    if (collectionTriples.Any())
                    {
                        return TransitionTo(ReaderState.CollectionPredicate);
                    }

                    return TransitionToIfAny(predicateTriples, ReaderState.Predicate, ReaderState.EndSubject);

                case ReaderState.Predicate:
                    @object = new Reader(Predicate.Object, false, seen);
                    @object.Read();

                    return TransitionTo(ReaderState.Object);

                case ReaderState.Object:
                    if (@object.Read())
                    {
                        return true;
                    }

                    goto case ReaderState.EndObject;

                case ReaderState.EndObject:
                    return TransitionTo(ReaderState.EndPredicate);

                case ReaderState.EndPredicate:
                    predicateTriples.Dequeue();
                    return TransitionToIfAny(predicateTriples, ReaderState.Predicate, ReaderState.EndSubject);

                case ReaderState.EndSubject:
                    return false;

                default:
                    throw new Exception(state.ToString());
            }
        }

        private Queue<string> prefixes;

        public override bool MoveToFirstAttribute()
        {
            switch (state)
            {
                case ReaderState.RDF:
                    prefixes = new Queue<string>(root.Graph.NamespaceMap.Prefixes);

                    if (prefixes.Any())
                    {
                        return TransitionTo(ReaderState.Prefix);
                    }

                    return false;

                case ReaderState.Object:
                case ReaderState.CollectionObject:
                    return @object.MoveToFirstAttribute();

                case ReaderState.List:
                    return ListItem.MoveToFirstAttribute();

                case ReaderState.Subject:
                    if (root.NodeType == GraphNodeType.Blank)
                    {
                        if (root.Graph.GetTriplesWithObject(root).Skip(1).Any())
                        {
                            return TransitionTo(ReaderState.NodeId);
                        }

                        return false;
                    }

                    return TransitionTo(ReaderState.About);

                case ReaderState.CollectionPredicate:
                    return TransitionTo(ReaderState.ParseType);

                case ReaderState.Predicate:
                    if (Predicate.Object.NodeType == GraphNodeType.Literal)
                    {
                        var literal = (ILiteralNode)Predicate.Object;
                        if (literal.DataType != null)
                        {
                            return TransitionTo(ReaderState.Datatype);
                        }

                        if (!string.IsNullOrEmpty(literal.Language))
                        {
                            return TransitionTo(ReaderState.Language);
                        }
                    }

                    return false;

                default:
                    throw new Exception(state.ToString());
            }
        }

        public override bool ReadAttributeValue()
        {
            switch (state)
            {
                case ReaderState.Object:
                case ReaderState.CollectionObject:
                    return @object.ReadAttributeValue();

                case ReaderState.List:
                    return ListItem.ReadAttributeValue();

                case ReaderState.About:
                    return TransitionTo(ReaderState.AboutValue);

                case ReaderState.AboutValue:
                    return TransitionTo(ReaderState.About, false);

                case ReaderState.NodeId:
                    return TransitionTo(ReaderState.NodeIdValue);

                case ReaderState.NodeIdValue:
                    return TransitionTo(ReaderState.NodeId, false);

                case ReaderState.ParseType:
                    return TransitionTo(ReaderState.ParseTypeValue);

                case ReaderState.ParseTypeValue:
                    return TransitionTo(ReaderState.ParseType, false);

                case ReaderState.Datatype:
                    return TransitionTo(ReaderState.DatatypeValue);

                case ReaderState.DatatypeValue:
                    return TransitionTo(ReaderState.Datatype, false);

                case ReaderState.Language:
                    return TransitionTo(ReaderState.LanguageValue);

                case ReaderState.LanguageValue:
                    return TransitionTo(ReaderState.Language, false);

                case ReaderState.Prefix:
                    return TransitionTo(ReaderState.PrefixValue);

                case ReaderState.PrefixValue:
                    return TransitionTo(ReaderState.Prefix, false);

                default:
                    throw new Exception(state.ToString());
            }
        }

        public override bool MoveToNextAttribute()
        {
            switch (state)
            {
                case ReaderState.Prefix:
                    prefixes.Dequeue();

                    return prefixes.Any();

                default:
                    return false;
            }
        }

        public override bool MoveToElement()
        {
            switch (state)
            {
                case ReaderState.Object:
                case ReaderState.CollectionObject:
                    return @object.MoveToElement();

                case ReaderState.List:
                    return ListItem.MoveToElement();

                case ReaderState.About:
                case ReaderState.NodeId:
                    return TransitionTo(ReaderState.Subject);

                case ReaderState.ParseType:
                    return TransitionTo(ReaderState.CollectionPredicate);

                case ReaderState.Datatype:
                case ReaderState.Language:
                    return TransitionTo(ReaderState.Predicate);

                case ReaderState.Prefix:
                    return TransitionTo(ReaderState.RDF);

                default:
                    throw new Exception(state.ToString());
            }
        }

        [DebuggerStepThrough]
        private bool TransitionTo(ReaderState targetState)
        {
            return TransitionTo(targetState, true);
        }

        [DebuggerStepThrough]
        private bool TransitionTo(ReaderState targetState, bool result)
        {
            state = targetState;
            return result;
        }

        [DebuggerStepThrough]
        private bool TransitionToIfAny<T>(Queue<T> queue, ReaderState yesState, ReaderState noState)
        {
            if (queue.Any())
            {
                return TransitionTo(yesState);
            }
            else
            {
                return TransitionTo(noState);
            }
        }

        [DebuggerStepThrough]
        private string GetLocalName(IUriNode uriNode)
        {
            var u = uriNode.Uri;
            if (u.AbsoluteUri.Contains("#"))
            {
                return u.Fragment.TrimStart('#');
            }
            else
            {
                return u.AbsoluteUri.Split('/').Last();
            }
        }

        [DebuggerStepThrough]
        private string GetNamespaceUri(IUriNode uriNode)
        {
            var u = uriNode.Uri.AbsoluteUri;
            if (u.Contains("#"))
            {
                return u.Substring(0, u.LastIndexOf('#') + 1);
            }
            else
            {
                return u.Substring(0, u.LastIndexOf('/') + 1);
            }
        }















        public override int AttributeCount => throw new NotImplementedException();

        public override bool EOF => throw new NotImplementedException();

        // TODO: Implement
        public override XmlNameTable NameTable => throw new NotImplementedException();

        public override string GetAttribute(int i) => throw new NotImplementedException();

        public override string GetAttribute(string name) => throw new NotImplementedException();

        public override string GetAttribute(string name, string namespaceURI) => throw new NotImplementedException();

        public override string LookupNamespace(string prefix) => throw new NotImplementedException();

        public override bool MoveToAttribute(string name) => throw new NotImplementedException();

        public override bool MoveToAttribute(string name, string ns) => throw new NotImplementedException();

        public override void ResolveEntity() => throw new NotImplementedException();
    }
}
