// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml.XPath;
using XPathTests.Common;
using Xunit;

namespace XPathTests.XPathNavigatorTests
{
    public class XPathNavigatorEvaluateTests
    {
        [Fact]
        public void CoreFunctionNodeSetLast()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/><squonk/></foo>");
            XPathNavigator nav2 = Utils.CreateNavigator("<foo><bar baz='1'/><bar baz='2'/><bar baz='3'/></foo>");
            XPathExpression expr = nav.Compile("last()");
            XPathNodeIterator iter = nav.Select("/foo");

            Assert.Equal("1", nav.Evaluate("last()").ToString());
            Assert.Equal("1", nav.Evaluate(expr, null).ToString());
            Assert.Equal("1", nav.Evaluate(expr, iter).ToString());
            iter = nav.Select("/foo/*");
            Assert.Equal("4", nav.Evaluate(expr, iter).ToString());
            Assert.Equal("3", nav2.Evaluate("string(//bar[last()]/@baz)"));
        }

        [Fact]
        public void CoreFunctionNodeSetPosition()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/><squonk/></foo>");
            XPathExpression expr = nav.Compile("position()");
            XPathNodeIterator iter = nav.Select("/foo");

            Assert.Equal("1", nav.Evaluate("position()").ToString());
            Assert.Equal("1", nav.Evaluate(expr, null).ToString());
            Assert.Equal("0", nav.Evaluate(expr, iter).ToString());
            iter = nav.Select("/foo/*");
            Assert.Equal("0", nav.Evaluate(expr, iter).ToString());
            Assert.True(iter.MoveNext());
            Assert.Equal("1", nav.Evaluate(expr, iter).ToString());
            Assert.True(iter.MoveNext());
            Assert.Equal("2", nav.Evaluate(expr, iter).ToString());
            Assert.True(iter.MoveNext());
            Assert.Equal("3", nav.Evaluate(expr, iter).ToString());
        }

        [Fact]
        public void CoreFunctionNodeSetCount()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/><squonk/></foo>");
            XPathNavigator nav2 = Utils.CreateNavigator("<foo><bar baz='1'/><bar baz='2'/><bar baz='3'/></foo>");

            Assert.Equal("5", nav.Evaluate("count(//*)").ToString());
            Assert.Equal("1", nav.Evaluate("count(//foo)").ToString());
            Assert.Equal("1", nav.Evaluate("count(/foo)").ToString());
            Assert.Equal("1", nav.Evaluate("count(/foo/bar)").ToString());
            Assert.Equal("3", nav2.Evaluate("count(//bar)").ToString());
        }

        [Fact]
        public void CoreFunctionLocalName()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/><squonk/></foo>");
            XPathNavigator nav2 = Utils.CreateNavigator("<foo><bar/><baz/><qux/></foo>");

            Assert.Equal("", nav.Evaluate("local-name()").ToString());
            Assert.Equal("", nav.Evaluate("local-name(/bogus)").ToString());
            Assert.Equal("foo", nav.Evaluate("local-name(/foo)").ToString());
            Assert.Equal("bar", nav2.Evaluate("local-name(/foo/*)").ToString());
        }

        [Fact]
        public void CoreFunctionConcat()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/><squonk/></foo>");

            Assert.Throws<XPathException>(() => nav.Evaluate("concat()"));
            Assert.Throws<XPathException>(() => nav.Evaluate("concat('foo')"));

            Assert.Equal("foobar", nav.Evaluate("concat('foo', 'bar')").ToString());
            Assert.Equal("foobarbaz", nav.Evaluate("concat('foo', 'bar', 'baz')").ToString());
            Assert.Equal("foobarbazqux", nav.Evaluate("concat('foo', 'bar', 'baz', 'qux')").ToString());
            Assert.Equal("foobarbazquxquux", nav.Evaluate("concat('foo', 'bar', 'baz', 'qux', 'quux')").ToString());
        }

        [Fact]
        public void CoreFunctionStartsWith()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/><squonk/></foo>");

            Assert.Throws<XPathException>(() => nav.Evaluate("starts-with()"));
            Assert.Throws<XPathException>(() => nav.Evaluate("starts-with('foo')"));
            Assert.Throws<XPathException>(() => nav.Evaluate("starts-with('foo', 'bar', 'baz')"));

            Assert.True((bool)nav.Evaluate("starts-with('foobar', 'foo')"));
            Assert.True(!(bool)nav.Evaluate("starts-with('foobar', 'bar')"));
        }

        [Fact]
        public void CoreFunctionContains()
        {
            XPathNavigator nav = Utils.CreateNavigator("<foo><bar/><baz/><qux/><squonk/></foo>");

            Assert.Throws<XPathException>(() => nav.Evaluate("contains()"));
            Assert.Throws<XPathException>(() => nav.Evaluate("contains('foo')"));
            Assert.Throws<XPathException>(() => nav.Evaluate("contains('foobar', 'oob', 'baz')"));

            Assert.True((bool)nav.Evaluate("contains('foobar', 'oob')"));
            Assert.True(!(bool)nav.Evaluate("contains('foobar', 'baz')"));
        }
    }
}
