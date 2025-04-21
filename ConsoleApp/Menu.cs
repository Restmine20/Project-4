using GameLibrary;
using ConsoleInteraction;
using ParserLibrary;
using System.Security;
using HTTPLibrary;
namespace Program
{
    /// <summary>
    /// Класс меню
    /// </summary>
    internal static class Menu
    {
        /// <summary>
        /// Распределяет какой пункт меню выполняется
        /// </summary>
        /// <param name="data">выбранный каталог игр</param>
        /// <param name="pickedItem">выбранный пункт меню</param>
        /// <param name="isEmpty">проверка на пустоту каталога</param>
        /// <param name="client">клиент для http обращений</param>
        /// <param name="mainData">основной каталог</param>
        /// <param name="completedData">каталог пройденных игр</param>
        /// <param name="wannaPlayData">список игр которые пользователь хочет пройти</param>
        /// <param name="dataCursor">указатель на используемый каталог</param>
        /// <returns>успешно ли завершена программа</returns>
        internal static bool ChooseMenuItem(ref List<Game> data, int pickedItem, bool isEmpty, Client client, List<Game> mainData, List<Game> completedData, List<Game> wannaPlayData, ref string dataCursor)
        {
            bool isSuccess = false;
            switch (pickedItem + 1)
            {
                case 1:
                    isSuccess = MenuItem1(out data, client);

                    break;
                case 2:

                    if (isEmpty) { ConsoleOutput.PrintMessage("Сначала введите данные", ConsoleColor.Red); break; }
                    isSuccess = MenuItem2(ref data);
                    break;
                case 3:
                    isSuccess = MenuItem3(data);
                    break;
                case 4:

                    if (isEmpty) { ConsoleOutput.PrintMessage("Сначала введите данные", ConsoleColor.Red); break; }
                    isSuccess = MenuItem4(data);
                    break;
                case 5:

                    if (isEmpty) { ConsoleOutput.PrintMessage("Сначала введите данные", ConsoleColor.Red); break; }
                    isSuccess = MenuItem5(data);
                    break;
                case 6:

                    if (isEmpty) { ConsoleOutput.PrintMessage("Сначала введите данные", ConsoleColor.Red); break; }
                    isSuccess = MenuItem6(data);
                    break;
                case 7:
                    var sData = data;
                    isSuccess = MenuItem7(mainData, completedData, wannaPlayData, ref dataCursor);
                    break;
                case 8:
                    MenuItem8(data);
                    break;

            }
            return isSuccess;
        }

        /// <summary>
        /// Считывает данные из файла
        /// </summary>
        /// <param name="data">выбранный каталог</param>
        /// <param name="client">клиент для http обращений</param>
        /// <returns>успешно ли выполнен пункт</returns>
        private static bool MenuItem1(out List<Game> data, Client client)
        {
            string inputWay = ConsoleInput.GetChoice(["Резервный файл", "Собственный файл"], "Выберите, из какого файла надо считать данные")[0];
            switch (inputWay)
            {
                case "Резервный файл":
                    try
                    {
                        ConsoleStream.SetInputStream("reserve.txt");
                        data = Reader.ReadFile(client);
                        
                        return true;
                    }
                    catch
                    {
                        ConsoleOutput.PrintMessage("Резервный файл не был найден или поврежден, попробуйте собственный файл", ConsoleColor.Red);
                        data = [];
                        return false;
                    }
                    finally
                    {
                        ConsoleStream.SetDefaultInputStream();
                        
                    }
                case "Собственный файл":
                    string? path = null;
                    path = ConsoleInput.GetInputFromUser("Введите корректный путь до доступного файла, откуда надо считать данные", File.Exists);

                    //Ловим разные ошибки
                    try
                    {
                        ConsoleStream.SetInputStream(path);
                        data = Reader.ReadFile(client);

                    }
                    catch (SecurityException)
                    {
                        data = [];
                        ConsoleOutput.PrintMessage("Нет прав для чтения из указанного файла", ConsoleColor.Red);
                        return false;
                    }
                    catch (FormatException)
                    {
                        data = [];
                        ConsoleOutput.PrintMessage("Указанный JSON файл невалидный или не соотвествует структуре файла варианта", ConsoleColor.Red);
                        return false;
                    }
                    catch (IOException)
                    {
                        data = [];
                        ConsoleOutput.PrintMessage("Произошла ошибка ввода", ConsoleColor.Red);
                        return false;
                    }
                    
                    finally
                    {
                        ConsoleStream.SetDefaultInputStream();
                    }
                    return true;
                default:
                    data = [];
                    return false;
            
            }

        }

        /// <summary>
        /// визуализация даных
        /// </summary>
        /// <param name="data">выбранный каталог</param>
        /// <returns>успешно ли выполнен пункт</returns>
        private static bool MenuItem2(ref List<Game> data)
        {
            
            string[] types = ["[green]1.[/] Интерактивная таблица", "[green]2.[/] Дерево", "[green]3.[/] Календарь", "[green]4.[/] Диаграмма", "[green]5.[/] Вывести обложки"];
            int visualType = Array.IndexOf(types, ConsoleInput.GetChoice(types, "[yellow]Выберите тип визуализаии:[/]")[0]) + 1;
            
            switch (visualType)
            {
                case 1:
                    List<Game> copiedData = [];
                    foreach (Game game in data)
                    {
                        copiedData.Add(game);
                    }
                Start:

                    ConsoleOutput.PrintTable(copiedData);
                    string task = ConsoleInput.GetChoice(["[green]1.[/] Сортировка", "[green]2.[/] Фильтрация", "[green]3.[/] Вернуться в меню"], "[yellow]Выберите действие с таблицей[/]")[0];
                    switch (task)
                    {
                        case "[green]1.[/] Сортировка":
                            string[] sortFields = ConsoleInput.GetChoice(Game.AllFields, "[yellow]Выберите поля для сортировки[/]");
                            for (int i = 0; i < sortFields.Length; i++)
                            {
                                Game.SortData(copiedData, sortFields[i], ConsoleInput.GetChoice(["По возрастанию", "По убыванию"], "Выберите направление сортировки")[0] == "По возрастанию");
                            }
                            Console.Clear();
                            goto Start;

             
                        case "[green]2.[/] Фильтрация":
                            string filterField = ConsoleInput.GetChoice(Game.AllFields, "[yellow]Выберите поле для фильтрации[/]")[0];
                            string[] filterValues = ConsoleInput.GetChoice(Game.GetFieldsByName(filterField, data), "Выберите значения для фильтрации", true);

                            

                            copiedData = Game.FilterData(copiedData, filterField, filterValues);
                            Console.Clear();
                            goto Start;

                        case "[green]3.[/] Вернуться в меню":
                            if (ConsoleInput.GetChoice(["[green]Да[/]", "[red]Нет[/]"], "[yellow]Сохранить изменения в каталог?[/]")[0] == "[green]Да[/]")
                            {
                                data = copiedData;
                            }
                            return true;
                    }
                    break;
                case 2:
                    ConsoleOutput.PrintTree(data);
                    break;
                case 3:
                    ConsoleOutput.PrintCalendar(data);
                    break;
                case 4:
                    ConsoleOutput.PrintBarChart(data);
                    break;
                case 5:
                    string name = ConsoleInput.GetChoice(Game.GetFieldsByName("Name", data), "[yellow]Выберите имена игр[/]", true)[0];
                    ConsoleOutput.PrintImage(data, name);
                    break;
            }
            return true;
        }

        /// <summary>
        /// добавить игру в каталог
        /// </summary>
        /// <param name="data">выбранный каталог</param>
        /// <returns>успешно ли выполнен пункт</returns>
        private static bool MenuItem3(List<Game> data)
        {
            string[] addWays = ["[green]1.[/] Добавить игру вручную", "[green]2.[/] Найти игру в IGDB по ID", "[green]3.[/] Найти игру в IGDB по названию"];
            int addWay = Array.IndexOf(addWays, ConsoleInput.GetChoice(addWays, "[yellow]Выберите способ добавления игры[/]")[0]) + 1;
            switch (addWay)
            {
                case 1:
                    List<string> rawGame = [];
                    foreach (string field in Game.AllFields)
                    {
                        rawGame.Add(FillField(field));
                    }

                    data.Add(new Game(rawGame));
                    break;
                case 2:
                    string id = ConsoleInput.GetInputFromUser("Ввведите ID искомой игры", delegate (string x) { foreach (char i in x) { if (!char.IsDigit(i)) return false; } return x.Length>0; });
                    Game addGameById = Program.client.FindGame("id", id) ?? new Game();

                    ConsoleOutput.PrintMessage(addGameById.ToString(), ConsoleColor.Cyan);

                    string isCorrectGameById = ConsoleInput.GetChoice(["[green]Да[/]", "[red]Нет[/]"], "[yellow]Это нужная игра[/]?")[0];
                    if (isCorrectGameById == "[green]Да[/]")
                    {
                        addGameById.SetField("Status", FillField("Status"));
                        data.Add(addGameById);
                    }
                    else
                    {
                        ConsoleOutput.PrintMessage("Попробуйте добавить игру вручную", ConsoleColor.Magenta);
                        return false;
                    }
                    break;
                case 3:
                    string name = ConsoleInput.GetInputFromUser("Ввведите название искомой игры", (x) => true);
                    Game addGameByName = Program.client.FindGame("name", name) ?? new Game();

                    ConsoleOutput.PrintMessage(addGameByName.ToString(), ConsoleColor.Cyan);

                    string isCorrectGameByName = ConsoleInput.GetChoice(["[green]Да[/]", "[red]Нет[/]"], "[yellow]Это нужная игра?[/]")[0];
                    if (isCorrectGameByName == "[green]Да[/]")
                    {
                        addGameByName.SetField("Status", FillField("Status"));
                        data.Add(addGameByName);
                    }
                    else
                    {
                        ConsoleOutput.PrintMessage("Попробуйте добавить игру вручную", ConsoleColor.Magenta);
                        return false;
                    }
                    break;
            }
            
            return true;
        }

        /// <summary>
        /// Редактирование информации об игре
        /// </summary>
        /// <param name="data">выбранный каталог</param>
        /// <returns>успешно ли выполнен пункт</returns>
        private static bool MenuItem4(List<Game> data)
        {
            string[] namesOfGames = Game.GetFieldsByName("Name", data);
            string nameOfEditableGame = ConsoleInput.GetChoice(namesOfGames, "[yellow]Выберите название игры, информацию о которой хотите изменить[/]")[0];
            string nameOfEditableField = ConsoleInput.GetChoice(Game.AllFields, "[yellow]Выберите название поля, котороое хотите изменить[/]")[0];


            string newValue = FillField(nameOfEditableField);
            foreach (Game game in data)
            {
                if (game.GetField("Name") == nameOfEditableGame)
                {
                    game.SetField(nameOfEditableField, newValue);
                }
            }
            return true;
        }

        /// <summary>
        /// удаляет игру
        /// </summary>
        /// <param name="data">выбранный каталог</param>
        /// <returns>успешно ли выполнен пункт</returns>
        private static bool MenuItem5(List<Game> data)
        {
            string[] namesOfGames = Game.GetFieldsByName("Name", data);
            string nameOfDeletedGame = ConsoleInput.GetChoice(namesOfGames, "[yellow]Выберите название игры, которую хотите удалить[/]")[0];

            foreach (Game game in data)
            {
                if (game.GetField("Name") == nameOfDeletedGame)
                {
                    data.Remove(game);
                    break;
                }
            }
            return true;
        }

        /// <summary>
        /// сохраняет данные в пользовательский файл
        /// </summary>
        /// <param name="data">выбранынй каталог</param>
        /// <returns>успешно ли выполнен пункт</returns>
        private static bool MenuItem6(List<Game> data)
        {
            string? path = null;
            path = ConsoleInput.GetInputFromUser("Введите корректный путь до директории с выходным файлом", Directory.Exists);

            string? fileName = null;
            fileName = ConsoleInput.GetInputFromUser("Введите корректное название файла", (x) => x.IndexOfAny(Path.GetInvalidFileNameChars()) == -1 && x.Length >= 5 && x[^4..] == ".txt");
            //Ловим разные ошибки
            try
            {
                ConsoleStream.SetOutputStream(path + Path.DirectorySeparatorChar + fileName);
                Writer.WriteFile(data);
            }
            catch (UnauthorizedAccessException)
            {
                ConsoleOutput.PrintMessage("Нет прав для записи в указанный файл", ConsoleColor.Red);
                return false;
            }
            catch (SecurityException)
            {
                ConsoleOutput.PrintMessage("Нет прав для записи в указанный файл", ConsoleColor.Red);
                return false;
            }
            catch (ArgumentException)
            {
                ConsoleOutput.PrintMessage("Указанный файл недоступен для записи", ConsoleColor.Red);
                return false;
            }
            catch (IOException)
            {
                ConsoleOutput.PrintMessage("Произошла ошибка ввода", ConsoleColor.Red);
                return false;
            }
            finally
            {
                ConsoleStream.SetDefaultOutputStream();
            }
            return true;
        }

        /// <summary>
        /// выбирает между каталогами и перемещает игры
        /// </summary>
        /// <param name="mainData">основной каталог</param>
        /// <param name="completedData">каталог пройденных игр</param>
        /// <param name="wannaPlayData">игры в которые пользователь хочет поиграть</param>
        /// <param name="dataCursor">указать на выбранный массив</param>
        /// <returns></returns>
        private static bool MenuItem7(List<Game> mainData, List<Game> completedData, List<Game> wannaPlayData, ref string dataCursor)
        {
            if (ConsoleInput.GetChoice(["[green]1.[/] Поменять текущий каталог", "[green]2.[/] Переместить игру в другой каталог"], "[yellow]Выберите необходимое действие[/]")[0] == "[green]1.[/] Поменять текущий каталог")
            {
                string newCataloge = ConsoleInput.GetChoice(["[green]1.[/] Основной каталог", "[green]2.[/] Пройденные игры", "[green]3.[/] \"Хочу поиграть\""], "[yellow]Выберите новый каталог[/]")[0];
                switch (newCataloge)
                {
                    case "[green]1.[/] Основной каталог":
                        dataCursor = "mainData";
                        break;
                    case "[green]2.[/] Пройденные игры":
                        dataCursor = "completedData";
                        break;
                    case "[green]3.[/] \"Хочу поиграть\"":
                        dataCursor = "wannaPlayData";
                        break;
                }
                return true;
            }
            else
            {
                string rawOldCataloge = ConsoleInput.GetChoice(["[green]1.[/] Основной каталог", "[green]2.[/] Пройденные игры", "[green]3.[/] \"Хочу поиграть\""], "[yellow]Выберите каталог, откуда надо переместить игру[/]")[0];
                List<Game> oldCataloge = (rawOldCataloge == "[green]1.[/] Основной каталог") ? mainData : ((rawOldCataloge == "[green]2.[/] Пройденные игры") ? completedData : wannaPlayData);

                string nameOfGame = ConsoleInput.GetChoice(Game.GetFieldsByName("Name", oldCataloge), "[yellow]Выберите нужную игру[/]")[0];

                if (nameOfGame == "")
                {
                    ConsoleOutput.PrintMessage("Пустой каталог!", ConsoleColor.Red);
                    return false;
                }

                string rawNewCataloge = ConsoleInput.GetChoice(["[green]1.[/] Основной каталог", "[green]2.[/] Пройденные игры", "[green]3.[/] \"Хочу поиграть\""], "[yellow]Выберите каталог, в который надо переместить игру[/]")[0];

                List<Game> newCataloge = (rawNewCataloge == "[green]1.[/] Основной каталог") ? mainData : ((rawNewCataloge == "[green]2.[/] Пройденные игры") ? completedData : wannaPlayData);
                foreach (Game game in oldCataloge)
                {
                    if (game.GetField("Name") == nameOfGame)
                    {
                        oldCataloge.Remove(game);
                        newCataloge.Add(game);
                        break;
                    }
                }
                return true;

            }
        }

        /// <summary>
        /// завершает программу и записывает в файл резервной копии
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool MenuItem8(List<Game> data)
        {
            try
            {
                ConsoleStream.SetOutputStream(Program.ReservePath);

                Writer.WriteFile(data);

                ConsoleStream.SetDefaultOutputStream();

                Environment.Exit(0);

            }
            catch
            {
                ConsoleStream.SetDefaultOutputStream();

                ConsoleOutput.PrintMessage("Ошибка резервного сохранения данных", ConsoleColor.Red);
                string confirm = ConsoleInput.GetChoice(["[red]Да, завершить без сохранения[/]", "[green]Нет, вернуться в меню[/]"], "[yellow]Вы точно хотите завершить программу без возможности восстановления данных?[/]")[0];
                if (confirm == "[red]Да, завершить без сохранения[/]")
                {
                    Environment.Exit(0);
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// просит значения полей игры
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static string FillField(string fieldName)
        {
            switch (fieldName)
            {
                case "Name":
                    return ConsoleInput.GetInputFromUser("Введите название добавляемой игры:", (x) => true);
                case "Platforms":
                    return string.Join(", ", ConsoleInput.GetChoice(Game.DefaultPlatforms, "Выберите платформы, на которых доступна игра", true));
                case "Genre":
                    return ConsoleInput.GetChoice(Game.DefaultGenres, "Выберите подходящий жанр")[0];
                case "Year":
                    return ConsoleInput.GetInputFromUser("Введите год выпуска игры", delegate (string x) { foreach (char i in x) { if (!char.IsDigit(i)) return false; } return true; });
                case "Status":
                    return ConsoleInput.GetChoice(Game.DefaultStatuses, "Выберите статус игры")[0];
                default:
                    return ConsoleInput.GetInputFromUser($"Введите значение поля {fieldName}:", (x) => true);
            }

        }
    }
}