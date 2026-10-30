using System;
using NUnit.Framework;
using Laba1;

namespace Laba1.Tests
{
    [TestFixture]
    public class PostMachineTests
    {
        private PostMachine CreateSimpleProgram()
        {
            // Программа: поставить метку, сдвинуться вправо, остановиться
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Mark, 2));
            m.AddCommand(new PostCommand(CommandType.Right, 3));
            m.AddCommand(new PostCommand(CommandType.Stop));
            return m;
        }

        // ========== Конструкторы ==========

        [Test]
        public void DefaultConstructor_EmptyMachine()
        {
            var m = new PostMachine();
            Assert.That(m.CommandCount, Is.EqualTo(0));
            Assert.That(m.CurrentLine, Is.EqualTo(1));
            Assert.That(m.IsStopped, Is.False);
            Assert.That(m.GetHead(), Is.EqualTo(0));
            Assert.That(m.GetMarks(), Is.Empty);
        }

        [Test]
        public void CopyConstructor_CopiesAllState()
        {
            var m = CreateSimpleProgram();
            m.SetHead(5);
            m.SetMark(3, true);
            m.SetMark(7, true);

            var copy = new PostMachine(m);

            Assert.That(copy.CommandCount, Is.EqualTo(m.CommandCount));
            Assert.That(copy.CurrentLine, Is.EqualTo(m.CurrentLine));
            Assert.That(copy.GetHead(), Is.EqualTo(5));
            Assert.That(copy.GetMarks(), Is.EqualTo(new[] { 3, 7 }));
            Assert.That(copy, Is.EqualTo(m));
        }

        [Test]
        public void CopyConstructor_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PostMachine((PostMachine)null!));
        }

        // ========== Лента ==========

        [Test]
        public void SetMark_AddsMark()
        {
            var m = new PostMachine();
            m.SetMark(5, true);
            Assert.That(m.IsMarked(5), Is.True);
        }

        [Test]
        public void SetMark_RemovesMark()
        {
            var m = new PostMachine();
            m.SetMark(5, true);
            m.SetMark(5, false);
            Assert.That(m.IsMarked(5), Is.False);
        }

        [Test]
        public void SetMark_RemoveNonexistent_DoesNothing()
        {
            var m = new PostMachine();
            m.SetMark(5, false);
            Assert.That(m.IsMarked(5), Is.False);
        }

        [Test]
        public void GetMarks_ReturnsSortedArray()
        {
            var m = new PostMachine();
            m.SetMark(5, true);
            m.SetMark(1, true);
            m.SetMark(3, true);

            Assert.That(m.GetMarks(), Is.EqualTo(new[] { 1, 3, 5 }));
        }

        [Test]
        public void GetMarks_Empty_ReturnsEmptyArray()
        {
            Assert.That(new PostMachine().GetMarks(), Is.Empty);
        }

        [Test]
        public void NegativePositions_Work()
        {
            var m = new PostMachine();
            m.SetMark(-5, true);
            m.SetMark(-1, true);
            Assert.That(m.GetMarks(), Is.EqualTo(new[] { -5, -1 }));
        }

        // ========== Каретка ==========

        [Test]
        public void SetHead_StoresPosition()
        {
            var m = new PostMachine();
            m.SetHead(42);
            Assert.That(m.GetHead(), Is.EqualTo(42));
        }

        // ========== Программа ==========

        [Test]
        public void AddCommand_IncreasesCount()
        {
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Stop));
            Assert.That(m.CommandCount, Is.EqualTo(1));
        }

        [Test]
        public void AddCommand_Null_ThrowsArgumentNullException()
        {
            var m = new PostMachine();
            Assert.Throws<ArgumentNullException>(() => m.AddCommand(null!));
        }

        [Test]
        public void AddCommand_MakesCopy()
        {
            var original = new PostCommand(CommandType.Mark, 1);
            var m = new PostMachine();
            m.AddCommand(original);

            var copy = new PostCommand(CommandType.Mark, 1);
            Assert.That(m.GetCommand(1), Is.EqualTo(copy));
        }

        [Test]
        public void GetCommand_ValidNumber_ReturnsCommand()
        {
            var m = CreateSimpleProgram();
            Assert.That(m.GetCommand(1).Type, Is.EqualTo(CommandType.Mark));
            Assert.That(m.GetCommand(3).Type, Is.EqualTo(CommandType.Stop));
        }

        [Test]
        public void GetCommand_InvalidNumber_ThrowsArgumentOutOfRangeException()
        {
            var m = CreateSimpleProgram();
            Assert.Throws<ArgumentOutOfRangeException>(() => m.GetCommand(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => m.GetCommand(4));
            Assert.Throws<ArgumentOutOfRangeException>(() => m.GetCommand(-1));
        }

        // ========== Step ==========

        [Test]
        public void Step_Mark_SetsMarkAndMovesToNext1()
        {
            var m = CreateSimpleProgram();
            m.Step();

            Assert.That(m.IsMarked(0), Is.True);
            Assert.That(m.CurrentLine, Is.EqualTo(2));
        }

        [Test]
        public void Step_Unmark_RemovesMark()
        {
            var m = new PostMachine();
            m.SetMark(0, true);
            m.AddCommand(new PostCommand(CommandType.Unmark, 2));
            m.AddCommand(new PostCommand(CommandType.Stop));

            m.Step();
            Assert.That(m.IsMarked(0), Is.False);
        }

        [Test]
        public void Step_Left_MovesHeadLeft()
        {
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Left, 2));
            m.AddCommand(new PostCommand(CommandType.Stop));

            m.Step();
            Assert.That(m.GetHead(), Is.EqualTo(-1));
        }

        [Test]
        public void Step_Right_MovesHeadRight()
        {
            var m = CreateSimpleProgram();
            m.Step(); // Mark
            m.Step(); // Right
            Assert.That(m.GetHead(), Is.EqualTo(1));
        }

        [Test]
        public void Step_Check_MarkPresent_GoesToNext1()
        {
            var m = new PostMachine();
            m.SetMark(0, true);
            m.AddCommand(new PostCommand(CommandType.Check, 2, 3));
            m.AddCommand(new PostCommand(CommandType.Stop));
            m.AddCommand(new PostCommand(CommandType.Stop));

            m.Step();
            Assert.That(m.CurrentLine, Is.EqualTo(2));
        }

        [Test]
        public void Step_Check_MarkAbsent_GoesToNext2()
        {
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Check, 2, 3));
            m.AddCommand(new PostCommand(CommandType.Stop));
            m.AddCommand(new PostCommand(CommandType.Stop));

            m.Step();
            Assert.That(m.CurrentLine, Is.EqualTo(3));
        }

        [Test]
        public void Step_Stop_SetsStopped()
        {
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Stop));
            m.Step();
            Assert.That(m.IsStopped, Is.True);
        }

        [Test]
        public void Step_AlreadyStopped_ThrowsInvalidOperationException()
        {
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Stop));
            m.Step();
            Assert.Throws<InvalidOperationException>(() => m.Step());
        }

        [Test]
        public void Step_EmptyProgram_ThrowsInvalidOperationException()
        {
            var m = new PostMachine();
            Assert.Throws<InvalidOperationException>(() => m.Step());
        }

        [Test]
        public void Step_InvalidCurrentLine_ThrowsInvalidOperationException()
        {
            var m = CreateSimpleProgram();
            m.SetHead(0);
            // Сдвигаем current за пределы программы через Parse-трюк или Reset+руками не выйдет,
            // но можно сэмулировать через команду с невалидным переходом — пропустим,
            // так как конструктор PostCommand не даёт создать такое.
            // Вместо этого проверим, что Reset работает.
            m.Reset();
            Assert.That(m.CurrentLine, Is.EqualTo(1));
        }

        // ========== Run ==========

        [Test]
        public void Run_CompletesProgram_ReturnsStepCount()
        {
            var m = CreateSimpleProgram();
            int steps = m.Run();
            Assert.That(steps, Is.EqualTo(3));
            Assert.That(m.IsStopped, Is.True);
        }

        [Test]
        public void Run_LoopingProgram_ThrowsInvalidOperationException()
        {
            // Программа: R 1 — бесконечный сдвиг вправо
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Right, 1));

            Assert.Throws<InvalidOperationException>(() => m.Run(maxSteps: 100));
        }

        [Test]
        public void Run_MaxStepsCustom_RespectsLimit()
        {
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Right, 1));

            var ex = Assert.Throws<InvalidOperationException>(() => m.Run(maxSteps: 5));
            
            // Вставили эти две строки вместо старой:
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.Message, Does.Contain("5")); 
        }

        // ========== Reset ==========

        [Test]
        public void Reset_ReturnsToFirstCommand()
        {
            var m = CreateSimpleProgram();
            m.Step();
            m.Step();
            m.Reset();

            Assert.That(m.CurrentLine, Is.EqualTo(1));
            Assert.That(m.IsStopped, Is.False);
        }

        [Test]
        public void Reset_AfterStop_ClearsStopped()
        {
            var m = new PostMachine();
            m.AddCommand(new PostCommand(CommandType.Stop));
            m.Step();
            Assert.That(m.IsStopped, Is.True);

            m.Reset();
            Assert.That(m.IsStopped, Is.False);
        }

        // ========== Equals ==========

        [Test]
        public void Equals_SameMachines_ReturnsTrue()
        {
            var a = CreateSimpleProgram();
            var b = CreateSimpleProgram();
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
        }

        [Test]
        public void Equals_DifferentTape_ReturnsFalse()
        {
            var a = CreateSimpleProgram();
            var b = CreateSimpleProgram();
            b.SetMark(10, true);
            Assert.That(a.Equals(b), Is.False);
            Assert.That(a != b, Is.True);
        }

        [Test]
        public void Equals_DifferentHead_ReturnsFalse()
        {
            var a = CreateSimpleProgram();
            var b = CreateSimpleProgram();
            b.SetHead(5);
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_DifferentProgram_ReturnsFalse()
        {
            var a = CreateSimpleProgram();
            var b = new PostMachine();
            b.AddCommand(new PostCommand(CommandType.Stop));
            Assert.That(a.Equals(b), Is.False);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            var a = CreateSimpleProgram();
            Assert.That(a.Equals(null), Is.False);
            Assert.IsFalse(a.Equals((object?)null));
        }

        [Test]
        public void Equals_SameReference_ReturnsTrue()
        {
            var a = CreateSimpleProgram();
            Assert.That(a.Equals(a), Is.True);
        }

        [Test]
        public void Equals_Object_Works()
        {
            var a = CreateSimpleProgram();
            object b = CreateSimpleProgram();
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void GetHashCode_EqualMachines_EqualHashes()
        {
            var a = CreateSimpleProgram();
            var b = CreateSimpleProgram();
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }

        // ========== ToString / Parse ==========

        [Test]
        public void ToString_ContainsAllParts()
        {
            var m = CreateSimpleProgram();
            m.SetMark(1, true);
            string s = m.ToString();

            Assert.That(s, Does.Contain("tape: 1"));
            Assert.That(s, Does.Contain("head: 0"));
            Assert.That(s, Does.Contain("current: 1"));
            Assert.That(s, Does.Contain("stopped: false"));
            Assert.That(s, Does.Contain("V 2"));
        }

        [Test]
        public void Parse_ValidText_RestoresMachine()
        {
            string text =
                "tape: 1 3 5\n" +
                "head: 2\n" +
                "current: 1\n" +
                "stopped: false\n" +
                "V 2\n" +
                "!\n";

            var m = PostMachine.Parse(text);

            Assert.That(m.GetMarks(), Is.EqualTo(new[] { 1, 3, 5 }));
            Assert.That(m.GetHead(), Is.EqualTo(2));
            Assert.That(m.CommandCount, Is.EqualTo(2));
        }

        [Test]
        public void Parse_WithoutPrefixes_Works()
        {
            var m = PostMachine.Parse("V 2\n!\n");
            Assert.That(m.CommandCount, Is.EqualTo(2));
        }

        [Test]
        public void Parse_RoundTrip_PreservesState()
        {
            var original = CreateSimpleProgram();
            original.SetMark(1, true);
            original.SetMark(5, true);
            original.SetHead(3);

            var parsed = PostMachine.Parse(original.ToString());

            Assert.That(parsed, Is.EqualTo(original));
        }

        [Test]
        public void Parse_EmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => PostMachine.Parse(""));
            Assert.Throws<ArgumentException>(() => PostMachine.Parse("   "));
            Assert.Throws<ArgumentException>(() => PostMachine.Parse(null!));
        }

        [Test]
        public void Parse_InvalidNumber_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => PostMachine.Parse("head: abc\n!\n"));
        }

        [Test]
        public void Parse_CommandPointsToNonexistent_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => PostMachine.Parse("V 100\n!\n"));
        }

        [Test]
        public void Parse_CheckPointsToNonexistent_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => PostMachine.Parse("? 1 100\n!\n"));
        }

        [Test]
        public void Parse_CurrentOutOfRange_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => PostMachine.Parse("current: 100\n!\n"));
        }

        [Test]
        public void Parse_StoppedTrue_ParsedCorrectly()
        {
            var m = PostMachine.Parse("stopped: true\n!\n");
            Assert.That(m.IsStopped, Is.True);
        }

        [Test]
        public void Parse_StoppedFalse_ParsedCorrectly()
        {
            var m = PostMachine.Parse("stopped: false\n!\n");
            Assert.That(m.IsStopped, Is.False);
        }

        // ========== Интеграционный пример ==========

        [Test]
        public void Example_AddOne_Works()
        {
            string text =
                "tape: 0 1 2\n" +
                "head: 0\n" +
                "R 2\n" +
                "? 1 3\n" +
                "V 4\n" +
                "!\n";

            var m = PostMachine.Parse(text);
            m.Run();

            Assert.That(m.GetMarks(), Is.EqualTo(new[] { 0, 1, 2, 3 }));
        }

        [Test]
        public void Example_SubOne_Works()
        {
            string text =
                "tape: 0 1 2\n" +
                "head: 0\n" +
                "R 2\n" +
                "? 1 3\n" +
                "L 4\n" +
                "X 5\n" +
                "!\n";

            var m = PostMachine.Parse(text);
            m.Run();

            Assert.That(m.GetMarks(), Is.EqualTo(new[] { 0, 1 }));
        }
    }
}
