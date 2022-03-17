using System.Drawing;

namespace CyberHacker.ExtensionClasses
{
    internal static class ConvertedExtensions
    {
        internal static void Deconstruct<T1, T2>(this object[] args, out T1 item1, out Size item2,
                                                 out Graphics item3, out T2 item4)
        {
            new BitmapProcClasses.ConvertedArgs<T1, T2>(args).Deconstruct(out item1, out item2, out item3, out item4);
        }
    }
}
