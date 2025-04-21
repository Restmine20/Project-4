using GameLibrary;
using HTTPLibrary;
using ConsoleInteraction;
using System.Text.RegularExpressions;
namespace ParserLibrary
{
    /// <summary>
    /// работает с чтением файла
    /// </summary>
    public static class Reader
    {
        /// <summary>
        /// считывает данные из файла в формате, указанном в примере варианта
        /// </summary>
        /// <param name="client">клиент для http request</param>
        /// <returns></returns>
        public static List<Game> ReadFile(Client client)
        {
            List<Game> data = [];

            string? line = Console.ReadLine();

            Regex pattern = new(@"\[.+?\]");
            while (line != null)
            {
                MatchCollection rawFields = pattern.Matches(line);
                List<string> rawGame = [];

                foreach (Match match in rawFields)
                {
                    rawGame.Add(match.Value.Trim(['[', ']']));
                }

                data.Add(CompareWithFoundedGame(client, rawGame));
                ConsoleOutput.PrintMessage($"Прогресс: {data.Count} игр\r", ConsoleColor.DarkBlue, false);
                line = Console.ReadLine();
            }
            return data;
        }

        /// <summary>
        /// ищет игру с таким же названием, ее полями дополняет считанную игру
        /// </summary>
        /// <param name="client">клиент для http request</param>
        /// <param name="rawGame">лист с полями игры</param>
        /// <returns>готовая игра</returns>
        private static Game CompareWithFoundedGame(Client client, List<string> rawGame)
        {
            Game foundedGame = client.FindGame("name", rawGame[0]) ?? new Game();

            for (int i = 0; i < Game.AllFields.Length; i++)
            {
                if (i < rawGame.Count)
                {
                    rawGame[i] = (rawGame[i] == "") ? foundedGame.GetField(Game.AllFields[i]) : rawGame[i];
                }
                else
                {
                    rawGame.Add(foundedGame.GetField(Game.AllFields[i]));
                }
            }
            Game newGame = new Game(rawGame);

            if (!File.Exists(newGame.GetField("Cover")))
            {
                newGame.SetField("Cover", foundedGame.GetField("Cover"));
            }
            return newGame;
        }

    }
}
