using System.Drawing;

namespace CyberHacker.BitmapProcClasses
{
    internal sealed class ConvertedArgs<T1, T2>
    {
        internal readonly T1 Info;
        internal readonly Size Size;
        internal readonly Graphics Graphics;
        internal readonly T2 Arg;

        internal ConvertedArgs(T1 Info, Size Size, Graphics Graphics, T2 Arg)
        {
            this.Info = Info;
            this.Size = Size;
            this.Graphics = Graphics;
            this.Arg = Arg;
        }

        internal ConvertedArgs(object[] args) : this((T1)args[0] ?? default, (Size)(args[1] ?? new Size(0, 0)), (Graphics)args[2], (T2)args[3] ?? default)
        {

        }

        internal void Deconstruct(out T1 item1, out Size item2, out Graphics item3, out T2 item4)
        {
            item1 = Info;
            item2 = Size;
            item3 = Graphics;
            item4 = Arg;
        }

        public static implicit operator (T1, Size, Graphics, T2)(ConvertedArgs<T1, T2> value)
        {
            return (value.Info, value.Size, value.Graphics, value.Arg);
        }

        public static implicit operator ConvertedArgs<T1, T2>((T1, Size, Graphics, T2) value)
        {
            return new ConvertedArgs<T1, T2>(value.Item1, value.Item2, value.Item3, value.Item4);
        }
    }
}
