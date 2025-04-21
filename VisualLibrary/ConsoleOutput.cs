using Spectre.Console;
using GameLibrary;

namespace ConsoleInteraction
{
    /// <summary>
    /// класс содердит методы вывода различной информации в консоль
    /// </summary>
    public static class ConsoleOutput
    {
        /// <summary>
        /// выводит сообщение в консоль
        /// </summary>
        /// <param name="message">текст</param>
        /// <param name="color">цвет</param>
        /// <param name="endLine">переходить ли на новую строку</param>
        public static void PrintMessage(string message, ConsoleColor color = ConsoleColor.Gray, bool endLine = true)
        {
            Console.ForegroundColor = color;
            if (endLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// выводит таблицу в консоль согласно азаданию
        /// </summary>
        /// <param name="data"></param>
        public static void PrintTable(List<Game>data)
        {
            Table table = new Table();
            table.Title("[cyan]Каталог[/]");
            foreach (string name in Game.AllFields)
            {
                table.AddColumn(name);
            }
            for (int i = 0; i < data.Count; i++)
            {

                
                table.AddRow(data[i].GetAllValues());
            }
            table.Width(400);
            table.ShowRowSeparators = true;
            AnsiConsole.Write(table);
        }

        /// <summary>
        /// выводит иерархию в консоль, согласно заданию
        /// </summary>
        /// <param name="data"></param>
        public static void PrintTree(List<Game> data)
        {
            Tree root = new Tree("Каталог");

            string[] allGenres = Game.GetFieldsByName("Genre", data);

            foreach (string genre in allGenres)
            {
                TreeNode genreNode = root.AddNode(genre);

                List<Game> dataByGenre = Game.FilterData(data, "Genre", [genre]);

                string[] allPlatforms = Game.GetFieldsByName("Platforms", data);

                foreach (string platform in allPlatforms)
                {
                    TreeNode platformNode = genreNode.AddNode(platform);
                    List<Game> dataByGenreAndPlatform = Game.FilterData(dataByGenre, "Platforms", [platform]);
                    foreach (Game game in dataByGenreAndPlatform)
                    {
                        platformNode.AddNode(game.GetField("Name"));
                    }
                }
            }
            AnsiConsole.Write(root);
        }

        /// <summary>
        /// выводит календарь в консоль, согласно заданию
        /// </summary>
        /// <param name="data"></param>
        public static void PrintCalendar(List<Game> data)
        {
            Table table = new Table();
            table.Title("[cyan]Календарь[/]");

            string[] years = Game.GetFieldsByName("Year", data);
            List<int> countOfYears = [];
            int totalCountOfYears = 0;

            foreach (string year in years)
            {
                int yearCount = Game.FilterData(data, "Year", [year]).Count;
                totalCountOfYears += yearCount;
                countOfYears.Add(yearCount);
            }

            for (int i = 0; i < years.Length; i++)
            {
                double share = countOfYears[i] * 1.0 / totalCountOfYears;
                years[i] = $"[rgb({Math.Min(255, (int)(510*share))},{Math.Min(255, (int)(-510 * share + 510))},{0})]{years[i]}[/]";
            }
            

            int x = Math.Min(years.Length, 10);
            int y = years.Length/x + 1;

            for (int i = 0; i < x; i++)
            {
                table.AddColumn("");
            }

            for (int i = 0; i < y; i++)
            {

                if (i*x+x > years.Length)
                {
                    table.AddRow(years[(i * x)..]);
                }
                else
                {
                    table.AddRow(years[(i * x)..(i*x+x)]);

                }
            }
            table.ShowRowSeparators = true;
            table.ShowHeaders = false;
            table.Width(100);

            AnsiConsole.Write(table);


        }

        /// <summary>
        /// выводит диаграмму в консоль, согласно заданию
        /// </summary>
        /// <param name="data"></param>
        public static void PrintBarChart(List<Game> data)
        {
            Random rnd = new Random();

            Dictionary<string, int> countGamesByPlatforms = new Dictionary<string, int>();
            foreach (string platform in Game.GetFieldsByName("Platforms", data))
            { 
                countGamesByPlatforms[platform] = Game.FilterData(data, "Platforms", [platform]).Count;
            }

            BarChart barChart = new BarChart();

            foreach (var item in countGamesByPlatforms)
            {
                barChart.AddItem(item.Key, item.Value, new Spectre.Console.Color((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256)));
            }
            barChart.Width = 100;
            AnsiConsole.Write(barChart);
        }

        /// <summary>
        /// выводит обложку в консоль, согласно заданию
        /// </summary>
        /// <param name="data"></param>
        /// <param name="name"></param>
        public static void PrintImage(List<Game> data, string name)
        {
            foreach (Game game in data)
            {
                if (game.GetField("Name") == name)
                {
                    try
                    {
                        CanvasImage image = new CanvasImage(game.GetField("Cover"));
                        AnsiConsole.Write(image);
                        
                    }
                    catch
                    {
                        PrintMessage("Изображение не найдено");
                    }
                }
                
            }
        }
    }
}
