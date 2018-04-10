using System;
using System.IO;

namespace TTStockSource.Tests
{
    internal static class TestUtilities
    {
        public static string ReadTestFile(string filename)
        {
            string filePath = Path.Combine(Environment.CurrentDirectory, "TestFiles", filename);

            if (!File.Exists(filePath))
            {
                return null;
            }

            using (StreamReader sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
