using GameLibrary;
namespace ParserLibrary
{
    /// <summary>
    /// работате с записью файла
    /// </summary>
    public static class Writer
    {
        /// <summary>
        /// записывает данные в файл
        /// </summary>
        /// <param name="data">каталог игр</param>
        public static void WriteFile(List<Game> data)
        {
            foreach (Game game in data)
            {
                Console.WriteLine(GameToString(game));
            }
        }

        /// <summary>
        /// конвертирует игру в строку для записи
        /// </summary>
        /// <param name="game">конвертируемая игра</param>
        /// <returns>ее строковое представление</returns>
        private static string GameToString(Game game)
        {
            List<string> result = [];
            string[] names = Game.AllFields;
            foreach (string name in names)
            {
                string value = game.GetField(name);
                if (value != null && value.Length > 0)
                {
                    result.Add($"[{value}]");

                }

            }
            return string.Join(' ', result.ToArray());
        }
    }
}
