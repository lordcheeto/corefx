// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.XPath;
using XPathTests.Common;
using Xunit;

namespace XPathTests.XPathNavigatorTests
{
    public class XPathNavigatorMatchesTests
    {
        [Fact]
        public void MatchRoot()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo />");
            Assert.True(nav.Matches("/"));
        }

        [Fact]
        public void FalseMatchRoot()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo />");
            Assert.True(!nav.Matches("foo"));
        }

        [Fact]
        public void MatchDocumentElement()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo />");
            Assert.True(nav.MoveToFirstChild());

            Assert.True(nav.Matches("foo"));
        }

        [Fact]
        public void MatchAbsoluteDocumentElement()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo />");
            Assert.True(nav.MoveToFirstChild());

            Assert.True(nav.Matches("/foo"));
        }

        [Fact]
        public void MatchDocumentElementChild()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar /></foo>");
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());

            Assert.True(nav.Matches("bar"));
            Assert.True(nav.Matches("foo/bar"));
        }

        [Fact]
        public void MatchAttribute()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo bar='baz' />");
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstAttribute());

            Assert.True(nav.Matches("@bar"));
            Assert.True(nav.Matches("foo/@bar"));
        }

        [Fact]
        public void SlashSlash()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar><baz/></bar></foo>");
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());

            Assert.True(nav.Matches("foo//baz"));
        }

        [Fact]
        public void AbsoluteSlashSlash()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar><baz/></bar></foo>");
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());

            Assert.True(nav.Matches("//baz"));
        }

        [Fact]
        public void MatchDocumentElementWithPredicate()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar /></foo>");
            Assert.True(nav.MoveToFirstChild());

            Assert.True(nav.Matches("foo[bar]"));
        }

        [Fact]
        public void FalseMatchDocumentElementWithPredicate()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar /></foo>");
            Assert.True(nav.MoveToFirstChild());

            Assert.True(!nav.Matches("foo[baz]"));
        }

        [Fact]
        public void MatchesAncestorsButNotCurrent()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar><baz/></bar></foo>");
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());
            Assert.True(nav.MoveToFirstChild());

            Assert.True(nav.Matches("baz"));
            Assert.True(nav.Matches("bar/baz"));
            Assert.True(!nav.Matches("foo/bar"));
        }

        [Fact]
        public void MatchesParentAxis()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo/>");
            Assert.Throws<XPathException>(() => nav.Matches(".."));
        }

        [Fact]
        public void MatchesPredicatedParentAxis()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo/>");
            Assert.Throws<XPathException>(() => nav.Matches("..[1]"));
        }
    }
}
