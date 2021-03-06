using FluentAssertions;
using System;
using Xunit;

namespace ZBRA.Maybe.Test
{
    public class MaybeExtensionsTests
    {
        [Fact]
        public void ToMaybe_WithMaybeSubject_ReturnsItself()
        {
            var subject = 1.ToMaybe();
            var result = subject.ToMaybe();
            result.Should().Be(subject);

            subject = Maybe<int>.Nothing;
            result = subject.ToMaybe();
            result.Should().Be(subject);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("value", "value")]
        public void OrEmpty_ReturnsValueOrEmpty(string subject, string expected)
        {
            var result = subject.ToMaybe()
                .OrEmpty();

            result.Should().Be(expected);
        }

        [Fact]
        public void OrTrue_ReturnsValueOrTrue()
        {
            var result = Maybe<bool>.Nothing
                .OrTrue();
            result.Should().BeTrue();

            result = true.ToMaybe()
                .OrTrue();
            result.Should().BeTrue();

            result = false.ToMaybe()
                .OrTrue();
            result.Should().BeFalse();
        }

        [Fact]
        public void OrFalse_ReturnsValueOrFalse()
        {
            var result = Maybe<bool>.Nothing
                .OrFalse();
            result.Should().BeFalse();

            result = false.ToMaybe()
               .OrFalse();
            result.Should().BeFalse();

            result = true.ToMaybe()
                .OrFalse();
            result.Should().BeTrue();
        }

        [Fact]
        public void ToMaybe_WithNullString_ShouldReturnMaybeNothing()
        {
            string subject = null;
            subject.ToMaybe().Should().Be(Maybe<string>.Nothing);
        }

        [Fact]
        public void ToMaybe_WithEmptyString_ShouldNotReturnMaybeNothing()
        {
            var subject = "";
            var maybe = subject.ToMaybe();
            maybe.Should().NotBe(Maybe<string>.Nothing);
            maybe.HasValue.Should().BeTrue();
            maybe.Value.Should().Be(subject);
        }

        [Theory]
        [MemberData(nameof(Select_WithNonNullablePropertyTestCases))]
        public void Select_WithNonNullableProperty_ShouldReturnSelectedProperty(Maybe<IntObj> subject, Maybe<int> expected)
        {
            var result = subject.Select(i => i.Count);

            result.Should().Be(expected);
        }

        public static TheoryData<Maybe<IntObj>, Maybe<int>> Select_WithNonNullablePropertyTestCases()
        {
            return new TheoryData<Maybe<IntObj>, Maybe<int>>
            {
                { Maybe<IntObj>.Nothing, Maybe<int>.Nothing },
                { new IntObj(1).ToMaybe(), 1.ToMaybe() },
            };
        }

        [Theory]
        [MemberData(nameof(Select_WithNullablePropertyTestCases))]
        public void Select_WithNullableProperty_ReturnsSelectedProperty(Maybe<NullableIntObj> subject, Maybe<int> expected)
        {
            var result = subject.Select(o => o.Count);

            result.Should().Be(expected);
        }

        public static TheoryData<Maybe<NullableIntObj>, Maybe<int>> Select_WithNullablePropertyTestCases()
        {
            return new TheoryData<Maybe<NullableIntObj>, Maybe<int>>
            {
                { Maybe<NullableIntObj>.Nothing, Maybe<int>.Nothing },
                { new NullableIntObj(null).ToMaybe(), Maybe<int>.Nothing },
                { new NullableIntObj(1).ToMaybe(), 1.ToMaybe() },
            };
        }

        [Fact]
        public void Select_NullArgument_ShouldThrow()
        {
            Action subject = () => 1.ToMaybe().Select<int, int>(null);

            subject.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Select_ConstrainedStructNullArgument_ShouldThrow()
        {
            Action subject = () => 1.ToMaybe().Select((Func<int, int?>)null);

            subject.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(SelectMany_WithMaybePropertyTestCases))]
        public void SelectMany_WithMaybeProperty_ReturnsSelectedProperty(Maybe<MaybeIntObj> subject, Maybe<int> expected)
        {
            var result = subject.SelectMany(o => o.Count);

            result.Should().Be(expected);
        }

        public static TheoryData<Maybe<MaybeIntObj>, Maybe<int>> SelectMany_WithMaybePropertyTestCases()
        {
            return new TheoryData<Maybe<MaybeIntObj>, Maybe<int>>
            {
                { Maybe<MaybeIntObj>.Nothing, Maybe<int>.Nothing },
                { new MaybeIntObj(null).ToMaybe(), Maybe<int>.Nothing },
                { new MaybeIntObj(1).ToMaybe(), 1.ToMaybe() },
            };
        }

        [Fact]
        public void SelectMany_NullArgument_ShouldThrow()
        {
            Action subject = () => 1.ToMaybe().SelectMany<int, int>(null);

            subject.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
