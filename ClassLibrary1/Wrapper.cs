namespace ClassLibrary1
{
    using System;
    using System.Xml;

    public class Wrapper : XmlReader
    {
        private readonly XmlReader original;

        public Wrapper(XmlReader original)
        {
            this.original = original;
        }

        public override bool IsEmptyElement
        {
            get
            {
                var result = original.IsEmptyElement;
                Console.WriteLine($"IsEmptyElement {result}");
                return result;
            }
        }

        public override string LocalName
        {
            get
            {
                var result = original.LocalName;
                Console.WriteLine($"LocalName {result}");
                return result;
            }
        }

        public override XmlNodeType NodeType
        {
            get
            {
                var result = original.NodeType;
                Console.WriteLine($"NodeType {result}");
                return result;
            }
        }

        public override bool Read()
        {
            var result = original.Read();
            Console.WriteLine($"Read {result}");
            return result;
        }

        public override string BaseURI
        {
            get
            {
                var result = original.BaseURI;
                Console.WriteLine($"BaseURI {result}");
                return result;
            }
        }

        public override ReadState ReadState
        {
            get
            {
                var result = original.ReadState;
                Console.WriteLine($"ReadState {result}");
                return result;
            }
        }

        public override string Prefix
        {
            get
            {
                var result = original.Prefix;
                Console.WriteLine($"Prefix {result}");
                return result;
            }
        }

        public override string NamespaceURI
        {
            get
            {
                var result = original.NamespaceURI;
                Console.WriteLine($"NamespaceURI {result}");
                return result;
            }
        }

        public override bool MoveToFirstAttribute()
        {
            var result = original.MoveToFirstAttribute();
            Console.WriteLine($"MoveToFirstAttribute {result}");
            return result;
        }

        public override bool ReadAttributeValue()
        {
            var result = original.ReadAttributeValue();
            Console.WriteLine($"ReadAttributeValue {result}");
            return result;
        }

        public override string Value
        {
            get
            {
                var result = original.Value;
                Console.WriteLine($"Value {result}");
                return result;
            }
        }

        public override bool MoveToNextAttribute()
        {
            var result = original.MoveToNextAttribute();
            Console.WriteLine($"MoveToNextAttribute {result}");
            return result;
        }

        public override bool MoveToElement()
        {
            var result = original.MoveToElement();
            Console.WriteLine($"MoveToElement {result}");
            return result;
        }

        public override int AttributeCount
        {
            get
            {
                var result = original.AttributeCount;
                Console.WriteLine($"AttributeCount {result}");
                return result;
            }
        }

        public override int Depth
        {
            get
            {
                var result = original.Depth;
                Console.WriteLine($"Depth {result}");
                return result;
            }
        }

        public override bool EOF
        {
            get
            {
                var result = original.EOF;
                Console.WriteLine($"EOF {result}");
                return result;
            }
        }

        public override XmlNameTable NameTable
        {
            get
            {
                var result = original.NameTable;
                Console.WriteLine($"NameTable {result}");
                return result;
            }
        }

        public override string GetAttribute(int i)
        {
            var result = original.GetAttribute(i);
            Console.WriteLine($"GetAttribute {result}");
            return result;
        }

        public override string GetAttribute(string name)
        {
            var result = original.GetAttribute(name);
            Console.WriteLine($"GetAttribute {result}");
            return result;
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            var result = original.GetAttribute(name, namespaceURI);
            Console.WriteLine($"GetAttribute {result}");
            return result;
        }

        public override string LookupNamespace(string prefix)
        {
            var result = original.LookupNamespace(prefix);
            Console.WriteLine($"LookupNamespace {result}");
            return result;
        }

        public override bool MoveToAttribute(string name)
        {
            var result = original.MoveToAttribute(name);
            Console.WriteLine($"MoveToAttribute {result}");
            return result;
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            var result = original.MoveToAttribute(name, ns);
            Console.WriteLine($"MoveToAttribute {result}");
            return result;
        }

        public override void ResolveEntity()
        {
            Console.WriteLine($"ResolveEntity");
            original.ResolveEntity();
        }
    }
}
