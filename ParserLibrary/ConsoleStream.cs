using System.Text;

namespace ParserLibrary
{
    /// <summary>
    /// работает с потоками ввода/вывода
    /// </summary>
    public static class ConsoleStream
    {
        /// <summary>
        /// устанавливает входной поток в файл
        /// </summary>
        /// <param name="path">файл</param>
        public static void SetInputStream(string? path)
        {
            if (path != null)
            {
                StreamReader @in = new(path, UnicodeEncoding.UTF8);
                Console.SetIn(@in);
            }
        }

        /// <summary>
        /// устанавливает выходной поток в файл
        /// </summary>
        /// <param name="path">файл</param>
        public static void SetOutputStream(string? path)
        {
            if (path != null)
            {
                StreamWriter @out = new(path, false, UnicodeEncoding.UTF8);
                Console.SetOut(@out);
            }
        }

        /// <summary>
        /// ставит дефолтный входной поток
        /// </summary>
        public static void SetDefaultInputStream()
        {
            Console.In.Close();
            Console.SetIn(new StreamReader(Console.OpenStandardInput(), Console.InputEncoding));
        }

        /// <summary>
        /// ставит дефолтный выходной поток
        /// </summary>
        public static void SetDefaultOutputStream()
        {
            Console.Out.Close();
            StreamWriter standardOut = new(Console.OpenStandardOutput(), Console.OutputEncoding)
            {
                AutoFlush = true
            };
            Console.SetOut(standardOut);
        }

    }
}
