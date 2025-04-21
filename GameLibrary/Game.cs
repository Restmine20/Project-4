namespace GameLibrary
{
    /// <summary>
    /// Игра.
    /// </summary>
    public struct Game 
    {
        //начальный список жанров
        public static string[] DefaultGenres => ["Action", "RPG", "Strategy", "Simulator", "Adventure", "Puzzle", "Fighting", "Racing", "Sports", "Другое"];


        //начальный список статусов
        public static string[] DefaultStatuses => ["Не начата", "В процессе", "Пройдена", "Заброшена", "100% Completion"];

        //начальный список платформ
        public static string[] DefaultPlatforms => ["PC", "PS5", "Xbox Series X", "Nintendo Switch", "Mobile"];


        //список всех полей
        public static string[] AllFields => ["Name", "Platforms", "Genre", "Year", "Status", "Summary", "Cover", "Developer", "Publisher", "DLCs", "SimilarGames"];

        //конструкторы
        public Game(List<string> rawObject)
        {
            for (int i = 0; i < rawObject.Count; i++) 
            {
                fields[AllFields[i]] = rawObject[i];
            }
        }
        public Game()
        {
            for (int i = 0; i < AllFields.Length; i++)
            {
                fields[AllFields[i]] = "";
            }
        }

        //главное поле игры
        private Dictionary<string, string> fields = new Dictionary<string, string>();

        /// <summary>
        /// получает все значения поля массива игр
        /// </summary>
        /// <param name="fieldName">название поля</param>
        /// <param name="data">список игр</param>
        /// <returns>все значения поля массива игр</returns>
        public static string[] GetFieldsByName(string fieldName, List<Game> data)
        {
            List<string> result = [];
            if (fieldName == "Platforms")
            {
                foreach (Game game in data)
                {
                    string[] platforms = game.GetField(fieldName).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    foreach (string plat in platforms)
                    {
                        if (!result.Contains(plat))
                        {
                            result.Add(plat);
                        }
                    }
                    
                }
                return result.ToArray();
            }
            foreach(Game game in data)
            {
                string value = game.GetField(fieldName);
                if (!result.Contains(value))
                {
                    result.Add(value);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// получает значение поля
        /// </summary>
        /// <param name="fieldName">имя поля</param>
        /// <returns>значение поля</returns>
        public string GetField(string fieldName)
        {
            return fields[fieldName];
        }

        /// <summary>
        /// устанавливает значение поля
        /// </summary>
        /// <param name="fieldName">имя поля</param>
        /// <param name="value">значение</param>
        public void SetField(string fieldName, string value)
        {
            fields[fieldName] = value;
        }

        /// <summary>
        /// получает список всех значений игры
        /// </summary>
        /// <returns>массив значений</returns>
        public string[] GetAllValues()
        {
            List<string> result = [];
            foreach (string fieldName in AllFields)
            {
                result.Add(fields[fieldName]??"");
            }
            return result.ToArray();
        }

        /// <summary>
        /// фильтрует данные
        /// </summary>
        /// <param name="data">список игр</param>
        /// <param name="fieldName">по какому полю фильтрация</param>
        /// <param name="values">значения филтрации</param>
        /// <returns></returns>
        public static List<Game> FilterData(List<Game> data, string fieldName, string[] values)
        {
            List<Game> result = [];
            if (fieldName == "Platforms")
            {
                foreach (Game game in data)
                {
                    string[] platforms = game.GetField(fieldName).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (string plat in platforms)
                    {
                        if (values.Contains(plat))
                        {
                            result.Add(game);
                            break;
                        }
                    }
                    
                }
                return result;
            }
            foreach (Game game in data)
            {
                if (values.Contains(game.GetField(fieldName)))
                {
                    result.Add(game);
                }
            }
            return result;
        }

        /// <summary>
        /// сортирует данные
        /// </summary>
        /// <param name="data">список игр</param>
        /// <param name="fieldName">имя поля для сортировки</param>
        /// <param name="isIncreasing">направление сортировки</param>
        public static void SortData(List<Game> data, string fieldName, bool isIncreasing)
        {
            Comparison<Game> comparison = (Game x, Game y) => isIncreasing ? string.Compare(x.GetField(fieldName), y.GetField(fieldName)) : -string.Compare(x.GetField(fieldName), y.GetField(fieldName));
            data.Sort(comparison);
        }

        /// <summary>
        /// переопределенный tostring
        /// </summary>
        /// <returns>строковое представление игры для консоли</returns>
        public override string ToString()
        {
            string result = "";
            foreach(var item in fields)
            {
                result += $"{item.Key}: {item.Value}\n\r";
            }
            return result;
        }


       
    }
}
