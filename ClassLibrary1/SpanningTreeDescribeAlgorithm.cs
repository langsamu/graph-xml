namespace ClassLibrary1
{
    using System;
    using System.Collections.Generic;
    using VDS.RDF;
    using VDS.RDF.Query;

    // TODO: Implement
    // Could be used to narrow input to Reader
    internal class SpanningTreeDescribeAlgorithm : VDS.RDF.Query.Describe.BaseDescribeAlgorithm
    {
        protected override void DescribeInternal(IRdfHandler handler, SparqlEvaluationContext context, IEnumerable<INode> nodes)
        {
            throw new NotImplementedException();
        }
    }
}
