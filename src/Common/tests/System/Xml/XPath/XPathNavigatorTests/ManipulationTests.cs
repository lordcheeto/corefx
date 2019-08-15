// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml.XPath;
using XPathTests.Common;
using Xunit;

namespace XPathTests.XPathNavigatorTests
{
    public class ManipulationTests
    {
        private void AssertNavigator(XPathNavigator nav, XPathNodeType type, string prefix, string localName, string ns,
            string name, string value, bool hasAttributes, bool hasChildren, bool isEmptyElement)
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
        public void AppendChild()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/></foo>");

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);

            if (nav.CanEdit)
            {
                nav.AppendChild("<quux/>");
                Assert.True(nav.MoveToFirstChild());
                AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "qux", "", "qux", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "quux", "", "quux", "", false, false, true);
                Assert.False(nav.MoveToNext());
            }
            else
                Assert.Throws<NotSupportedException>(() => nav.AppendChild("<quux/>"));
        }

        [Fact]
        public void PrependChild()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/></foo>");

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);

            if (nav.CanEdit)
            {
                nav.PrependChild("<quux/>");
                Assert.True(nav.MoveToFirstChild());
                AssertNavigator(nav, XPathNodeType.Element, "", "quux", "", "quux", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "qux", "", "qux", "", false, false, true);
                Assert.False(nav.MoveToNext());
            }
            else
                Assert.Throws<NotSupportedException>(() => nav.PrependChild("<quux/>"));
        }

        [Fact]
        public void InsertAfter()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/></foo>");

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.True(nav.MoveToChild("baz", ""));
            AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);

            if (nav.CanEdit)
            {
                nav.InsertAfter("<quux/>");
                Assert.True(nav.MoveToParent());
                AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
                Assert.True(nav.MoveToFirstChild());
                AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "quux", "", "quux", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "qux", "", "qux", "", false, false, true);
                Assert.False(nav.MoveToNext());
            }
            else
                Assert.Throws<NotSupportedException>(() => nav.InsertAfter("<quux/>"));
        }

        [Fact]
        public void InsertBefore()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/></foo>");

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.True(nav.MoveToChild("baz", ""));
            AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);

            if (nav.CanEdit)
            {
                nav.InsertBefore("<quux/>");
                Assert.True(nav.MoveToParent());
                AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
                Assert.True(nav.MoveToFirstChild());
                AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "quux", "", "quux", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);
                Assert.True(nav.MoveToNext());
                AssertNavigator(nav, XPathNodeType.Element, "", "qux", "", "qux", "", false, false, true);
                Assert.False(nav.MoveToNext());
            }
            else
                Assert.Throws<NotSupportedException>(() => nav.InsertBefore("<quux/>"));
        }

        [Fact]
        public void ReplaceSelf()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/></foo>");

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);

            if (nav.CanEdit)
            {
                nav.ReplaceSelf("<baz/>");
                Assert.True(nav.MoveToParent());
                AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
                Assert.True(nav.MoveToFirstChild());
                AssertNavigator(nav, XPathNodeType.Element, "", "baz", "", "baz", "", false, false, true);
                Assert.False(nav.MoveToNext());
            }
            else
                Assert.Throws<NotSupportedException>(() => nav.ReplaceSelf("<baz/>"));
        }

        [Fact]
        public void DeleteSelf()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/></foo>");

            AssertNavigator(nav, XPathNodeType.Root, "", "", "", "", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, true, false);
            Assert.True(nav.MoveToFirstChild());
            AssertNavigator(nav, XPathNodeType.Element, "", "bar", "", "bar", "", false, false, true);

            if (nav.CanEdit)
            {
                nav.DeleteSelf();
                AssertNavigator(nav, XPathNodeType.Element, "", "foo", "", "foo", "", false, false, false);
            }
            else
                Assert.Throws<NotSupportedException>(() => nav.DeleteSelf());
        }
    }
}
