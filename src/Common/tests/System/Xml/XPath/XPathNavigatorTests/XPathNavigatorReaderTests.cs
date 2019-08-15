// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Xml.XPath;
using XPathTests.Common;
using Xunit;

namespace XPathTests.XPathNavigatorTests
{
    public class XPathNavigatorReaderTests
    {
        private void AssertNode(XmlReader xmlReader, XmlNodeType nodeType, int depth, bool isEmptyElement,
            string name, string prefix, string localName, string namespaceURI, string value, bool hasValue,
            int attributeCount, bool hasAttributes)
        {
            Assert.Equal(nodeType, xmlReader.NodeType);
            Assert.Equal(isEmptyElement, xmlReader.IsEmptyElement);
            Assert.Equal(name, xmlReader.Name);
            Assert.Equal(prefix, xmlReader.Prefix);
            Assert.Equal(localName, xmlReader.LocalName);
            Assert.Equal(namespaceURI, xmlReader.NamespaceURI);
            Assert.Equal(depth, xmlReader.Depth);
            Assert.Equal(hasValue, xmlReader.HasValue);
            Assert.Equal(value, xmlReader.Value);
            Assert.Equal(hasAttributes, xmlReader.HasAttributes);
            Assert.Equal(attributeCount, xmlReader.AttributeCount);
        }

        [Fact]
        public void ReadSubtree1()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root/>");
            ReadSubtree1(nav);

            nav.MoveToRoot();
            Assert.True(nav.MoveToFirstChild());
            ReadSubtree1(nav);
        }

        void ReadSubtree1(XPathNavigator nav)
        {
            XmlReader r = nav.ReadSubtree();
            AssertNode(r, XmlNodeType.None, 0, false, "", "", "", "", "", false, 0, false);

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.Element, 0, true, "root", "", "root", "", "", false, 0, false);

            Assert.False(r.Read());
        }

        [Fact]
        public void ReadSubtree2()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root></root>");
            ReadSubtree2(nav);

            nav.MoveToRoot();
            Assert.True(nav.MoveToFirstChild());
            ReadSubtree2(nav);
        }

        void ReadSubtree2(XPathNavigator nav)
        {
            XmlReader r = nav.ReadSubtree();
            AssertNode(r, XmlNodeType.None, 0, false, "", "", "", "", "", false, 0, false);

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.Element, 0, false, "root", "", "root", "", "", false, 0, false);

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.EndElement, 0, false, "root", "", "root", "", "", false, 0, false);

            Assert.False(r.Read());
        }

        [Fact]
        public void ReadSubtree3()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root attr='value'/>");
            ReadSubtree3(nav);

            nav.MoveToRoot();
            Assert.True(nav.MoveToFirstChild());
            ReadSubtree3(nav);
        }

        void ReadSubtree3(XPathNavigator nav)
        {
            XmlReader r = nav.ReadSubtree();
            AssertNode(r, XmlNodeType.None, 0, false, "", "", "", "", "", false, 0, false);

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.Element, 0, true, "root", "", "root", "", "", false, 1, true);

            Assert.True(r.MoveToFirstAttribute());
            AssertNode(r, XmlNodeType.Attribute, 1, false, "attr", "", "attr", "", "value", true, 1, true);

            Assert.True(r.ReadAttributeValue());
            AssertNode(r, XmlNodeType.Text, 2, false, "", "", "", "", "value", true, 1, true);

            Assert.False(r.ReadAttributeValue());
            Assert.False(r.MoveToNextAttribute());
            Assert.True(r.MoveToElement());

            Assert.False(r.Read());
        }

        [Fact]
        public void DocElem_OpenClose_Attribute()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root attr='value'></root>");
            DocElem_OpenClose_Attribute(nav);

            nav.MoveToRoot();
            Assert.True(nav.MoveToFirstChild());
            DocElem_OpenClose_Attribute(nav);
        }

        void DocElem_OpenClose_Attribute(XPathNavigator nav)
        {
            XmlReader r = nav.ReadSubtree();
            AssertNode(r, XmlNodeType.None, 0, false, "", "", "", "", "", false, 0, false);

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.Element, 0, false, "root", "", "root", "", "", false, 1, true);

            Assert.True(r.MoveToFirstAttribute());
            AssertNode(r, XmlNodeType.Attribute, 1, false, "attr", "", "attr", "", "value", true, 1, true);

            Assert.True(r.ReadAttributeValue());
            AssertNode(r, XmlNodeType.Text, 2, false, "", "", "", "", "value", true, 1, true);

            Assert.False(r.ReadAttributeValue());
            Assert.False(r.MoveToNextAttribute());
            Assert.True(r.MoveToElement());

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.EndElement, 0, false, "root", "", "root", "", "", false, 0, false);

            Assert.False(r.Read());
        }

        [Fact]
        public void FromChildElement()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root><foo attr='value'>test</foo><bar/></root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());

            XmlReader r = nav.ReadSubtree();
            AssertNode(r, XmlNodeType.None, 0, false, "", "", "", "", "", false, 0, false);

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.Element, 0, false, "foo", "", "foo", "", "", false, 1, true);

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.Text, 1, false, "", "", "", "", "test", true, 0, false);

            Assert.True(r.Read());
            AssertNode(r, XmlNodeType.EndElement, 0, false, "foo", "", "foo", "", "", false, 0, false);

            // end at </foo>, without moving toward <bar>.
            Assert.False(r.Read());
        }

        [Fact]
        public void MoveToFirstAttributeFromAttribute()
        {
            XPathNavigator nav = Utils.CreateNavigator(@"<one xmlns:foo='urn:foo' a='v' />");
            MoveToFirstAttributeFromAttribute(nav);

            nav.MoveToRoot();
            Assert.True(nav.MoveToFirstChild());
            MoveToFirstAttributeFromAttribute(nav);
        }

        void MoveToFirstAttributeFromAttribute(XPathNavigator nav)
        {
            XmlReader r = nav.ReadSubtree();
            r.MoveToContent();
            Assert.True(r.MoveToFirstAttribute());
            Assert.True(r.MoveToFirstAttribute());
            Assert.True(r.ReadAttributeValue());
            Assert.True(r.MoveToFirstAttribute());
            Assert.True(r.MoveToNextAttribute());
            Assert.True(r.MoveToFirstAttribute());
        }

        [Fact]
        public void ReadSubtreeAttribute()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root a='b' />");

            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstAttribute());
            Assert.Throws<InvalidOperationException>(() => nav.ReadSubtree());
        }

        [Fact]
        public void ReadSubtreeNamespace()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root xmlns='urn:foo' />");

            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstNamespace());
            Assert.Throws<InvalidOperationException>(() => nav.ReadSubtree());
        }

        [Fact]
        public void ReadSubtreePI()
        {
            XPathNavigator nav = Utils.CreateNavigator("<?pi ?><root xmlns='urn:foo' />");

            Assert.True(nav.MoveToFirstChild());
            Assert.Throws<InvalidOperationException>(() => nav.ReadSubtree());
        }

        [Fact]
        public void ReadSubtreeComment()
        {
            XPathNavigator nav = Utils.CreateNavigator("<!-- comment --><root xmlns='urn:foo' />");

            Assert.True(nav.MoveToFirstChild());
            Assert.Throws<InvalidOperationException>(() => nav.ReadSubtree());
        }

        [Fact]
        public void ReadSubtreeAttributesByIndex()
        {
            XPathNavigator nav = Utils.CreateNavigator("<u:Timestamp u:Id='ID1' xmlns:u='urn:foo'></u:Timestamp>");

            XmlReader r = nav.ReadSubtree();
            Assert.True(r.Read());
            r.MoveToAttribute(0);
            if (r.LocalName != "Id")
                r.MoveToAttribute(1);
            if (r.LocalName != "Id")
                Assert.True(false, "Should move to the attribute.");
        }
    }
}
