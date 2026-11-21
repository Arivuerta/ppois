using Laba1;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
using System;
using System.Data;

namespace Laba1.Tests
{
    [TestFixture]
    public class PostCommandTests
    {
        // ========== Конструктор ==========

        [Test]
        public void Constructor_Mark_StoresTypeAndNext1()
        {
            var c = new PostCommand(CommandType.Mark, 5);
            Assert.That(c.Type, Is.EqualTo(CommandType.Mark));
            Assert.That(c.Next1, Is.EqualTo(5));
            Assert.That(c.Next2, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_Unmark_StoresTypeAndNext1()
        {
            var c = new PostCommand(CommandType.Unmark, 3);
            Assert.That(c.Type, Is.EqualTo(CommandType.Unmark));
            Assert.That(c.Next1, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_Left_StoresTypeAndNext1()
        {
            var c = new PostCommand(CommandType.Left, 2);
            Assert.That(c.Type, Is.EqualTo(CommandType.Left));
            Assert.That(c.Next1, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_Right_StoresTypeAndNext1()
        {
            var c = new PostCommand(CommandType.Right, 4);
            Assert.That(c.Type, Is.EqualTo(CommandType.Right));
            Assert.That(c.Next1, Is.EqualTo(4));
        }

        [Test]
        public void Constructor_Check_StoresBothNexts()
        {
            var c = new PostCommand(CommandType.Check, 1, 2);
            Assert.That(c.Type, Is.EqualTo(CommandType.Check));
            Assert.That(c.Next1, Is.EqualTo(1));
            Assert.That(c.Next2, Is.EqualTo(2));
        }

        [Test]
        public void Constructor_Stop_IgnoresNexts()
        {
            var c = new PostCommand(CommandType.Stop, 5, 7);
            Assert.That(c.Type, Is.EqualTo(CommandType.Stop));
            Assert.That(c.Next1, Is.EqualTo(0));
            Assert.That(c.Next2, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_InvalidNext1_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new PostCommand(CommandType.Mark, 0));
            Assert.Throws<ArgumentException>(() => new PostCommand(CommandType.Mark, -1));
        }

        [Test]
        public void Constructor_CheckWithoutNext2_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new PostCommand(CommandType.Check, 1, 0));
        }

        // ========== Копирование ==========

        [Test]
        public void CopyConstructor_CopiesAllFields()
        {
            var original = new PostCommand(CommandType.Check, 1, 2);
            var copy = new PostCommand(original);

            Assert.That(copy.Type, Is.EqualTo(original.Type));
            Assert.That(copy.Next1, Is.EqualTo(original.Next1));
            Assert.That(copy.Next2, Is.EqualTo(original.Next2));
            Assert.That(copy, Is.EqualTo(original));
        }

        [Test]
        public void CopyConstructor_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PostCommand((PostCommand)null!));
        }

        // ========== Equals / GetHashCode ==========

        [Test]
        public void Equals_SameCommands_ReturnsTrue()
        {
            var a = new PostCommand(CommandType.Mark, 5);
            var b = new PostCommand(CommandType.Mark, 5);
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        [Test]
        public void Equals_DifferentCommands_ReturnsFalse()
        {
            var a = new PostCommand(CommandType.Mark, 5);
            var b = new PostCommand(CommandType.Mark, 6);
            Assert.That(a.Equals(b), Is.False);
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            var a = new PostCommand(CommandType.Mark, 5);
            Assert.That(a.Equals(null), Is.False);
        }

        [Test]
        public void Equals_WithObject_Works()
        {
            var a = new PostCommand(CommandType.Mark, 5);
            object b = new PostCommand(CommandType.Mark, 5);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new PostCommand(CommandType.Mark, 5);
            var b = new PostCommand(CommandType.Unmark, 5);
            Assert.That(a.Equals(b), Is.False);
        }

        // ========== ToString ==========

        [Test]
        public void ToString_Mark_ReturnsV()
            => Assert.That(new PostCommand(CommandType.Mark, 2).ToString(), Is.EqualTo("V 2"));

        [Test]
        public void ToString_Unmark_ReturnsX()
            => Assert.That(new PostCommand(CommandType.Unmark, 2).ToString(), Is.EqualTo("X 2"));

        [Test]
        public void ToString_Left_ReturnsL()
            => Assert.That(new PostCommand(CommandType.Left, 2).ToString(), Is.EqualTo("L 2"));

        [Test]
        public void ToString_Right_ReturnsR()
            => Assert.That(new PostCommand(CommandType.Right, 2).ToString(), Is.EqualTo("R 2"));

        [Test]
        public void ToString_Check_ReturnsQuestion()
            => Assert.That(new PostCommand(CommandType.Check, 1, 2).ToString(), Is.EqualTo("? 1 2"));

        [Test]
        public void ToString_Stop_ReturnsExclamation()
            => Assert.That(new PostCommand(CommandType.Stop).ToString(), Is.EqualTo("!"));

        // ========== Parse ==========

        [Test]
        public void Parse_Mark_ReturnsMarkCommand()
        {
            var c = PostCommand.Parse("V 5");
            Assert.That(c.Type, Is.EqualTo(CommandType.Mark));
            Assert.That(c.Next1, Is.EqualTo(5));
        }

        [Test]
        public void Parse_Unmark_ReturnsUnmarkCommand()
        {
            var c = PostCommand.Parse("X 3");
            Assert.That(c.Type, Is.EqualTo(CommandType.Unmark));
        }

        [Test]
        public void Parse_Left_ReturnsLeftCommand()
        {
            var c = PostCommand.Parse("L 2");
            Assert.That(c.Type, Is.EqualTo(CommandType.Left));
        }

        [Test]
        public void Parse_Right_ReturnsRightCommand()
        {
            var c = PostCommand.Parse("R 4");
            Assert.That(c.Type, Is.EqualTo(CommandType.Right));
        }

        [Test]
        public void Parse_Check_ReturnsCheckCommand()
        {
            var c = PostCommand.Parse("? 1 3");
            Assert.That(c.Type, Is.EqualTo(CommandType.Check));
            Assert.That(c.Next1, Is.EqualTo(1));
            Assert.That(c.Next2, Is.EqualTo(3));
        }

        [Test]
        public void Parse_Stop_ReturnsStopCommand()
        {
            var c = PostCommand.Parse("!");
            Assert.That(c.Type, Is.EqualTo(CommandType.Stop));
        }

        [Test]
        public void Parse_Lowercase_Works()
        {
            Assert.That(PostCommand.Parse("v 5").Type, Is.EqualTo(CommandType.Mark));
            Assert.That(PostCommand.Parse("r 5").Type, Is.EqualTo(CommandType.Right));
        }

        [Test]
        public void Parse_EmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PostCommand.Parse(""));
            Assert.Throws<ArgumentException>(() => PostCommand.Parse("   "));
            Assert.Throws<ArgumentException>(() => PostCommand.Parse(null!));
        }

        [Test]
        public void Parse_UnknownCommand_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => PostCommand.Parse("Z 5"));
        }

        [Test]
        public void Parse_WrongParameterCount_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => PostCommand.Parse("V"));
            Assert.Throws<FormatException>(() => PostCommand.Parse("V 1 2"));
            Assert.Throws<FormatException>(() => PostCommand.Parse("? 1"));
            Assert.Throws<FormatException>(() => PostCommand.Parse("! 1"));
        }

        [Test]
        public void Parse_InvalidNumber_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => PostCommand.Parse("V abc"));
            Assert.Throws<FormatException>(() => PostCommand.Parse("? 1 abc"));
        }
    }
}
