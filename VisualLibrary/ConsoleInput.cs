using Spectre.Console;

namespace ConsoleInteraction
{
    /// <summary>
    /// класс содержит методы получения информации от пользователя через консоль
    /// </summary>
    public static class ConsoleInput
    {
       /// <summary>
       /// дают пользователю выбрать пункт меню
       /// </summary>
       /// <param name="IsEmpty"></param>
       /// <returns></returns>
        public static int MainMenu(bool IsEmpty)
        {
            string[] items = [$"[green]1.[/] { ((IsEmpty) ? "Загрузить данные из консоли/файла" : "Изменить входные данные")}", "[green]2.[/] Просмотр всех игр","[green]3.[/] Добавить новую игры","[green]4.[/] Редактировать информацию об играх","[green]5.[/] Удалить игру","[green]6.[/] Сохранить в файл", "[green]7.[/] Перемещение каталогов", "[green]8.[/] Завершить программу"];
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Введите номер пункта меню для запуска действия[/]:")
                    .AddChoices(items));
            return Array.IndexOf(items, choice);
            
        }

        /// <summary>
        /// дает пользователю выбор из опций
        /// </summary>
        /// <param name="items">опции выбора</param>
        /// <param name="message">сообщение пользователю</param>
        /// <param name="isMultiChoice">множественный ли выбор</param>
        /// <returns></returns>
        public static string[] GetChoice(string[] items, string message, bool isMultiChoice = false)
        {
            List<string> choices = [];
            if (items.Length == 0)
            {
                return [""];
            }
            if (!isMultiChoice)
            {
                choices.Add(AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title(message)
                        .AddChoices(items)));
            }
            else
            {
                choices = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title(message)
                        .InstructionsText(
                            "[grey](Нажмите [blue]<SPACE>[/], чтобы выбрать опцию, " + "[green]<ENTER>[/], чтобы подтвердить выбор)[/]")
                        .AddChoices(items));
            }
            return choices.ToArray();


        }

        /// <summary>
        /// получает от пользователя информацию из консоли
        /// </summary>
        /// <param name="message">сообщение в консоль</param>
        /// <param name="predicate">условие которому должны удовлетворять введенные данные</param>
        /// <returns></returns>
        public static string GetInputFromUser(string message, Predicate<string> predicate)
        {
            string? result;
            do
            {
                ConsoleOutput.PrintMessage(message, ConsoleColor.Yellow);
                result = Console.ReadLine();
            }
            while (result == null || !predicate(result));
            return result;
        }

    }
}
