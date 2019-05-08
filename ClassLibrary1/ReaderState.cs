﻿namespace ClassLibrary1
{
    internal enum ReaderState
    {
        Initial,
        Subject,
        Predicate,
        Object,
        EndSubject,
        EndPredicate,
        EndObject,
        About,
        AboutValue,
        ParseType,
        ParseTypeValue,
        CollectionPredicate,
        EndCollectionPredicate,
        CollectionObject,
        List,
        Literal,
        NodeId,
        NodeIdValue,
        RDF,
        Datatype,
        DatatypeValue,
        Language,
        LanguageValue,
        Prefix,
        PrefixValue,
    }
}