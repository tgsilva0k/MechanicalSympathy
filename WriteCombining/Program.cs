using System.Diagnostics;
using static System.Console;

namespace WriteCombining
{
    public static class Program
    {
        private const int Items = 1 << 24;
        private const int Iterations = int.MaxValue;
        private const int Mask = Items - 1;

        private static readonly byte[] ArrayA = new byte[Items];
        private static readonly byte[] ArrayB = new byte[Items];
        private static readonly byte[] ArrayC = new byte[Items];
        private static readonly byte[] ArrayD = new byte[Items];
        private static readonly byte[] ArrayE = new byte[Items];
        private static readonly byte[] ArrayF = new byte[Items];
    
        public static void Main()
        {
            for (var i = 1; i <= 3; i++)
            {
                WriteLine($"{i} SingleLoop duration (ns) = {RunCaseOne()}");
                WriteLine($"{i} SplitLoop  duration (ns) = {RunCaseTwo()}");
            }
            
            var result = ArrayA[1] + ArrayB[2] + ArrayC[3] + ArrayD[4] + ArrayE[5] + ArrayF[6];
            WriteLine($"Result = {result}");
        }

        private static long RunCaseOne()
        {
            var watch = Stopwatch.StartNew();

            var i = Iterations;
            while (--i != 0)
            {
                var slot = i & Mask;
                var b = (byte)i;
                ArrayA[slot] = b;
                ArrayB[slot] = b;
                ArrayC[slot] = b;
                ArrayD[slot] = b;
                ArrayE[slot] = b;
                ArrayF[slot] = b;
            }

            watch.Stop();
            
            return watch.ElapsedNanoSeconds();
        }

        // Taking advantage of Write Combining technique, using Line Fill Buffers (aka, Miss Address Buffer)
        // we're able to get a boost of performance. (Intel and AMD processors only)
        // 
        // Write combining?
        // Write combining is a computer bus technique for allowing data to be combined and temporarily stored in a
        // buffer, to be released together later in burst mode instead of writing (immediately) as single bits or small chunks
        //
        // Why?
        // "If we can fill these buffers before they are transferred to the outer caches then we will greatly
        // improve the effective use of the transfer bus at every level."
        //
        // There are a limited number of these buffers, and they differ by CPU model.
        // For example on an Intel CPU you are only guaranteed to get 4 of them at one time.
        // What this means is that within a loop you should not write to more than 4 distinct memory locations
        // at one time or you will not benefit from the write combining effect.
        //
        // Important:
        // With hyper-threading we can have 2 threads in competition for these buffers on the same core
        //
        // References:
        // https://download.intel.com/design/PentiumII/applnots/24442201.pdf
        // https://stackoverflow.com/questions/49959963/where-is-the-write-combining-buffer-located-x86/49961612
        // https://mechanical-sympathy.blogspot.com/2011/07/write-combining.html
        // https://community.intel.com/t5/Intel-Moderncode-for-Parallel/What-is-the-aim-of-the-line-fill-buffer/m-p/1180777
        private static long RunCaseTwo()
        {
            var watch = Stopwatch.StartNew();
            
            var i = Iterations;
            while (--i != 0)
            {
                var slot = i & Mask;
                var b = (byte)i;
                ArrayA[slot] = b;
                ArrayB[slot] = b;
                ArrayC[slot] = b;
            }
            
            i = Iterations;
            while (--i != 0)
            {
                var slot = i & Mask;
                var b = (byte)i;
                ArrayD[slot] = b;
                ArrayE[slot] = b;
                ArrayF[slot] = b;
            }
            
            watch.Stop();

            return watch.ElapsedNanoSeconds();
        }
    }
}