// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.XPath;
using XPathTests.Common;
using Xunit;

namespace XPathTests.XPathNavigatorTests
{
    public class XPathNavigatorCommonTests
    {
        private void AssertNavigator(XPathNavigator nav, XPathNodeType type, string prefix, string localName, string ns, string name, string value, bool hasAttributes, bool hasChildren, bool isEmptyElement)
        {
            Assert.Equal(type, nav.NodeType);
            Assert.Equal(prefix, nav.Prefix);
            Assert.Equal(localName, nav.LocalName);
            Assert.Equal(ns, nav.NamespaceURI);
            Assert.Equal(name, nav.Name);
            Assert.Equal(value, nav.Value);
            Assert.Equal(hasAttributes, nav.HasAttributes);
            Assert.Equal(hasChildren, nav.HasChildren);
            Assert.Equal(isEmptyElement, nav.IsEmptyElement);
        }

        [Fact]
        public void XmlRootElementOnly()
        {
            string xml = "<foo />";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, false, true);
            Assert.True(!nav.MoveToFirstChild());
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());
            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(!nav.MoveToNext());
        }

        [Fact]
        public void XmlSimpleTextContent()
        {
            string xml = "<foo>Test.</foo>";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "Test.", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "Test.", false, true, false);
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Text, "", "", "", "", "Test.", false, false, false);

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "Test.", false, true, false);

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "Test.", false, true, false);

            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());
            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "Test.", false, true, false);
            Assert.True(!nav.MoveToNext());
        }

        [Fact]
        public void XmlSimpleElementContent()
        {
            string xml = "<foo><bar /></foo>";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(!nav.MoveToNext());
        }

        [Fact]
        public void XmlTwoElementsContent()
        {
            string xml = "<foo><bar /><baz /></foo>";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);
            Assert.True(!nav.MoveToFirstChild());

            Assert.True(nav.MoveToNext());
            AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);
            Assert.True(!nav.MoveToFirstChild());

            Assert.True(nav.MoveToPrevious());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(!nav.MoveToNext());
        }

        [Fact]
        public void XmlElementWithAttributes()
        {
            string xml = "<img src='foo.png' alt='image Fooooooo!' />";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "img", "", "img", "", true, false, true);
            Assert.True(!nav.MoveToNext());
            Assert.True(!nav.MoveToPrevious());

            Assert.True(nav.MoveToFirstAttribute());
            AssertNavigator(nav, XPathNodeType.Attribute, "", "src", "", "src", "foo.png", false, false, false);
            Assert.True(!nav.MoveToFirstAttribute()); // On attributes, it fails.

            Assert.True(nav.MoveToNextAttribute());
            AssertNavigator(nav, XPathNodeType.Attribute, "", "alt", "", "alt", "image Fooooooo!", false, false, false);
            Assert.True(!nav.MoveToNextAttribute());

            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "img", "", "img", "", true, false, true);

            Assert.True(nav.MoveToAttribute("alt", ""));
            AssertNavigator(nav, XPathNodeType.Attribute, "", "alt", "", "alt", "image Fooooooo!", false, false, false);
            Assert.True(!nav.MoveToAttribute("src", "")); // On attributes, it fails.
            Assert.True(nav.MoveToParent());
            Assert.True(nav.MoveToAttribute("src", ""));
            AssertNavigator(nav, XPathNodeType.Attribute, "", "src", "", "src", "foo.png", false, false, false);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
        }

        [Fact]
        public void XmlNamespaceNode()
        {
            string xml = "<html xmlns='http://www.w3.org/1999/xhtml'><body>test.</body></html>";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            string xhtml = "http://www.w3.org/1999/xhtml";
            string xmlNS = "http://www.w3.org/XML/1998/namespace";
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "html", xhtml, "html", "test.", false, true, false);
            Assert.True(nav.MoveToFirstNamespace(XPathNamespaceScope.Local));
            AssertNavigator(nav, XPathNodeType.Namespace, "", "", "", "", xhtml, false, false, false);

            // Test difference between Local, ExcludeXml and All.
            Assert.True(!nav.MoveToNextNamespace(XPathNamespaceScope.Local));
            Assert.True(!nav.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml));
            
            Assert.True(nav.MoveToNextNamespace(XPathNamespaceScope.All));
            AssertNavigator(nav, XPathNodeType.Namespace, "", "xml", "", "xml", xmlNS, false, false, false);
            Assert.True(!nav.MoveToNextNamespace(XPathNamespaceScope.All));

            // Test to check if MoveToRoot() resets Namespace node status.
            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "test.", false, true, false);
            Assert.True(nav.MoveToFirstChild());

            // Test without XPathNamespaceScope argument.
            Assert.True(nav.MoveToFirstNamespace());
            Assert.True(nav.MoveToNextNamespace());
            AssertNavigator(nav, XPathNodeType.Namespace, "", "xml", "", "xml", xmlNS, false, false, false);

            // Test MoveToParent()
            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "html", xhtml, "html", "test.", false, true, false);

            Assert.True(nav.MoveToFirstChild());

            // Test difference between Local and ExcludeXml
            Assert.True(!nav.MoveToFirstNamespace(XPathNamespaceScope.Local), "Local should fail");
            Assert.True(nav.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml), "ExcludeXml should succeed");
            AssertNavigator(nav, XPathNodeType.Namespace, "", "", "", "", xhtml, false, false, false);

            Assert.True(nav.MoveToNextNamespace(XPathNamespaceScope.All));
            AssertNavigator(nav, XPathNodeType.Namespace, "", "xml", "", "xml", xmlNS, false, false, false);
            Assert.True(nav.MoveToParent());
            AssertNavigator(nav, XPathNodeType.Element, "", "body", xhtml, "body", "test.", false, true, false);

            nav.MoveToRoot();
            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "test.", false, true, false);
        }

        [Fact]
        public void MoveToNamespaces()
        {
            string xml = "<a xmlns:x='urn:x'><b xmlns:y='urn:y'/><c/><d><e attr='a'/></d></a>";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            XPathNodeIterator iter = nav.Select("//e");
            Assert.True(iter.MoveNext());
            Assert.True(nav.MoveTo(iter.Current));
            Assert.Equal("e", nav.Name);
            Assert.True(nav.MoveToFirstNamespace());
            Assert.Equal("x", nav.Name);
            Assert.True(nav.MoveToNextNamespace());
            Assert.Equal("xml", nav.Name);
        }

        [Fact]
        public void IsDescendant()
        {
            string xml = "<a><b/><c/><d><e attr='a'/></d></a>";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            XPathNavigator tmp = nav.Clone();
            XPathNodeIterator iter = nav.Select("//e");
            Assert.True(iter.MoveNext());
            Assert.True(nav.MoveTo(iter.Current));
            Assert.True(nav.MoveToFirstAttribute());
            Assert.Equal("attr", nav.Name);
            Assert.Equal("", tmp.Name);
            Assert.True(tmp.IsDescendant(nav));
            Assert.True(!nav.IsDescendant(tmp));
            Assert.True(tmp.MoveToFirstChild());
            Assert.Equal("a", tmp.Name);
            Assert.True(tmp.IsDescendant(nav));
            Assert.True(!nav.IsDescendant(tmp));
            Assert.True(tmp.MoveTo(iter.Current));
            Assert.Equal("e", tmp.Name);
            Assert.True(tmp.IsDescendant(nav));
            Assert.True(!nav.IsDescendant(tmp));
        }

        [Fact]
        public void LiterallySplittedText()
        {
            string xml = "<root><![CDATA[test]]> string</root>";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(XPathNodeType.Text, nav.NodeType);
            Assert.Equal("test string", nav.Value);
        }

        [Fact]
        public void SelectChildren()
        {
            string xml = "<root><foo xmlns='urn:foo' /><ns:foo xmlns:ns='urn:foo' /></root>";
            XPathNavigator nav = Utils.CreateNavigator(xml);

            Assert.True(nav.MoveToFirstChild());
            XPathNodeIterator iter = nav.SelectChildren("foo", "urn:foo");
            Assert.Equal(2, iter.Count);
        }
    }
}
