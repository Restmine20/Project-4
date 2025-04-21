/*Ковалев Иван Андреевич
 * БПИ 2410, подгруппа 1
 * Проект 3_2, вариант 10
 * B_side*/
using GameLibrary;
using ConsoleInteraction;
using HTTPLibrary;
namespace Program
{
    /// <summary>
    /// Основной класс программы
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Клиент, с помощью которого осуществляются HTTP запросы
        /// </summary>
        public static Client client = new Client();
        internal static string ReservePath => Directory.GetCurrentDirectory() +Path.DirectorySeparatorChar + "reserve.txt";
        private static void Main()
        {
            //различные каталоги
            List<Game> mainData = [];
            List<Game> completedData = [];
            List<Game> wannaPlayData = [];


            string dataCursor = "mainData";
            
            while (true)
            {

                //узнаем какой каталог используется
                ref List<Game> data = ref mainData;
                if (dataCursor == "completedData")
                {
                    data = ref completedData;
                }
                if (dataCursor == "wannaPlayData")
                {
                    data = ref wannaPlayData;
                }


                int pickedItem = ConsoleInput.MainMenu(data.Count == 0);


                if (Menu.ChooseMenuItem(ref data, pickedItem, data.Count == 0, client, mainData, completedData, wannaPlayData, ref dataCursor))
                {
                    ConsoleOutput.PrintMessage("Операция успешно выполнена!", ConsoleColor.Green);
                }


                ConsoleOutput.PrintMessage("Чтобы вернуться в главное меню, нажмите TAB (если хотите вернуться и очистить консоль - нажмите ESC)", ConsoleColor.White);

                ConsoleKeyInfo key = Console.ReadKey(true);
                while (key.Key is not ConsoleKey.Escape and not ConsoleKey.Tab)
                {
                    key = Console.ReadKey(true);
                }
                if (key.Key == ConsoleKey.Escape)
                {
                    Console.Clear();
                  
                }
            }
        }

    }

}
