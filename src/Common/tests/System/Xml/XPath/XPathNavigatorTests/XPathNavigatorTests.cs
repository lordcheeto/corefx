// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml.XPath;
using XPathTests.Common;
using Xunit;

namespace XPathTests.XPathNavigatorTests
{
    public class XPathNavigatorTests
    {
        [Fact]
        public void CreateNavigator()
        {
            XPathNavigator nav = Utils.CreateNavigatorFromFile("xp001.xml");
            
            Assert.NotNull(nav);
        }

        [Fact]
        public void PropertiesOnDocument()
        {
            XPathNavigator nav = Utils.CreateNavigatorFromFile("xp001.xml");

            Assert.Equal(XPathNodeType.Root, nav.NodeType);
            Assert.Equal(string.Empty, nav.Name);
            Assert.Equal(string.Empty, nav.LocalName);
            Assert.Equal(string.Empty, nav.NamespaceURI);
            Assert.Equal(string.Empty, nav.Prefix);
            Assert.True(!nav.HasAttributes);
            Assert.True(nav.HasChildren);
            Assert.True(!nav.IsEmptyElement);
        }

        [Fact]
        public void PropertiesOnElement()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo:bar xmlns:foo='#foo' />");
            Assert.True(nav.MoveToFirstChild());

            Assert.Equal(XPathNodeType.Element, nav.NodeType);
            Assert.Equal("foo:bar", nav.Name);
            Assert.Equal("bar", nav.LocalName);
            Assert.Equal("#foo", nav.NamespaceURI);
            Assert.Equal("foo", nav.Prefix);
            Assert.True(!nav.HasAttributes);
            Assert.True(!nav.HasChildren);
            Assert.True(nav.IsEmptyElement);
        }

        [Fact]
        public void PropertiesOnAttribute()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo bar:baz='quux' xmlns:bar='#bar' />");
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstAttribute());

            Assert.Equal(XPathNodeType.Attribute, nav.NodeType);
            Assert.Equal("bar:baz", nav.Name);
            Assert.Equal("baz", nav.LocalName);
            Assert.Equal("#bar", nav.NamespaceURI);
            Assert.Equal("bar", nav.Prefix);
            Assert.True(!nav.HasAttributes);
            Assert.True(!nav.HasChildren);
            Assert.True(!nav.IsEmptyElement);
        }

        [Fact]
        public void PropertiesOnNamespace()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root xmlns='urn:foo' />");
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstNamespace());

            Assert.Equal(XPathNodeType.Namespace, nav.NodeType);
        }

        [Fact]
        public void Navigation()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar /><baz /></foo>");
            Assert.True(nav.MoveToFirstChild());

            Assert.Equal("foo", nav.Name);
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal("bar", nav.Name);
            Assert.True(nav.MoveToNext());
            Assert.Equal("baz", nav.Name);
            Assert.True(!nav.MoveToNext());
            Assert.Equal("baz", nav.Name);
            Assert.True(nav.MoveToPrevious());
            Assert.Equal("bar", nav.Name);
            Assert.True(!nav.MoveToPrevious());
            Assert.True(nav.MoveToParent());
            Assert.Equal("foo", nav.Name);
            nav.MoveToRoot();
            Assert.Equal(XPathNodeType.Root, nav.NodeType);
            Assert.True(!nav.MoveToParent());
            Assert.Equal(XPathNodeType.Root, nav.NodeType);
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal("foo", nav.Name);
            Assert.True(nav.MoveToFirst());
            Assert.Equal("foo", nav.Name);
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal("bar", nav.Name);
            Assert.True(nav.MoveToNext());
            Assert.Equal("baz", nav.Name);
            Assert.True(nav.MoveToFirst());
            Assert.Equal("bar", nav.Name);
        }

        [Fact]
        public void AttributeNavigation()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo bar='baz' quux='quuux' />");
            Assert.True(nav.MoveToFirstChild());

            Assert.Equal(XPathNodeType.Element, nav.NodeType);
            Assert.Equal("foo", nav.Name);
            Assert.True(nav.MoveToFirstAttribute());
            Assert.Equal(XPathNodeType.Attribute, nav.NodeType);
            Assert.Equal("bar", nav.Name);
            Assert.Equal("baz", nav.Value);
            Assert.True(nav.MoveToNextAttribute());
            Assert.Equal(XPathNodeType.Attribute, nav.NodeType);
            Assert.Equal("quux", nav.Name);
            Assert.Equal("quuux", nav.Value);
        }

        [Fact]
        public void ElementAndRootValues()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar>baz</bar><quux>quuux</quux></foo>");
            Assert.True(nav.MoveToFirstChild());

            Assert.Equal(XPathNodeType.Element, nav.NodeType);
            Assert.Equal("foo", nav.Name);
            Assert.Equal("bazquuux", nav.Value);

            nav.MoveToRoot();
            Assert.Equal("bazquuux", nav.Value);
        }

        [Fact]
        public void DocumentWithXmlDeclaration()
        {
            XPathNavigator nav = Utils.CreateNavigator("<?xml version=\"1.0\" standalone=\"yes\"?><Root><foo>bar</foo></Root>");

            nav.MoveToRoot();
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(XPathNodeType.Element, nav.NodeType);
            Assert.Equal("Root", nav.Name);
        }

        [Fact]
        public void DocumentWithProcessingInstruction()
        {
            XPathNavigator nav = Utils.CreateNavigator("<?xml-stylesheet href='foo.xsl' type='text/xsl' ?><foo />");

            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(XPathNodeType.ProcessingInstruction, nav.NodeType);
            Assert.Equal("xml-stylesheet", nav.Name);

            XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.Element);
            Assert.Equal(0, iter.Count);
        }

        [Fact]
        public void ValueAsBoolean()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>1</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.ValueAsBoolean);
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.ValueAsBoolean);
        }

        [Fact]
        public void ValueAsBooleanFail()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>1.0</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Throws<FormatException>(() => nav.ValueAsBoolean);
        }

        [Fact]
        public void ValueAsDateTime()
        {
            DateTime time = new DateTime(2005, 12, 13);

            XPathNavigator nav = Utils.CreateNavigator("<root>2005-12-13</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(time, nav.ValueAsDateTime);
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(time, nav.ValueAsDateTime);
        }

        [Fact]
        public void ValueAsDateTimeFail()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>dating time</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Throws<FormatException>(() => nav.ValueAsDateTime);
        }

        [Fact]
        public void ValueAsDouble()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>3.14159265359</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(3.14159265359, nav.ValueAsDouble);
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(3.14159265359, nav.ValueAsDouble);
        }

        [Fact]
        public void ValueAsDoubleFail()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>Double Dealer</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Throws<FormatException>(() => nav.ValueAsDouble);
        }

        [Fact]
        public void ValueAsInt()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>1</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(1, nav.ValueAsInt);
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(1, nav.ValueAsInt);
        }

        [Fact]
        public void ValueAsIntFail()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>1.0</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Throws<FormatException>(() => nav.ValueAsInt);
        }

        [Fact]
        public void ValueAsLong()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>10000000000000000</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(10000000000000000, nav.ValueAsLong);
            Assert.True(nav.MoveToFirstChild());
            Assert.Equal(10000000000000000, nav.ValueAsLong);
        }

        [Fact]
        public void ValueAsLongFail()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root>0x10000000000000000</root>");

            Assert.True(nav.MoveToFirstChild());
            Assert.Throws<FormatException>(() => nav.ValueAsLong);
        }

        [Fact]
        public void MoveToFollowingNodeTypeAll()
        {
            XPathNavigator nav = Utils.CreateNavigator("<root><child/><child2/></root>");

            Assert.True(nav.MoveToFollowing(XPathNodeType.All));
            Assert.True(nav.MoveToFollowing(XPathNodeType.All));
            Assert.Equal("child", nav.LocalName);
            Assert.True(nav.MoveToNext(XPathNodeType.All));
            Assert.Equal("child2", nav.LocalName);
        }

        [Fact]
        public void InnerXmlOnRoot()
        {
            XPathNavigator nav = Utils.CreateNavigator(@"<test>
			<node>z</node>
			<node>a</node>
			<node>b</node>
			<node>q</node>
			</test>");

            Assert.Equal(nav.OuterXml, nav.InnerXml);
        }
    }
}
