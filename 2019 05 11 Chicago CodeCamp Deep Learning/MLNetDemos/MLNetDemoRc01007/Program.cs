using System;

namespace MLNetDemoRc01007
{
    class Program
    {
        static void Main(string[] args)
        {
            var r = GetLabel(4);
            var r2 = GetLabel2(4);
            Console.WriteLine("Hello World!");
        }

        private static string GetLabel(int age)
        {
            var label = "baby";
            if(age > 3)
                label = "kid";
            return label;
        }

        private static string GetLabel2(int age)
        {
            var label = "baby";
            if(age > 12)
                label = "teenager";
            else if(age > 3)
                label = "kid";
            return label;
        }
    }
}
