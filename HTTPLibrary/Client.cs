using GameLibrary;
using Newtonsoft.Json;
using System.Text;
namespace HTTPLibrary
{
    /// <summary>
    ///  класс содержит нужные поля, конструктор, и методы для обращения к IDGB
    /// </summary>
    public class Client
    {
        ///
        public static string CoverPath => Path.GetFullPath(Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\") + @"VisualLibrary\covers";
        private readonly HttpClient client;
        
        /// <summary>
        /// конструктор
        /// </summary>
        public Client()
        {
            client = new HttpClient();
            client.BaseAddress = new System.Uri("https://api.igdb.com/v4");
            client.DefaultRequestHeaders.Add("Client-ID", "kmtn3ch149dytxhhc3crqueex461zy");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer vfpkne3wlcz3nj2v8t7dradci648s8");
            Directory.CreateDirectory(CoverPath);
        }

        /// <summary>
        /// ищет игру в IDGB
        /// </summary>
        /// <param name="findingField">название поля которое ищем ID или name</param>
        /// <param name="valueOfFindingField">его значение</param>
        /// <returns>найденная игра</returns>
        public Game? FindGame(string findingField, string valueOfFindingField)
        {
            valueOfFindingField = (findingField == "id") ? valueOfFindingField : $"\"{valueOfFindingField}\"";

            StringContent request = new($"fields name, cover, summary, dlcs, similar_games, platforms, genres, first_release_date, involved_companies; where {findingField} = {valueOfFindingField};");
            var result = client.PostAsync(client.BaseAddress+"/games", request).Result;

            string json = result.Content.ReadAsStringAsync().Result;
            Dictionary<string, string> rawGameFromJson;
            try
            {
                rawGameFromJson = Deserialize(json)[0];
            }
            catch 
            {
                return null;
            }

            string[] rawGame = new string[11];
            foreach (var item in rawGameFromJson)
            {
                switch (item.Key)
                {
                    case "name":
                        rawGame[0] = item.Value; 
                        break;
                    case "platforms":
                        List<Dictionary<string, string>> listWithPlatforms = GetItemsByIds(item.Key, rawGameFromJson[item.Key], ["name"]);
                        List<string> rawPlatforms = [];
                        foreach (var i in listWithPlatforms)
                        {
                            rawPlatforms.Add(i["name"]);
                        }
                        rawGame[1] = string.Join(",", rawPlatforms);
                        break;
                    case "genres":
                        List<Dictionary<string, string>> listWithGenres = GetItemsByIds(item.Key, rawGameFromJson[item.Key], ["name"]);
                        rawGame[2] = listWithGenres[0]["name"];
                        break;
                    case "first_release_date":
                        long year = Convert.ToInt64(rawGameFromJson[item.Key])/31536000 + 1970;
                        rawGame[3] = year.ToString();
                        break;
                    case "summary":
                        rawGame[5] = rawGameFromJson[item.Key];
                        break;
                    case "cover":
                        string rawCoverId = rawGameFromJson[item.Key];
                        rawGame[6] = GetItemsByIds("covers", rawCoverId, ["url"])[0]["url"]; 
                        break;
                    case "involved_companies":
                        List<Dictionary<string, string>> listWithInvolvedCompanies = GetItemsByIds(item.Key, rawGameFromJson[item.Key], ["developer", "publisher", "company"]);
                        foreach (var involvedCompany in listWithInvolvedCompanies)
                        {
                            if (involvedCompany["developer"] == "true")
                            {
                                rawGame[7] = GetItemsByIds("companies", involvedCompany["company"], ["name"])[0]["name"];
                            }
                            if (involvedCompany["publisher"] == "true")
                            {
                                rawGame[8] = GetItemsByIds("companies", involvedCompany["company"], ["name"])[0]["name"];
                            }
                        }
                        break;
                    case "dlcs":
                        List<Dictionary<string, string>> listWithDlcs = GetItemsByIds("games", rawGameFromJson[item.Key], ["name"]);
                        List<string> rawDlcs = [];
                        foreach (var i in listWithDlcs)
                        {
                            rawDlcs.Add(i["name"]);
                        }
                        rawGame[9] = string.Join(",", rawDlcs);
                        break;
                    case "similar_games":
                        List<Dictionary<string, string>> listWithSimilarGames = GetItemsByIds("games", rawGameFromJson[item.Key], ["name"]);
                        List<string> rawSimilarGames = [];
                        foreach (var i in listWithSimilarGames)
                        {
                            rawSimilarGames.Add(i["name"]);
                        }
                        rawGame[10] = string.Join(",", rawSimilarGames);
                        break;

                }
            }
            Game game = new Game(new List<string>(rawGame));
            DownloadCover(game, CoverPath);
            return game;
        }

        /// <summary>
        /// ищет по id нужные элемент в базе данных
        /// </summary>
        /// <param name="name">название искомого объекта</param>
        /// <param name="values">какие ID может принимать</param>
        /// <param name="fields">какие поля нужны от объекта</param>
        /// <returns>спаршенный JSON</returns>
        private List<Dictionary<string, string>> GetItemsByIds(string name, string values, string[] fields)
        {
            StringContent request = new($"fields {string.Join(',', fields)}; where id = ({values});");
            string json = client.PostAsync(client.BaseAddress + $"/{name}", request).Result.Content.ReadAsStringAsync().Result;

            return Deserialize(json);
        }

        /// <summary>
        /// загружает обложку
        /// </summary>
        /// <param name="game">название игры</param>
        /// <param name="dir">куда загружает</param>
        private void DownloadCover(Game game, string dir)
        {
            var stream = client.GetStreamAsync(game.GetField("Cover")).Result;
            string fileName = game.GetField("Name");
            for (int i = 0; i < fileName.Length; i++) 
            {
                if (Path.GetInvalidFileNameChars().Contains(fileName[i]))
                {
                    fileName = fileName[..i];
                }
            }
            game.SetField("Cover", $"{dir}\\{fileName}.{game.GetField("Cover")[^3..]}");
            using var file = File.OpenWrite(game.GetField("Cover"));

            stream.CopyToAsync(file);
        }

        /// <summary>
        /// десериализует строку
        /// </summary>
        /// <param name="json">входная строка</param>
        /// <returns>список словарей..</returns>
        private List<Dictionary<string, string>> Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(JsonHandler(json));
        }

        /// <summary>
        /// подготавливает строку к десериализации
        /// </summary>
        /// <param name="rawJson">строка</param>
        /// <returns>готовая строка</returns>
        private string JsonHandler(string rawJson)
        {
            StringBuilder result = new StringBuilder();
            result.Append('[');
            bool isQuote = false;
            for (int i = 1; i < rawJson.Length-1; i++)
            {
                if (rawJson[i] == '"' && rawJson[i - 1] != '\\')
                {
                    isQuote = !isQuote;
                }
                result.Append((rawJson[i] == '[' || rawJson[i] == ']') ? ((isQuote)? '\'' : '"') : rawJson[i]);
                
            }
            result.Append("]");
            return result.ToString();
        }

    }
}
