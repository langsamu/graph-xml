namespace UnitTestProject1
{
    using ClassLibrary1;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.IO;
    using System.Xml;
    using VDS.RDF;
    using VDS.RDF.Parsing;
    using VDS.RDF.Writing;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void NS()
        {
            Roundtrips(@"
@prefix x:<urn:/> .

x:s x:p x:o .
x:s <urn:a/p> x:o .
", "x:s");
        }

        [TestMethod]
        public void MyTestMethod()
        {
            using (var sr = new StringReader(@"<a><b></b></a>"))
            {
                using (var reader = new Wrapper(XmlReader.Create(sr)))
                {
                    using (var writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
                    {
                        writer.WriteNode(reader, false);
                    }
                }
            }
        }

        [TestMethod]
        public void Iri()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p :o .
");
        }

        [TestMethod]
        public void Iri2()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p :o1, :o2 .
");
        }

        [TestMethod]
        public void LiteralObject()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p ""o"" .
");
        }

        [TestMethod]
        public void IriAndLiteral()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p :o, ""o"" .
");
        }

        [TestMethod]
        public void Collection1Iri()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p (:o) .
");
        }

        [TestMethod]
        public void Collection2Iri()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p (:o1 :o2) .
");
        }

        [TestMethod]
        public void Collection2Literal()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p (""o1"" ""o2"") .
");
        }

        [TestMethod]
        public void CollectionIriLiteral()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p (:o ""o"") .
");
        }

        [TestMethod]
        public void Blank()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p [] .
");
        }

        [TestMethod]
        public void IriLiteralBlank()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p :o, ""o"", [] .
");
        }

        [TestMethod]
        public void All()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p (:o :o), (:o ""o"" []), :o1, ""o1"", [] .
");
        }

        [TestMethod]
        public void Collections2Iris()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p (:o :o), (:o :o) .
");
        }

        [TestMethod]
        public void X()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p ([:p :o]) .
");
        }

        [TestMethod]
        public void Ampersand()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s <http://www.w3.org/1999/02/22-rdf-syntax-ns#value> <http://example/q?abc=1&def=2> .
");
        }

        [TestMethod]
        public void Charmod()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p ""ü"" .
");
        }

        [TestMethod]
        public void CharmodUri()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p :ü .
");
        }

        [TestMethod]
        public void Recurse()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p :s .
");
        }

        [TestMethod]
        public void MultiParentBlank()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p _:a, :x .
:x :p _:a .
");
        }

        [TestMethod]
        public void Datatype()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p ""X""^^:datatype .
");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Empty()
        {
            Roundtrips(@"
@prefix :<urn:/> .
");
        }

        [TestMethod]
        public void Language()
        {
            Roundtrips(@"
@prefix :<urn:/> .

:s :p ""x""@en .
");
        }

        [TestMethod]
        public void XMLLiteral()
        {
            Assert.Fail("Implement https://www.w3.org/TR/rdf-syntax-grammar/#section-Syntax-XML-literals");

            Roundtrips(@"
@prefix :<urn:/> .

:s :p ""<x/>""^^<http://www.w3.org/1999/02/22-rdf-syntax-ns#XMLLiteral> .
");
        }

        [TestMethod]
        public void DianeAbbott()
        {
            var rdf = @"
@prefix :<https://id.parliament.uk/schema/> .
@prefix ex:<http://example.com/> .
@prefix id:<https://id.parliament.uk/> .

<https://id.parliament.uk/43RHonMf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/Person> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personGivenName> ""Diane"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personOtherNames> ""Julie"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personFamilyName> ""Abbott"" .
<https://id.parliament.uk/43RHonMf> <http://example.com/F31CBD81AD8343898B49DC65743F0BDF> ""Ms Diane Abbott"" .
<https://id.parliament.uk/43RHonMf> <http://example.com/D79B0BAC513C4A9A87C9D5AFF1FC632F> ""Rt Hon Diane Abbott MP"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasMemberImage> <https://id.parliament.uk/S3bGSTqn> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/partyMemberHasPartyMembership> <https://id.parliament.uk/UcFeoI5t> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasParliamentaryIncumbency> <https://id.parliament.uk/5b1mxVJ7> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personHasFormalBodyMembership> <https://id.parliament.uk/VHM0rBq1> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/oppositionPersonHasOppositionIncumbency> <https://id.parliament.uk/wE8Hq016> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personPimsId> ""3572"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberMnisId> ""172"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personHasGenderIdentity> <https://id.parliament.uk/SPRKaz3b> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personHasPersonalWebLink> <http://www.dianeabbott.org.uk/> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personHasTwitterWebLink> <https://twitter.com/HackneyAbbott> .
<https://id.parliament.uk/SPRKaz3b> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/GenderIdentity> .
<https://id.parliament.uk/SPRKaz3b> <https://id.parliament.uk/schema/genderIdentityHasGender> <https://id.parliament.uk/Hnymnoxw> .
<https://id.parliament.uk/Hnymnoxw> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/Gender> .
<https://id.parliament.uk/Hnymnoxw> <https://id.parliament.uk/schema/genderName> ""Female"" .
<https://id.parliament.uk/S3bGSTqn> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/MemberImage> .
<https://id.parliament.uk/HSoMS1VX> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/ConstituencyGroup> .
<https://id.parliament.uk/HSoMS1VX> <https://id.parliament.uk/schema/constituencyGroupName> ""Hackney North and Stoke Newington"" .
<https://id.parliament.uk/HSoMS1VX> <https://id.parliament.uk/schema/constituencyGroupStartDate> ""1997-05-01+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/HSoMS1VX> <https://id.parliament.uk/schema/constituencyGroupEndDate> ""2010-05-06+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/5b1mxVJ7> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/SeatIncumbency> .
<https://id.parliament.uk/5b1mxVJ7> <https://id.parliament.uk/schema/parliamentaryIncumbencyEndDate> ""2005-05-05+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/5b1mxVJ7> <https://id.parliament.uk/schema/parliamentaryIncumbencyStartDate> ""2001-06-07+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/5b1mxVJ7> <https://id.parliament.uk/schema/seatIncumbencyHasHouseSeat> <https://id.parliament.uk/L71LLeyL> .
<https://id.parliament.uk/L71LLeyL> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/HouseSeat> .
<https://id.parliament.uk/L71LLeyL> <https://id.parliament.uk/schema/houseSeatHasConstituencyGroup> <https://id.parliament.uk/HSoMS1VX> .
<https://id.parliament.uk/L71LLeyL> <https://id.parliament.uk/schema/houseSeatHasHouse> <https://id.parliament.uk/1AFu55Hs> .
<https://id.parliament.uk/LEYIBvV9> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/Party> .
<https://id.parliament.uk/LEYIBvV9> <https://id.parliament.uk/schema/partyName> ""Labour"" .
<https://id.parliament.uk/UcFeoI5t> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/PartyMembership> .
<https://id.parliament.uk/UcFeoI5t> <https://id.parliament.uk/schema/partyMembershipStartDate> ""2017-06-08+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/UcFeoI5t> <https://id.parliament.uk/schema/partyMembershipHasParty> <https://id.parliament.uk/LEYIBvV9> .
<https://id.parliament.uk/1AFu55Hs> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/House> .
<https://id.parliament.uk/1AFu55Hs> <https://id.parliament.uk/schema/houseName> ""House of Commons"" .
<https://id.parliament.uk/VHM0rBq1> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBodyMembership> .
<https://id.parliament.uk/VHM0rBq1> <https://id.parliament.uk/schema/formalBodyMembershipStartDate> ""1995-11-15+00:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/VHM0rBq1> <https://id.parliament.uk/schema/formalBodyMembershipEndDate> ""1997-03-21+00:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/VHM0rBq1> <https://id.parliament.uk/schema/formalBodyMembershipHasFormalBody> <https://id.parliament.uk/cLjFRjRt> .
<https://id.parliament.uk/cLjFRjRt> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBody> .
<https://id.parliament.uk/cLjFRjRt> <https://id.parliament.uk/schema/formalBodyName> ""Treasury Committee"" .
<https://id.parliament.uk/wE8Hq016> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/OppositionIncumbency> .
<https://id.parliament.uk/wE8Hq016> <https://id.parliament.uk/schema/incumbencyStartDate> ""2016-06-27+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/wE8Hq016> <https://id.parliament.uk/schema/incumbencyEndDate> ""2016-10-06+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/wE8Hq016> <https://id.parliament.uk/schema/oppositionIncumbencyHasOppositionPosition> <https://id.parliament.uk/skJwXTDQ> .
<https://id.parliament.uk/skJwXTDQ> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/OppositionPosition> .
<https://id.parliament.uk/skJwXTDQ> <https://id.parliament.uk/schema/positionName> ""Shadow Secretary of State for Health"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/oppositionPersonHasOppositionIncumbency> <https://id.parliament.uk/2SdmzfSs> .
<https://id.parliament.uk/2SdmzfSs> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/OppositionIncumbency> .
<https://id.parliament.uk/2SdmzfSs> <https://id.parliament.uk/schema/incumbencyStartDate> ""2016-10-06+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/2SdmzfSs> <https://id.parliament.uk/schema/oppositionIncumbencyHasOppositionPosition> <https://id.parliament.uk/yYN7V6yX> .
<https://id.parliament.uk/yYN7V6yX> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/OppositionPosition> .
<https://id.parliament.uk/yYN7V6yX> <https://id.parliament.uk/schema/positionName> ""Shadow Home Secretary"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/oppositionPersonHasOppositionIncumbency> <https://id.parliament.uk/ZiqjL3DQ> .
<https://id.parliament.uk/ZiqjL3DQ> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/OppositionIncumbency> .
<https://id.parliament.uk/ZiqjL3DQ> <https://id.parliament.uk/schema/incumbencyStartDate> ""2010-10-08+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/ZiqjL3DQ> <https://id.parliament.uk/schema/incumbencyEndDate> ""2013-10-07+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/ZiqjL3DQ> <https://id.parliament.uk/schema/oppositionIncumbencyHasOppositionPosition> <https://id.parliament.uk/3YwE0NhM> .
<https://id.parliament.uk/3YwE0NhM> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/OppositionPosition> .
<https://id.parliament.uk/3YwE0NhM> <https://id.parliament.uk/schema/positionName> ""Shadow Minister (Public Health)"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/oppositionPersonHasOppositionIncumbency> <https://id.parliament.uk/xi9CFNH9> .
<https://id.parliament.uk/xi9CFNH9> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/OppositionIncumbency> .
<https://id.parliament.uk/xi9CFNH9> <https://id.parliament.uk/schema/incumbencyStartDate> ""2015-09-14+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/xi9CFNH9> <https://id.parliament.uk/schema/incumbencyEndDate> ""2016-06-27+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/xi9CFNH9> <https://id.parliament.uk/schema/oppositionIncumbencyHasOppositionPosition> <https://id.parliament.uk/b8cXZPwI> .
<https://id.parliament.uk/b8cXZPwI> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/OppositionPosition> .
<https://id.parliament.uk/b8cXZPwI> <https://id.parliament.uk/schema/positionName> ""Shadow Secretary of State for International Development"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personHasFormalBodyMembership> <https://id.parliament.uk/uzym0vom> .
<https://id.parliament.uk/uzym0vom> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBodyMembership> .
<https://id.parliament.uk/uzym0vom> <https://id.parliament.uk/schema/formalBodyMembershipStartDate> ""1997-11-25+00:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/uzym0vom> <https://id.parliament.uk/schema/formalBodyMembershipEndDate> ""1998-11-19+00:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/uzym0vom> <https://id.parliament.uk/schema/formalBodyMembershipHasFormalBody> <https://id.parliament.uk/HKmf0Ca7> .
<https://id.parliament.uk/HKmf0Ca7> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBody> .
<https://id.parliament.uk/HKmf0Ca7> <https://id.parliament.uk/schema/formalBodyName> ""Foreign Affairs: Entry Clearance Sub-Committee"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personHasFormalBodyMembership> <https://id.parliament.uk/IsEMoAUF> .
<https://id.parliament.uk/IsEMoAUF> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBodyMembership> .
<https://id.parliament.uk/IsEMoAUF> <https://id.parliament.uk/schema/formalBodyMembershipStartDate> ""1989-05-17+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/IsEMoAUF> <https://id.parliament.uk/schema/formalBodyMembershipEndDate> ""1995-11-08+00:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/IsEMoAUF> <https://id.parliament.uk/schema/formalBodyMembershipHasFormalBody> <https://id.parliament.uk/wXqxbB2N> .
<https://id.parliament.uk/wXqxbB2N> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBody> .
<https://id.parliament.uk/wXqxbB2N> <https://id.parliament.uk/schema/formalBodyName> ""Treasury & Civil Service Sub-Committee"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personHasFormalBodyMembership> <https://id.parliament.uk/EYQCxVic> .
<https://id.parliament.uk/EYQCxVic> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBodyMembership> .
<https://id.parliament.uk/EYQCxVic> <https://id.parliament.uk/schema/formalBodyMembershipStartDate> ""1989-05-15+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/EYQCxVic> <https://id.parliament.uk/schema/formalBodyMembershipEndDate> ""1995-11-08+00:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/EYQCxVic> <https://id.parliament.uk/schema/formalBodyMembershipHasFormalBody> <https://id.parliament.uk/ZSJXcWE1> .
<https://id.parliament.uk/ZSJXcWE1> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBody> .
<https://id.parliament.uk/ZSJXcWE1> <https://id.parliament.uk/schema/formalBodyName> ""Treasury & Civil Service"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/personHasFormalBodyMembership> <https://id.parliament.uk/UXpLpmfS> .
<https://id.parliament.uk/UXpLpmfS> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBodyMembership> .
<https://id.parliament.uk/UXpLpmfS> <https://id.parliament.uk/schema/formalBodyMembershipStartDate> ""1997-07-16+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/UXpLpmfS> <https://id.parliament.uk/schema/formalBodyMembershipEndDate> ""2001-05-11+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/UXpLpmfS> <https://id.parliament.uk/schema/formalBodyMembershipHasFormalBody> <https://id.parliament.uk/fYbWHWhk> .
<https://id.parliament.uk/fYbWHWhk> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/FormalBody> .
<https://id.parliament.uk/fYbWHWhk> <https://id.parliament.uk/schema/formalBodyName> ""Foreign Affairs Committee"" .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/partyMemberHasPartyMembership> <https://id.parliament.uk/GmWQzr0b> .
<https://id.parliament.uk/GmWQzr0b> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/PartyMembership> .
<https://id.parliament.uk/GmWQzr0b> <https://id.parliament.uk/schema/partyMembershipStartDate> ""1987-06-11+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/GmWQzr0b> <https://id.parliament.uk/schema/partyMembershipEndDate> ""2015-03-30+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/GmWQzr0b> <https://id.parliament.uk/schema/partyMembershipHasParty> <https://id.parliament.uk/LEYIBvV9> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/partyMemberHasPartyMembership> <https://id.parliament.uk/JWf4RycJ> .
<https://id.parliament.uk/JWf4RycJ> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/PartyMembership> .
<https://id.parliament.uk/JWf4RycJ> <https://id.parliament.uk/schema/partyMembershipStartDate> ""2015-05-07+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/JWf4RycJ> <https://id.parliament.uk/schema/partyMembershipEndDate> ""2017-05-03+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/JWf4RycJ> <https://id.parliament.uk/schema/partyMembershipHasParty> <https://id.parliament.uk/LEYIBvV9> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasParliamentaryIncumbency> <https://id.parliament.uk/P8dV98n8> .
<https://id.parliament.uk/fy1cWNCD> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/ContactPoint> .
<https://id.parliament.uk/fy1cWNCD> <https://id.parliament.uk/schema/email> ""diane.abbott.office@parliament.uk"" .
<https://id.parliament.uk/fy1cWNCD> <https://id.parliament.uk/schema/phoneNumber> ""020 7219 4426"" .
<https://id.parliament.uk/fy1cWNCD> <https://id.parliament.uk/schema/contactPointHasPostalAddress> <https://id.parliament.uk/uBuD8ZqA> .
<https://id.parliament.uk/uBuD8ZqA> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/PostalAddress> .
<https://id.parliament.uk/uBuD8ZqA> <https://id.parliament.uk/schema/addressLine1> ""House of Commons"" .
<https://id.parliament.uk/uBuD8ZqA> <https://id.parliament.uk/schema/addressLine2> ""London"" .
<https://id.parliament.uk/uBuD8ZqA> <https://id.parliament.uk/schema/postCode> ""SW1A 0AA"" .
<https://id.parliament.uk/5bX5Se0u> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/ConstituencyGroup> .
<https://id.parliament.uk/5bX5Se0u> <https://id.parliament.uk/schema/constituencyGroupName> ""Hackney North and Stoke Newington"" .
<https://id.parliament.uk/5bX5Se0u> <https://id.parliament.uk/schema/constituencyGroupStartDate> ""2010-05-06+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/P8dV98n8> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/SeatIncumbency> .
<https://id.parliament.uk/P8dV98n8> <https://id.parliament.uk/schema/parliamentaryIncumbencyStartDate> ""2017-06-08+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/P8dV98n8> <https://id.parliament.uk/schema/seatIncumbencyHasHouseSeat> <https://id.parliament.uk/0BhROnYP> .
<https://id.parliament.uk/P8dV98n8> <https://id.parliament.uk/schema/parliamentaryIncumbencyHasContactPoint> <https://id.parliament.uk/fy1cWNCD> .
<https://id.parliament.uk/0BhROnYP> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/HouseSeat> .
<https://id.parliament.uk/0BhROnYP> <https://id.parliament.uk/schema/houseSeatHasConstituencyGroup> <https://id.parliament.uk/5bX5Se0u> .
<https://id.parliament.uk/0BhROnYP> <https://id.parliament.uk/schema/houseSeatHasHouse> <https://id.parliament.uk/1AFu55Hs> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasParliamentaryIncumbency> <https://id.parliament.uk/FC1IDBx2> .
<https://id.parliament.uk/FC1IDBx2> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/SeatIncumbency> .
<https://id.parliament.uk/FC1IDBx2> <https://id.parliament.uk/schema/parliamentaryIncumbencyEndDate> ""2001-06-07+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/FC1IDBx2> <https://id.parliament.uk/schema/parliamentaryIncumbencyStartDate> ""1997-05-01+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/FC1IDBx2> <https://id.parliament.uk/schema/seatIncumbencyHasHouseSeat> <https://id.parliament.uk/L71LLeyL> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasParliamentaryIncumbency> <https://id.parliament.uk/PfWrzef7> .
<https://id.parliament.uk/PfWrzef7> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/SeatIncumbency> .
<https://id.parliament.uk/PfWrzef7> <https://id.parliament.uk/schema/parliamentaryIncumbencyEndDate> ""2010-05-06+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/PfWrzef7> <https://id.parliament.uk/schema/parliamentaryIncumbencyStartDate> ""2005-05-05+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/PfWrzef7> <https://id.parliament.uk/schema/seatIncumbencyHasHouseSeat> <https://id.parliament.uk/L71LLeyL> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasParliamentaryIncumbency> <https://id.parliament.uk/TuxAdhNO> .
<https://id.parliament.uk/p869pBTf> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/ConstituencyGroup> .
<https://id.parliament.uk/p869pBTf> <https://id.parliament.uk/schema/constituencyGroupName> ""Hackney North and Stoke Newington"" .
<https://id.parliament.uk/p869pBTf> <https://id.parliament.uk/schema/constituencyGroupStartDate> ""1983-06-09+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/p869pBTf> <https://id.parliament.uk/schema/constituencyGroupEndDate> ""1997-05-01+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/TuxAdhNO> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/SeatIncumbency> .
<https://id.parliament.uk/TuxAdhNO> <https://id.parliament.uk/schema/parliamentaryIncumbencyEndDate> ""1997-05-01+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/TuxAdhNO> <https://id.parliament.uk/schema/parliamentaryIncumbencyStartDate> ""1992-04-09+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/TuxAdhNO> <https://id.parliament.uk/schema/seatIncumbencyHasHouseSeat> <https://id.parliament.uk/8khvuL6U> .
<https://id.parliament.uk/8khvuL6U> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/HouseSeat> .
<https://id.parliament.uk/8khvuL6U> <https://id.parliament.uk/schema/houseSeatHasConstituencyGroup> <https://id.parliament.uk/p869pBTf> .
<https://id.parliament.uk/8khvuL6U> <https://id.parliament.uk/schema/houseSeatHasHouse> <https://id.parliament.uk/1AFu55Hs> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasParliamentaryIncumbency> <https://id.parliament.uk/9jEv9wQr> .
<https://id.parliament.uk/9jEv9wQr> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/SeatIncumbency> .
<https://id.parliament.uk/9jEv9wQr> <https://id.parliament.uk/schema/parliamentaryIncumbencyEndDate> ""2015-03-30+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/9jEv9wQr> <https://id.parliament.uk/schema/parliamentaryIncumbencyStartDate> ""2010-05-06+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/9jEv9wQr> <https://id.parliament.uk/schema/seatIncumbencyHasHouseSeat> <https://id.parliament.uk/0BhROnYP> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasParliamentaryIncumbency> <https://id.parliament.uk/yVNL0a8Z> .
<https://id.parliament.uk/yVNL0a8Z> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/SeatIncumbency> .
<https://id.parliament.uk/yVNL0a8Z> <https://id.parliament.uk/schema/parliamentaryIncumbencyEndDate> ""1992-04-09+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/yVNL0a8Z> <https://id.parliament.uk/schema/parliamentaryIncumbencyStartDate> ""1987-06-11+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/yVNL0a8Z> <https://id.parliament.uk/schema/seatIncumbencyHasHouseSeat> <https://id.parliament.uk/8khvuL6U> .
<https://id.parliament.uk/43RHonMf> <https://id.parliament.uk/schema/memberHasParliamentaryIncumbency> <https://id.parliament.uk/DqWmAFwg> .
<https://id.parliament.uk/DqWmAFwg> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <https://id.parliament.uk/schema/SeatIncumbency> .
<https://id.parliament.uk/DqWmAFwg> <https://id.parliament.uk/schema/parliamentaryIncumbencyEndDate> ""2017-05-03+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/DqWmAFwg> <https://id.parliament.uk/schema/parliamentaryIncumbencyStartDate> ""2015-05-07+01:00""^^<http://www.w3.org/2001/XMLSchema#date> .
<https://id.parliament.uk/DqWmAFwg> <https://id.parliament.uk/schema/seatIncumbencyHasHouseSeat> <https://id.parliament.uk/0BhROnYP> .
";

            Roundtrips(rdf, "id:43RHonMf");
        }

        private static void Roundtrips(string rdf)
        {
            Roundtrips(rdf, ":s");
        }

        private static void Roundtrips(string rdf, string n)
        {
            var expected = new Graph();
            expected.LoadFromString(rdf);

            Process(expected.GetUriNode(n), Console.Out);

            Console.WriteLine();
            Console.WriteLine();

            var actual = Load(expected.GetUriNode(n));
            actual.SaveToStream(Console.Out, new RdfXmlWriter());

            Assert.AreEqual(expected, actual);
        }

        private static IGraph Load(INode n)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    Process(n, writer);

                    stream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(stream))
                    {
                        var g = new Graph();
                        new RdfXmlParser().Load(g, reader);
                        return g;
                    }
                }
            }
        }

        private static void Process(INode n, TextWriter stream)
        {
            using (var reader = new Reader(n))
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
                {
                    writer.WriteNode(reader, false);
                }
            }
            //using (var reader = new Reader(n))
            //{
            //    var x = new XmlDocument();
            //    x.Load(reader);
            //    x.Save(XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }));
            //}
        }
    }
}
