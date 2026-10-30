using System;
using NUnit.Framework;
using Laba1;

namespace Laba1.Tests
{
    [TestFixture]
    public class Vector3DTests
    {
        // ========== Конструкторы ==========

        [Test]
        public void DefaultConstructor_CreatesZeroVector()
        {
            var v = new Vector3D();
            Assert.That(v.Length(), Is.EqualTo(0));
            Assert.That(v.GetStartPoint(), Is.EqualTo(new[] { 0.0, 0.0, 0.0 }));
            Assert.That(v.GetEndPoint(), Is.EqualTo(new[] { 0.0, 0.0, 0.0 }));
        }

        [Test]
        public void FullConstructor_StoresCoordinates()
        {
            var v = new Vector3D(1, 2, 3, 4, 5, 6);
            Assert.That(v.GetStartPoint(), Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
            Assert.That(v.GetEndPoint(), Is.EqualTo(new[] { 4.0, 5.0, 6.0 }));
        }

        [Test]
        public void CopyConstructor_CopiesAllCoordinates()
        {
            var original = new Vector3D(1, 2, 3, 4, 5, 6);
            var copy = new Vector3D(original);

            Assert.That(copy.GetStartPoint(), Is.EqualTo(original.GetStartPoint()));
            Assert.That(copy.GetEndPoint(), Is.EqualTo(original.GetEndPoint()));
            Assert.That(copy == original, Is.True);
        }

        [Test]
        public void CopyConstructor_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Vector3D((Vector3D)null!));
        }

        // ========== Length ==========

        [Test]
        public void Length_ZeroVector_ReturnsZero()
        {
            Assert.That(new Vector3D(0, 0, 0, 0, 0, 0).Length(), Is.EqualTo(0));
        }

        [Test]
        public void Length_UnitVector_ReturnsOne()
        {
            Assert.That(new Vector3D(0, 0, 0, 1, 0, 0).Length(), Is.EqualTo(1).Within(1e-10));
            Assert.That(new Vector3D(0, 0, 0, 0, 1, 0).Length(), Is.EqualTo(1).Within(1e-10));
            Assert.That(new Vector3D(0, 0, 0, 0, 0, 1).Length(), Is.EqualTo(1).Within(1e-10));
        }

        [Test]
        public void Length_3_4_0_Returns5()
        {
            var v = new Vector3D(0, 0, 0, 3, 4, 0);
            Assert.That(v.Length(), Is.EqualTo(5).Within(1e-10));
        }

        [Test]
        public void Length_2_3_6_Returns7()
        {
            var v = new Vector3D(0, 0, 0, 2, 3, 6);
            Assert.That(v.Length(), Is.EqualTo(7).Within(1e-10));
        }

        // ========== Сложение ==========

        [Test]
        public void Add_TwoVectors_ReturnsSum()
        {
            var a = new Vector3D(0, 0, 0, 1, 2, 3);
            var b = new Vector3D(0, 0, 0, 4, 5, 6);

            var result = a + b;

            Assert.That(result.GetEndPoint(), Is.EqualTo(new[] { 5.0, 7.0, 9.0 }));
        }

        [Test]
        public void Add_NullLeft_ThrowsArgumentNullException()
        {
            Vector3D a = null!;
            var b = new Vector3D();
            Assert.Throws<ArgumentNullException>(() => { var _ = a + b; });
        }

        [Test]
        public void Add_NullRight_ThrowsArgumentNullException()
        {
            var a = new Vector3D();
            Vector3D b = null!;
            Assert.Throws<ArgumentNullException>(() => { var _ = a + b; });
        }

        // ========== Вычитание ==========

        [Test]
        public void Subtract_TwoVectors_ReturnsDifference()
        {
            var a = new Vector3D(0, 0, 0, 5, 7, 9);
            var b = new Vector3D(0, 0, 0, 1, 2, 3);

            var result = a - b;

            Assert.That(result.GetEndPoint(), Is.EqualTo(new[] { 4.0, 5.0, 6.0 }));
        }

        [Test]
        public void Subtract_Null_ThrowsArgumentNullException()
        {
            Vector3D a = null!;
            var b = new Vector3D();
            Assert.Throws<ArgumentNullException>(() => { var _ = a - b; });
        }

        // ========== Векторное произведение ==========

        [Test]
        public void CrossProduct_StandardBasis_ReturnsExpected()
        {
            // i × j = k
            var i = new Vector3D(0, 0, 0, 1, 0, 0);
            var j = new Vector3D(0, 0, 0, 0, 1, 0);

            var result = i * j;

            Assert.That(result.GetEndPoint(), Is.EqualTo(new[] { 0.0, 0.0, 1.0 }));
        }

        [Test]
        public void CrossProduct_ParallelVectors_ReturnsZero()
        {
            var a = new Vector3D(0, 0, 0, 2, 0, 0);
            var b = new Vector3D(0, 0, 0, 4, 0, 0);

            var result = a * b;

            Assert.That(result.Length(), Is.EqualTo(0).Within(1e-10));
        }

        [Test]
        public void CrossProduct_Null_ThrowsArgumentNullException()
        {
            Vector3D a = null!;
            var b = new Vector3D();
            Assert.Throws<ArgumentNullException>(() => { var _ = a * b; });
        }

        // ========== Умножение на скаляр ==========

        [Test]
        public void Multiply_ScalarOnRight_ScalesVector()
        {
            var v = new Vector3D(0, 0, 0, 1, 2, 3);
            var result = v * 2;
            Assert.That(result.GetEndPoint(), Is.EqualTo(new[] { 2.0, 4.0, 6.0 }));
        }

        [Test]
        public void Multiply_ScalarOnLeft_ScalesVector()
        {
            var v = new Vector3D(0, 0, 0, 1, 2, 3);
            var result = 2 * v;
            Assert.That(result.GetEndPoint(), Is.EqualTo(new[] { 2.0, 4.0, 6.0 }));
        }

        [Test]
        public void Multiply_ByZero_ReturnsZeroVector()
        {
            var v = new Vector3D(0, 0, 0, 1, 2, 3);
            Assert.That((v * 0).Length(), Is.EqualTo(0));
        }

        [Test]
        public void Multiply_ByNegative_FlipsDirection()
        {
            var v = new Vector3D(0, 0, 0, 1, 2, 3);
            var result = v * -1;
            Assert.That(result.GetEndPoint(), Is.EqualTo(new[] { -1.0, -2.0, -3.0 }));
        }

        [Test]
        public void Multiply_Null_ThrowsArgumentNullException()
        {
            Vector3D v = null!;
            Assert.Throws<ArgumentNullException>(() => { var _ = v * 2; });
        }

        // ========== Деление ==========

        [Test]
        public void Divide_ByScalar_ScalesVector()
        {
            var v = new Vector3D(0, 0, 0, 2, 4, 6);
            var result = v / 2;
            Assert.That(result.GetEndPoint(), Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
        }

        [Test]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            var v = new Vector3D(0, 0, 0, 1, 2, 3);
            Assert.Throws<DivideByZeroException>(() => { var _ = v / 0; });
        }

        [Test]
        public void Divide_Null_ThrowsArgumentNullException()
        {
            Vector3D v = null!;
            Assert.Throws<ArgumentNullException>(() => { var _ = v / 2; });
        }

        // ========== Cosine ==========

        [Test]
        public void Cosine_SameDirection_ReturnsOne()
        {
            var a = new Vector3D(0, 0, 0, 1, 0, 0);
            var b = new Vector3D(0, 0, 0, 5, 0, 0);
            Assert.That(Vector3D.Cosine(a, b), Is.EqualTo(1).Within(1e-10));
        }

        [Test]
        public void Cosine_Perpendicular_ReturnsZero()
        {
            var a = new Vector3D(0, 0, 0, 1, 0, 0);
            var b = new Vector3D(0, 0, 0, 0, 1, 0);
            Assert.That(Vector3D.Cosine(a, b), Is.EqualTo(0).Within(1e-10));
        }

        [Test]
        public void Cosine_OppositeDirection_ReturnsMinusOne()
        {
            var a = new Vector3D(0, 0, 0, 1, 0, 0);
            var b = new Vector3D(0, 0, 0, -1, 0, 0);
            Assert.That(Vector3D.Cosine(a, b), Is.EqualTo(-1).Within(1e-10));
        }

        [Test]
        public void Cosine_ZeroVector_ThrowsInvalidOperationException()
        {
            var a = new Vector3D(0, 0, 0, 0, 0, 0);
            var b = new Vector3D(0, 0, 0, 1, 0, 0);
            Assert.Throws<InvalidOperationException>(() => Vector3D.Cosine(a, b));
        }

        [Test]
        public void Cosine_Null_ThrowsArgumentNullException()
        {
            Vector3D a = null!;
            var b = new Vector3D();
            Assert.Throws<ArgumentNullException>(() => Vector3D.Cosine(a, b));
        }

        // ========== Операторы сравнения ==========

        [Test]
        public void Equality_SameCoordinates_ReturnsTrue()
        {
            var a = new Vector3D(1, 2, 3, 4, 5, 6);
            var b = new Vector3D(1, 2, 3, 4, 5, 6);
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
        }

        [Test]
        public void Equality_DifferentCoordinates_ReturnsFalse()
        {
            var a = new Vector3D(1, 2, 3, 4, 5, 6);
            var b = new Vector3D(1, 2, 3, 4, 5, 7);
            Assert.That(a == b, Is.False);
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void Equality_WithEpsilonTolerance_ReturnsTrue()
        {
            var a = new Vector3D(0, 0, 0, 1, 0, 0);
            var b = new Vector3D(0, 0, 0, 1 + 1e-11, 0, 0);
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void Equality_BothNull_ReturnsTrue()
        {
            Vector3D a = null!;
            Vector3D b = null!;
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void Equality_OneNull_ReturnsFalse()
        {
            var a = new Vector3D();
            Vector3D b = null!;
            Assert.That(a == b, Is.False);
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void Equality_SameReference_ReturnsTrue()
        {
            var a = new Vector3D(1, 1, 1, 2, 2, 2);
            var b = a;
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void Greater_LongerVector_ReturnsTrue()
        {
            var a = new Vector3D(0, 0, 0, 3, 4, 0);   // длина 5
            var b = new Vector3D(0, 0, 0, 1, 0, 0);   // длина 1
            Assert.That(a > b, Is.True);
            Assert.That(a >= b, Is.True);
            Assert.That(b < a, Is.True);
            Assert.That(b <= a, Is.True);
        }

        [Test]
        public void Greater_EqualLengths_ReturnsFalseForStrict()
        {
            var a = new Vector3D(0, 0, 0, 3, 4, 0);
            var b = new Vector3D(0, 0, 0, 0, 5, 0);
            Assert.That(a > b, Is.False);
            Assert.That(a >= b, Is.True);
        }

        [Test]
        public void Compare_Null_ThrowsArgumentNullException()
        {
            Vector3D a = null!;
            var b = new Vector3D();
            Assert.Throws<ArgumentNullException>(() => { var _ = a > b; });
            Assert.Throws<ArgumentNullException>(() => { var _ = a >= b; });
            Assert.Throws<ArgumentNullException>(() => { var _ = a < b; });
            Assert.Throws<ArgumentNullException>(() => { var _ = a <= b; });
        }

        // ========== ToString ==========

        [Test]
        public void ToString_UsesInvariantCulture()
        {
            var v = new Vector3D(1.5, 2.5, 3.5, 4.5, 5.5, 6.5);
            string s = v.ToString();
            Assert.That(s, Does.Contain("1.5"));
            Assert.That(s, Does.Not.Contain("1,5"));
        }

        // ========== Parse ==========

        [Test]
        public void Parse_ValidString_ReturnsVector()
        {
            var v = Vector3D.Parse("1 2 3 4 5 6");
            Assert.That(v.GetStartPoint(), Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
            Assert.That(v.GetEndPoint(), Is.EqualTo(new[] { 4.0, 5.0, 6.0 }));
        }

        [Test]
        public void Parse_WithSemicolons_Works()
        {
            var v = Vector3D.Parse("1;2;3;4;5;6");
            Assert.That(v.GetEndPoint(), Is.EqualTo(new[] { 4.0, 5.0, 6.0 }));
        }

        [Test]
        public void Parse_WithCommas_Works()
        {
            var v = Vector3D.Parse("1,2,3,4,5,6");
            Assert.That(v.GetEndPoint(), Is.EqualTo(new[] { 4.0, 5.0, 6.0 }));
        }

        [Test]
        public void Parse_WithFractional_UsesInvariantCulture()
        {
            var v = Vector3D.Parse("1.5 2.5 3.5 4.5 5.5 6.5");
            Assert.That(v.GetStartPoint()[0], Is.EqualTo(1.5));
        }

        [Test]
        public void Parse_EmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Vector3D.Parse(""));
            Assert.Throws<ArgumentException>(() => Vector3D.Parse("   "));
            Assert.Throws<ArgumentException>(() => Vector3D.Parse(null!));
        }

        [Test]
        public void Parse_WrongCount_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => Vector3D.Parse("1 2 3"));
            Assert.Throws<FormatException>(() => Vector3D.Parse("1 2 3 4 5 6 7"));
        }

        [Test]
        public void Parse_InvalidNumber_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => Vector3D.Parse("1 2 3 4 5 abc"));
        }
    }
}
