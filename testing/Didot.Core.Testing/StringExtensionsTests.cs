using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Core.Testing;
public class StringExtensionsTests
{
    [Test]
    [TestCase(".csv", ".csv")]
    [TestCase(" .csv ", ".csv")]
    [TestCase(".CSV", ".csv")]
    [TestCase("csv", ".csv")]
    [TestCase(" CSV ", ".csv")]
    public void NormalizeExtension_CorrectValue(string input, string expected)
        => Assert.That(input.NormalizeExtension(), Is.EqualTo(expected));

    [Test]
    [TestCase("true", true)]
    [TestCase("false", false)]
    [TestCase("True", true)]
    [TestCase("False", false)]
    [TestCase("1", true)]
    [TestCase("0", false)]
    public void ToBoolean_CorrectValue(string input, bool expected)
        => Assert.That(input.ToBoolean(), Is.EqualTo(expected));

    [Test]
    [TestCase("10", 10)]
    [TestCase("[]")]
    [TestCase("[1,2]", 1, 2)]
    [TestCase(" [ 1   , 2  ] ", 1, 2)]
    [TestCase("[1,,,,2] ", 1, 2)]
    public void ToArrayInt_CorrectValue(string input, params int[] expected)
        => Assert.That(input.ToArrayInt(), Is.EqualTo(expected));
}
