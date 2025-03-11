using NUnit.Framework;
using System;
using carenirvana.bre.engine.rulefunction;

namespace carenirvana.bre.tests
{
    [TestFixture]
    public class RuleFunctionsTests
    {
        [Test]
        public void CONCAT_ShouldConcatenateStrings()
        {
            var result = RuleFunctionInternal.CONCAT("Hello", "World", "from", "NUnit");
            Assert.That(result, Is.EqualTo("Hello World from NUnit"));
        }

        [Test]
        public void LOWERCASE_ShouldConvertToLowerCase()
        {
            var result = RuleFunctionInternal.LOWERCASE("HELLO");
            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public void UPPERCASE_ShouldConvertToUpperCase()
        {
            var result = RuleFunctionInternal.UPPERCASE("hello");
            Assert.That(result, Is.EqualTo("HELLO"));
        }

        [Test]
        public void TRIM_ShouldTrimString()
        {
            var result = RuleFunctionInternal.TRIM("  hello  ");
            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public void TRIMSTART_ShouldTrimStartOfString()
        {
            var result = RuleFunctionInternal.TRIMSTART("  hello  ");
            Assert.That(result, Is.EqualTo("hello  "));
        }

        [Test]
        public void TRIMEND_ShouldTrimEndOfString()
        {
            var result = RuleFunctionInternal.TRIMEND("  hello  ");
            Assert.That(result, Is.EqualTo("  hello"));
        }

        [Test]
        public void DATEDIFF_ShouldReturnDifferenceInDays()
        {
            var date1 = new DateTime(2025, 2, 28);
            var date2 = new DateTime(2025, 2, 25);
            var result = RuleFunctionInternal.DATEDIFF(date1, date2, "D");
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void DATEDIFF_ShouldReturnDifferenceInMonths()
        {
            var date1 = new DateTime(2025, 2, 28);
            var date2 = new DateTime(2024, 2, 28);
            var result = RuleFunctionInternal.DATEDIFF(date1, date2, "M");
            Assert.That(result, Is.EqualTo(12));
        }

        [Test]
        public void DATEDIFF_ShouldReturnDifferenceInYears()
        {
            var date1 = new DateTime(2025, 2, 28);
            var date2 = new DateTime(2020, 2, 28);
            var result = RuleFunctionInternal.DATEDIFF(date1, date2, "Y");
            Assert.That(result, Is.EqualTo(5));
        }
    }
}