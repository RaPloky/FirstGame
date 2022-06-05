using System;
namespace SeaBattle
{
    class MainLogic
    {
        // Gets a char number which describes a count of cells of ship. It's need for right hitboxes for 2-cell and 3-cell ships.
        public char getKnowAboutShip;

        public int[] blocked_cells = new int[130];
        public int[] bot_ship_places = new int[20];
        public int[] player_hitten_cells = new int[101];

        public int index = 0; // It's for dots right indexing
        public byte player_hits_counter = 0, bot_hits_counter = 0;
        public byte how_many_3_cell = 0, how_many_2_cell = 0, how_many_one_cell = 0; // For right recursion
        public int Nq = 100, Nm = 100, bot_shoot_order = 0, player_hit_order = 0;
        public int[] unique = new int[100];

        public bool is_somebody_win = false;
        public string choosen_line, choosen_column, choosen_position, what_ship_it_is; // The last one is for 3 and 2 cells ships
        public string A1, A2, A3, A4, A5, A6, A7, A8, A9, A0, /*A --> B*/ B1, B2, B3, B4, B5, B6, B7, B8, B9, B0,
                      C1, C2, C3, C4, C5, C6, C7, C8, C9, C0, /*C --> D*/ D1, D2, D3, D4, D5, D6, D7, D8, D9, D0,
                      E1, E2, E3, E4, E5, E6, E7, E8, E9, E0, /*E --> F*/ F1, F2, F3, F4, F5, F6, F7, F8, F9, F0,
                      G1, G2, G3, G4, G5, G6, G7, G8, G9, G0, /*G --> H*/ H1, H2, H3, H4, H5, H6, H7, H8, H9, H0,
                      I1, I2, I3, I4, I5, I6, I7, I8, I9, I0, /*I --> J*/ J1, J2, J3, J4, J5, J6, J7, J8, J9, J0;
        public string LineName(int counter)
        {
            string lineChar = "";
            switch (counter)
            {
                case 0: lineChar = "A"; break;
                case 10: lineChar = "B"; break;
                case 20: lineChar = "C"; break;
                case 30: lineChar = "D"; break;
                case 40: lineChar = "E"; break;
                case 50: lineChar = "F"; break;
                case 60: lineChar = "G"; break;
                case 70: lineChar = "H"; break;
                case 80: lineChar = "I"; break;
                case 90: lineChar = "J"; break;
            }
            return lineChar;
        }
        public void DisplayPlayerBattlefield(string[] playerCellsArray)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(7, 0);
            Console.WriteLine("\t       Your field\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(5, 2);
            Console.Write("\t   1 2 3 4 5 6 7 8 9 0");
            for (byte i = 0; i < 100; i++)
            {
                if (i % 10 == 0)
                {
                    Console.WriteLine();
                    Console.Write($"\t {LineName(i)} ");
                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                if (playerCellsArray[i] == ">" || playerCellsArray[i] == "v")
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(playerCellsArray[i] + ' ');
                }
                if (playerCellsArray[i] == ".")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(playerCellsArray[i] + ' ');
                }
                if (playerCellsArray[i] == "X")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(playerCellsArray[i] + ' ');
                }
                if (playerCellsArray[i] == "o")
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(playerCellsArray[i] + ' ');
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void DisplayEnemyBattlefield(string[] botCellsArray)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(35, 0);
            Console.WriteLine("\t      Enemy's field\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(30, 2);
            Console.Write("\t\t   1 2 3 4 5 6 7 8 9 0");
            for (byte i = 0; i < 100; i++)
            {
                if (i % 10 == 0)
                {
                    Console.WriteLine();
                    Console.Write($"\t\t\t\t\t {LineName(i)} ");
                }
                if (botCellsArray[i] == ".")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(botCellsArray[i] + ' ');
                }
                if (botCellsArray[i] == "X")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(botCellsArray[i] + ' ');
                }
                if (botCellsArray[i] == "o")
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(botCellsArray[i] + ' ');
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            Console.WriteLine("\n");
        }
        public void Loading()
        {
            Console.SetCursorPosition(16, 19);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\n\t\tWait:");

            for (double i = 1.0; i > 0; i -= 0.1)
            {
                Console.SetCursorPosition(22, 20);
                Console.WriteLine(Math.Round(i, 1));
                System.Threading.Thread.Sleep(100);
            }
            Console.Clear();
        }

        public void AskAboutLine()
        {
            Console.Write("\t   Line is [A to J]: ");
            choosen_line = Console.ReadLine();
            choosen_line = choosen_line.ToUpper();
        }
        public void AskAboutColumn()
        {
            Console.Write("\t  Column is [1 to 0]: ");
            choosen_column = Console.ReadLine();
        }
        public void AskAboutPosition()
        {
            Console.Write("\tHorizontal/Vertical [H/V]: ");
            choosen_position = Console.ReadLine();
            choosen_position = choosen_position.ToUpper();
        }
        public int ChooseLine(string choosenLine)
        {
            int increase = 0;
            switch (choosenLine)
            {
                case "A": increase = 0; break;
                case "B": increase = 10; break;
                case "C": increase = 20; break;
                case "D": increase = 30; break;
                case "E": increase = 40; break;
                case "F": increase = 50; break;
                case "G": increase = 60; break;
                case "H": increase = 70; break;
                case "I": increase = 80; break;
                case "J": increase = 90; break;
            }
            return increase;
        }

        public void DrawingHorizontal4Cell(string[] playerCellsArray)
        {
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            if (integerColumn == 8) integerColumn -= 1;
            if (integerColumn == 9) integerColumn -= 2;
            if (integerColumn == 0) integerColumn = 7;

            for (int i = integerColumn - 1 + increase; i < integerColumn + 3 + increase; i++)
                playerCellsArray[i] = ">";
        }
        public void DrawingHorizontal3Cell(string[] playerCellsArray)
        {
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);

            if (integerColumn == 9) integerColumn -= 1;
            if (integerColumn == 0) integerColumn = 8;

            for (int i = integerColumn - 1 + increase; i < integerColumn + 2 + increase; i++)
                playerCellsArray[i] = ">";
        }
        public void DrawingHorizontal2Cell(string[] playerCellsArray)
        {
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            if (integerColumn == 0) integerColumn = 9;

            for (int i = integerColumn - 1 + increase; i < integerColumn + 1 + increase; i++)
                playerCellsArray[i] = ">";
        }
        public void DrawingHorizontal1Cell(string[] playerCellsArray)
        {
            int counter = 0;
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            if (integerColumn == 0) integerColumn += 10;

            for (int i = integerColumn - 1 + increase; counter < 1; i += 10)
            {
                playerCellsArray[i] = ">";
                counter += 1;
            }
        }

        public void OutputHorizontalShip4(MainLogic logic, string[] playerCellsArray)
        {
            switch (logic.choosen_column)
            {
                case "1": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "2": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "3": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "4": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "5": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "6": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "7": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "8": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "9": logic.DrawingHorizontal4Cell(playerCellsArray); break;
                case "0": logic.DrawingHorizontal4Cell(playerCellsArray); break;
            }
        }
        public void OutputHorizontalShip3(MainLogic logic, string[] playerCellsArray)
        {
            switch (logic.choosen_column)
            {
                case "1": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "2": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "3": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "4": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "5": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "6": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "7": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "8": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "9": logic.DrawingHorizontal3Cell(playerCellsArray); break;
                case "0": logic.DrawingHorizontal3Cell(playerCellsArray); break;
            }
        }
        public void OutputHorizontalShip2(MainLogic logic, string[] playerCellsArray)
        {
            switch (logic.choosen_column)
            {
                case "1": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "2": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "3": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "4": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "5": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "6": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "7": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "8": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "9": logic.DrawingHorizontal2Cell(playerCellsArray); break;
                case "0": logic.DrawingHorizontal2Cell(playerCellsArray); break;
            }
        }
        public void OutputHorizontalShip1(MainLogic logic, string[] playerCellsArray)
        {
            switch (logic.choosen_column)
            {
                case "1": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "2": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "3": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "4": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "5": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "6": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "7": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "8": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "9": logic.DrawingHorizontal1Cell(playerCellsArray); break;
                case "0": logic.DrawingHorizontal1Cell(playerCellsArray); break;
            }
        }

        public void DrawingVertical4Cell(string[] playerCellsArray)
        {
            int counter = 0;
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            if (integerColumn == 0) integerColumn += 10;

            if (increase == 70 || increase == 80 || increase == 90) increase = 60;

            for (int i = integerColumn - 1 + increase; counter < 4; i += 10)
            {
                playerCellsArray[i] = "v";
                counter += 1;
            }
        }
        public void DrawingVertical3Cell(string[] playerCellsArray)
        {
            int counter = 0;
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            if (integerColumn == 0) integerColumn += 10;
            if (increase == 80 || increase == 90) increase = 70;

            for (int i = integerColumn - 1 + increase; counter < 3; i += 10)
            {
                playerCellsArray[i] = "v";
                counter += 1;
            }
        }
        public void DrawingVertical2Cell(string[] playerCellsArray)
        {
            int counter = 0;
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            if (integerColumn == 0) integerColumn += 10;
            if (increase == 90) increase = 80;
            for (int i = integerColumn - 1 + increase; counter < 2; i += 10)
            {
                playerCellsArray[i] = "v";
                counter += 1;
            }
        }
        public void DrawingVertical1Cell(string[] playerCellsArray)
        {
            int counter = 0;
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            if (integerColumn == 0) integerColumn += 10;

            for (int i = integerColumn - 1 + increase; counter < 1; i += 10)
            {
                playerCellsArray[i] = "v";
                counter += 1;
            }
        }

        public void OutputVerticalShip4(MainLogic logic, string[] playerCellsArray)
        {
            switch (logic.choosen_column)
            {
                case "1": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "2": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "3": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "4": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "5": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "6": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "7": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "8": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "9": logic.DrawingVertical4Cell(playerCellsArray); break;
                case "0": logic.DrawingVertical4Cell(playerCellsArray); break;
            }
        }
        public void OutputVerticalShip3(MainLogic logic, string[] playerCellsArray)
        {
            switch (logic.choosen_column)
            {
                case "1": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "2": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "3": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "4": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "5": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "6": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "7": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "8": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "9": logic.DrawingVertical3Cell(playerCellsArray); break;
                case "0": logic.DrawingVertical3Cell(playerCellsArray); break;
            }
        }
        public void OutputVerticalShip2(MainLogic logic, string[] playerCellsArray)
        {
            switch (logic.choosen_column)
            {
                case "1": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "2": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "3": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "4": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "5": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "6": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "7": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "8": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "9": logic.DrawingVertical2Cell(playerCellsArray); break;
                case "0": logic.DrawingVertical2Cell(playerCellsArray); break;
            }
        }
        public void OutputVerticalShip1(MainLogic logic, string[] playerCellsArray)
        {
            switch (logic.choosen_column)
            {
                case "1": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "2": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "3": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "4": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "5": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "6": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "7": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "8": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "9": logic.DrawingVertical1Cell(playerCellsArray); break;
                case "0": logic.DrawingVertical1Cell(playerCellsArray); break;
            }
        }

        public void Ship4CellManagement(MainLogic logic, string[] playerCellsArray)
        {
            Console.WriteLine("\tShip placement: [> > > >]\n");
            logic.AskAboutPosition();
            logic.AskAboutLine();
            logic.AskAboutColumn();

            if (choosen_line != "A" & choosen_line != "B" & choosen_line != "C" & choosen_line != "D" & choosen_line != "E" &
                choosen_line != "F" & choosen_line != "G" & choosen_line != "H" & choosen_line != "I" & choosen_line != "J" ||
                choosen_column != "1" & choosen_column != "2" & choosen_column != "3" & choosen_column != "4" & choosen_column != "5" &
                choosen_column != "6" & choosen_column != "7" & choosen_column != "8" & choosen_column != "9" & choosen_column != "0" ||
                choosen_position != "H" & choosen_position != "V")
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\t\t     Incorrect input!");
                Console.WriteLine("\t\tPlease check it next time!");
                Console.WriteLine("\t\tPress \"ENTER\" to continue:");
                Console.ReadLine();
                Console.Clear();
                DisplayPlayerBattlefield(playerCellsArray);
                Ship4CellManagement(logic, playerCellsArray);
            }
            logic.Loading();

            switch (logic.choosen_position)
            {
                case "H":
                    switch (logic.choosen_line)
                    {
                        case "A": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "B": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "C": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "D": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "E": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "F": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "G": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "H": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "I": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                        case "J": logic.OutputHorizontalShip4(logic, playerCellsArray); break;
                    }
                    break;
                case "V":
                    switch (logic.choosen_line)
                    {
                        case "A": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "B": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "C": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "D": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "E": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "F": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "G": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "H": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "I": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                        case "J": logic.OutputVerticalShip4(logic, playerCellsArray); break;
                    }
                    break;
            }
            Console.Clear();
            logic.DisplayPlayerBattlefield(playerCellsArray);
            HitBox4Cell(logic.blocked_cells);
        }
        public void Ship3CellManagement(MainLogic logic, string[] playerCellsArray)
        {
            getKnowAboutShip = '3';
            Console.WriteLine("\t  Ship placement: [> > >]\n");
            logic.AskAboutPosition();
            if (choosen_position == "V") what_ship_it_is = "3V";
            if (choosen_position == "H") what_ship_it_is = "3H";
            logic.AskAboutLine();
            logic.AskAboutColumn();

            if (choosen_line != "A" & choosen_line != "B" & choosen_line != "C" & choosen_line != "D" & choosen_line != "E" &
                choosen_line != "F" & choosen_line != "G" & choosen_line != "H" & choosen_line != "I" & choosen_line != "J" ||
                choosen_column != "1" & choosen_column != "2" & choosen_column != "3" & choosen_column != "4" & choosen_column != "5" &
                choosen_column != "6" & choosen_column != "7" & choosen_column != "8" & choosen_column != "9" & choosen_column != "0" ||
                choosen_position != "H" & choosen_position != "V")
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\t\t     Incorrect input!");
                Console.WriteLine("\t\tPlease check it next time!");
                Console.WriteLine("\t\tPress \"ENTER\" to continue:");
                Console.ReadLine();
                Console.Clear();
                DisplayPlayerBattlefield(playerCellsArray);
                Ship2CellManagement(logic, playerCellsArray);
            }
            logic.CheckForFreePlace(logic, playerCellsArray);
            logic.Loading();

            switch (logic.choosen_position)
            {
                case "H":
                    switch (logic.choosen_line)
                    {
                        case "A": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "B": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "C": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "D": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "E": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "F": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "G": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "H": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "I": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                        case "J": logic.OutputHorizontalShip3(logic, playerCellsArray); break;
                    }
                    break;
                case "V":
                    switch (logic.choosen_line)
                    {
                        case "A": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "B": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "C": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "D": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "E": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "F": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "G": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "H": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "I": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                        case "J": logic.OutputVerticalShip3(logic, playerCellsArray); break;
                    }
                    break;
            }
            Console.Clear();
            logic.DisplayPlayerBattlefield(playerCellsArray);
            HitBox3Cell(logic.blocked_cells);

            how_many_3_cell++;
            if (how_many_3_cell < 2)
                Ship3CellManagement(logic, playerCellsArray);
        }
        public void Ship2CellManagement(MainLogic logic, string[] playerCellsArray)
        {
            getKnowAboutShip = '2';
            Console.WriteLine("\t   Ship placement: [> >]\n");
            logic.AskAboutPosition();
            if (choosen_position == "V") what_ship_it_is = "2V";
            if (choosen_position == "H") what_ship_it_is = "2H";
            logic.AskAboutLine();
            logic.AskAboutColumn();

            if (choosen_line != "A" & choosen_line != "B" & choosen_line != "C" & choosen_line != "D" & choosen_line != "E" &
                choosen_line != "F" & choosen_line != "G" & choosen_line != "H" & choosen_line != "I" & choosen_line != "J" ||
                choosen_column != "1" & choosen_column != "2" & choosen_column != "3" & choosen_column != "4" & choosen_column != "5" &
                choosen_column != "6" & choosen_column != "7" & choosen_column != "8" & choosen_column != "9" & choosen_column != "0" ||
                choosen_position != "H" & choosen_position != "V")
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\t\t     Incorrect input!");
                Console.WriteLine("\t\tPlease check it next time!");
                Console.WriteLine("\t\tPress \"ENTER\" to continue:");
                Console.ReadLine();
                Console.Clear();
                DisplayPlayerBattlefield(playerCellsArray);
                Ship2CellManagement(logic, playerCellsArray);
            }
            logic.CheckForFreePlace(logic, playerCellsArray);
            logic.Loading();

            switch (logic.choosen_position)
            {
                case "H":
                    switch (logic.choosen_line)
                    {
                        case "A": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "B": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "C": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "D": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "E": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "F": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "G": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "H": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "I": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                        case "J": logic.OutputHorizontalShip2(logic, playerCellsArray); break;
                    }
                    break;
                case "V":
                    switch (logic.choosen_line)
                    {
                        case "A": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "B": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "C": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "D": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "E": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "F": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "G": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "H": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "I": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                        case "J": logic.OutputVerticalShip2(logic, playerCellsArray); break;
                    }
                    break;
            }
            Console.Clear();
            logic.DisplayPlayerBattlefield(playerCellsArray);
            HitBox2Cell(logic.blocked_cells);

            how_many_2_cell++;
            if (how_many_2_cell < 3)
                Ship2CellManagement(logic, playerCellsArray);
            if (how_many_2_cell == 3)
                what_ship_it_is = "";
        }
        public void Ship1CellManagement(MainLogic logic, string[] player_cells_array)
        {
            getKnowAboutShip = '1';
            Console.WriteLine("\t    Ship placement: [>]\n");
            logic.AskAboutPosition();
            logic.AskAboutLine();
            logic.AskAboutColumn();

            if (choosen_line != "A" & choosen_line != "B" & choosen_line != "C" & choosen_line != "D" & choosen_line != "E" &
                choosen_line != "F" & choosen_line != "G" & choosen_line != "H" & choosen_line != "I" & choosen_line != "J" ||
                choosen_column != "1" & choosen_column != "2" & choosen_column != "3" & choosen_column != "4" & choosen_column != "5" &
                choosen_column != "6" & choosen_column != "7" & choosen_column != "8" & choosen_column != "9" & choosen_column != "0" ||
                choosen_position != "H" & choosen_position != "V")
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\t\t     Incorrect input!");
                Console.WriteLine("\t\tPlease check it next time!");
                Console.WriteLine("\t\tPress \"ENTER\" to continue:");
                Console.ReadLine();
                Console.Clear();
                DisplayPlayerBattlefield(player_cells_array);
                Ship1CellManagement(logic, player_cells_array);
            }
            logic.CheckForFreePlace(logic, player_cells_array);
            logic.Loading();

            switch (logic.choosen_position)
            {
                case "H":
                    switch (logic.choosen_line)
                    {
                        case "A": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "B": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "C": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "D": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "E": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "F": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "G": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "H": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "I": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                        case "J": logic.OutputHorizontalShip1(logic, player_cells_array); break;
                    }
                    break;
                case "V":
                    switch (logic.choosen_line)
                    {
                        case "A": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "B": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "C": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "D": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "E": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "F": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "G": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "H": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "I": logic.OutputVerticalShip1(logic, player_cells_array); break;
                        case "J": logic.OutputVerticalShip1(logic, player_cells_array); break;
                    }
                    break;
            }
            Console.Clear(); logic.DisplayPlayerBattlefield(player_cells_array);
            HitBox1Cell(logic.blocked_cells);

            how_many_one_cell++;
            if (how_many_one_cell < 4)
                Ship1CellManagement(logic, player_cells_array);
        }

        // A big space for indexing methods!
        // "HOR" is "HORizontal".
        // "Full" means what the method is blocking the cells on above, under and on middle:
        public int FullBackOnOneHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 - 1;
            blockedCells[index + 1] = mainPosition - 1;
            blockedCells[index + 2] = mainPosition + 10 - 1;
            return index += 3;
        }
        public int FullOnCurrentHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10;
            blockedCells[index + 1] = mainPosition;
            blockedCells[index + 2] = mainPosition + 10;
            return index += 3;
        }
        public int FullForwardOnOneHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 + 1;
            blockedCells[index + 1] = mainPosition + 1;
            blockedCells[index + 2] = mainPosition + 10 + 1;
            return index += 3;
        }
        public int FullForwardOnTwoHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 + 2;
            blockedCells[index + 1] = mainPosition + 2;
            blockedCells[index + 2] = mainPosition + 10 + 2;
            return index += 3;
        }
        public int FullForwardOnThreeHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 + 3;
            blockedCells[index + 1] = mainPosition + 3;
            blockedCells[index + 2] = mainPosition + 10 + 3;
            return index += 3;
        }
        public int FullForwardOnFourHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 + 4;
            blockedCells[index + 1] = mainPosition + 4;
            blockedCells[index + 2] = mainPosition + 10 + 4;
            return index += 3;
        }

        // "NotUnder" means what the method is blocking the cells on above and on middle exepting on under:
        public int NotUnderBackOnOneHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 - 1;
            blockedCells[index + 1] = mainPosition - 1;
            return index += 2;
        }
        public int NotUnderOnCurrentHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10;
            blockedCells[index + 1] = mainPosition;
            return index += 2;
        }
        public int NotUnderForwardOnOneHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 + 1;
            blockedCells[index + 1] = mainPosition + 1;
            return index += 2;
        }
        public int NotUnderForwardOnTwoHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 + 2;
            blockedCells[index + 1] = mainPosition + 2;
            return index += 2;
        }
        public int NotUnderForwardOnThreeHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 + 3;
            blockedCells[index + 1] = mainPosition + 3;
            return index += 2;
        }
        public int NotUnderForwardOnFourHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 + 4;
            blockedCells[index + 1] = mainPosition + 4;
            return index += 2;
        }

        // "NotAbove" means what the method is blocking the cells on under and on middle exepting on above:
        public int NotAboveBackOnOneHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 1;
            blockedCells[index + 1] = mainPosition + 10 - 1;
            return index += 2;
        }
        public int NotAboveOnCurrentHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition;
            blockedCells[index + 1] = mainPosition + 10;
            return index += 2;
        }
        public int NotAboveForwardOnOneHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 1;
            blockedCells[index + 1] = mainPosition + 10 + 1;
            return index += 2;
        }
        public int NotAboveForwardOnTwoHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 2;
            blockedCells[index + 1] = mainPosition + 10 + 2;
            return index += 2;
        }
        public int NotAboveForwardOnThreeHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 3;
            blockedCells[index + 1] = mainPosition + 10 + 3;
            return index += 2;
        }
        public int NotAboveForwardOnFourHOR(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 4;
            blockedCells[index + 1] = mainPosition + 10 + 4;
            return index += 2;
        }

        // "VERtical" methods:
        public int FullBackOnOneVER(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 - 1;
            blockedCells[index + 1] = mainPosition - 10;
            blockedCells[index + 2] = mainPosition - 10 + 1;
            return index += 3;
        }
        public int FullOnCurrentVER(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 1;
            blockedCells[index + 1] = mainPosition;
            blockedCells[index + 2] = mainPosition + 1;
            return index += 3;
        }
        public int FullForwardOnOneVER(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 10 - 1;
            blockedCells[index + 1] = mainPosition + 10;
            blockedCells[index + 2] = mainPosition + 10 + 1;
            return index += 3;
        }
        public int FullForwardOnTwoVER(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 20 - 1;
            blockedCells[index + 1] = mainPosition + 20;
            blockedCells[index + 2] = mainPosition + 20 + 1;
            return index += 3;
        }
        public int FullForwardOnThreeVER(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 30 - 1;
            blockedCells[index + 1] = mainPosition + 30;
            blockedCells[index + 2] = mainPosition + 30 + 1;
            return index += 3;
        }
        public int FullForwardOnFourVER(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 40 - 1;
            blockedCells[index + 1] = mainPosition + 40;
            blockedCells[index + 2] = mainPosition + 40 + 1;
            return index += 3;
        }

        // "NotLeft":
        public int NotLeftBackOnOne(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10;
            blockedCells[index + 1] = mainPosition - 10 + 1;
            return index += 2;
        }
        public int NotLeftOnCurrent(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition;
            blockedCells[index + 1] = mainPosition + 1;
            return index += 2;
        }
        public int NotLeftForwardOnOne(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 10;
            blockedCells[index + 1] = mainPosition + 10 + 1;
            return index += 2;
        }
        public int NotLeftForwardOnTwo(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 20;
            blockedCells[index + 1] = mainPosition + 20 + 1;
            return index += 2;
        }
        public int NotLeftForwardOnThree(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 30;
            blockedCells[index + 1] = mainPosition + 30 + 1;
            return index += 2;
        }
        public int NotLeftForwardOnFour(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 40;
            blockedCells[index + 11] = mainPosition + 40 + 1;
            return index += 2;
        }

        // "NotRight":
        public int NotRightBackOnOne(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 10 - 1;
            blockedCells[index + 1] = mainPosition - 10;
            return index += 2;
        }
        public int NotRightOnCurrent(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition - 1;
            blockedCells[index + 1] = mainPosition;
            return index += 2;
        }
        public int NotRightForwardOnOne(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 10 - 1;
            blockedCells[index + 1] = mainPosition + 10;
            return index += 2;
        }
        public int NotRightForwardOnTwo(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 20 - 1;
            blockedCells[index + 1] = mainPosition + 20;
            return index += 2;
        }
        public int NotRightForwardOnThree(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 30 - 1;
            blockedCells[index + 1] = mainPosition + 30;
            return index += 2;
        }
        public int NotRightForwardOnFour(int[] blockedCells, ref int index, int mainPosition)
        {
            blockedCells[index] = mainPosition + 40 - 1;
            blockedCells[index + 11] = mainPosition + 40;
            return index += 2;
        }
        // The End of indexing methods...

        public int[] HitBox4Cell(int[] blockedCells)
        {
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            int mainPosition = increase + integerColumn - 1;

            switch (choosen_position)
            {
                case "H":
                    switch (choosen_line)
                    {
                        case "A":
                            if (choosen_column == "7" || choosen_column == "8" || choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 6;
                                NotAboveBackOnOneHOR(blockedCells, ref index, mainPosition);
                                NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                NotAboveForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                NotAboveForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnThreeHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnFourHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    NotAboveBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnThreeHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnFourHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        case "J":
                            if (choosen_column == "7" || choosen_column == "8" || choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 6 + increase;
                                NotUnderBackOnOneHOR(blockedCells, ref index, mainPosition);
                                NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                NotUnderForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                NotUnderForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnThreeHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnFourHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    NotUnderBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnThreeHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnFourHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        default:
                            if (choosen_column == "7" || choosen_column == "8" || choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 6 + increase;
                                FullBackOnOneHOR(blockedCells, ref index, mainPosition);
                                FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                FullForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                FullForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                FullForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnFourHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnFourHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                    }
                    break;
                case "V":
                    switch (choosen_line)
                    {
                        case "A":
                            switch (choosen_column)
                            {
                                case "1":
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnThree(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnFour(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition = 9;
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnThree(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnFour(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnFourVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        case "J":
                            mainPosition = 60 + integerColumn - 1;
                            switch (choosen_column)
                            {
                                case "1":
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnThree(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition = 9 + increase;
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnThree(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        default:
                            switch (choosen_column)
                            {
                                case "1":
                                    if (choosen_line == "G" || choosen_line == "H" || choosen_line == "I")
                                    {
                                        mainPosition = 60;
                                        NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                        NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                        NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                        NotLeftForwardOnTwo(blockedCells, ref index, mainPosition);
                                        NotLeftForwardOnThree(blockedCells, ref index, mainPosition); break;
                                    }
                                    NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnThree(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnFour(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    if (choosen_line == "G" || choosen_line == "H" || choosen_line == "I")
                                    {
                                        mainPosition = increase - 11;
                                        NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                        NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                        NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                        NotRightForwardOnTwo(blockedCells, ref index, mainPosition);
                                        NotRightForwardOnThree(blockedCells, ref index, mainPosition); break;
                                    }
                                    mainPosition += 10;
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnThree(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnFour(blockedCells, ref index, mainPosition); break;
                                default:
                                    if (choosen_line == "G" || choosen_line == "H" || choosen_line == "I")
                                    {
                                        if (choosen_line == "H") mainPosition -= 10;
                                        if (choosen_line == "I") mainPosition -= 20;

                                        FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                        FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                        FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                        FullForwardOnTwoVER(blockedCells, ref index, mainPosition);
                                        FullForwardOnThreeVER(blockedCells, ref index, mainPosition); break;
                                    }
                                    FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnFourVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                    }
                    break;
            }
            return blockedCells;
        }
        public int[] HitBox3Cell(int[] blockedCells)
        {
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            int mainPosition = increase + integerColumn - 1;

            switch (choosen_position)
            {
                case "H":
                    switch (choosen_line)
                    {
                        case "A":
                            if (choosen_column == "8" || choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 7;
                                NotAboveBackOnOneHOR(blockedCells, ref index, mainPosition);
                                NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                NotAboveForwardOnTwoHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    NotAboveBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        case "J":
                            if (choosen_column == "8" || choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 7 + increase;
                                NotUnderBackOnOneHOR(blockedCells, ref index, mainPosition);
                                NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                NotUnderForwardOnTwoHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    NotUnderBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        default:
                            if (choosen_column == "8" || choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 7 + increase;
                                FullBackOnOneHOR(blockedCells, ref index, mainPosition);
                                FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                FullForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                FullForwardOnTwoHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                    }
                    break;

                case "V":
                    switch (choosen_line)
                    {
                        case "A":
                            switch (choosen_column)
                            {
                                case "1":
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnThree(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition = 9;
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnThree(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        case "J":
                            mainPosition = 70 + integerColumn - 1;
                            switch (choosen_column)
                            {
                                case "1":
                                    NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnTwo(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition = 9 + increase;
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnTwo(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        default:
                            switch (choosen_column)
                            {
                                case "1":
                                    if (choosen_line == "H" || choosen_line == "I")
                                    {
                                        mainPosition = 70;
                                        NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                        NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                        NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                        NotLeftForwardOnTwo(blockedCells, ref index, mainPosition); break;
                                    }
                                    NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnThree(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    if (choosen_line == "H" || choosen_line == "I")
                                    {
                                        mainPosition = increase - 11;
                                        NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                        NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                        NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                        NotRightForwardOnTwo(blockedCells, ref index, mainPosition); break;
                                    }
                                    mainPosition += 10;
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnTwo(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnThree(blockedCells, ref index, mainPosition); break;
                                default:
                                    if (choosen_line == "H" || choosen_line == "I")
                                    {
                                        if (choosen_line == "I")
                                            mainPosition -= 10;

                                        FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                        FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                        FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                        FullForwardOnTwoVER(blockedCells, ref index, mainPosition); break;
                                    }
                                    FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnThreeVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                    }
                    break;
            }
            Unique(blockedCells);
            return blockedCells;
        }
        public int[] HitBox2Cell(int[] blockedCells)
        {
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            int mainPosition = increase + integerColumn - 1;

            switch (choosen_position)
            {
                case "H":
                    switch (choosen_line)
                    {
                        case "A":
                            if (choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 8;
                                NotAboveBackOnOneHOR(blockedCells, ref index, mainPosition);
                                NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnTwoHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    NotAboveBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnTwoHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        case "J":
                            if (choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 8 + increase;
                                NotUnderBackOnOneHOR(blockedCells, ref index, mainPosition);
                                NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnTwoHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    NotUnderBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnTwoHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        default:
                            if (choosen_column == "9" || choosen_column == "0")
                            {
                                mainPosition = 8 + increase;
                                FullBackOnOneHOR(blockedCells, ref index, mainPosition);
                                FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                FullForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                    }
                    break;

                case "V":
                    switch (choosen_line)
                    {
                        case "A":
                            switch (choosen_column)
                            {
                                case "1":
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnTwo(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition = 9;
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnTwo(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        case "J":
                            mainPosition = 80 + integerColumn - 1;
                            switch (choosen_column)
                            {
                                case "1":
                                    NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition = 9 + increase;
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        default:
                            switch (choosen_column)
                            {
                                case "1":
                                    if (choosen_line == "I")
                                    {
                                        mainPosition = 80;
                                        NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                        NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                        NotLeftForwardOnOne(blockedCells, ref index, mainPosition); break;
                                    }
                                    NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnTwo(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    if (choosen_line == "I")
                                    {
                                        mainPosition += 10;
                                        NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                        NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                        NotRightForwardOnOne(blockedCells, ref index, mainPosition); break;
                                    }
                                    mainPosition += 10;
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnTwo(blockedCells, ref index, mainPosition); break;
                                default:
                                    if (choosen_line == "I")
                                    {
                                        FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                        FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                        FullForwardOnOneVER(blockedCells, ref index, mainPosition); break;
                                    }
                                    FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnTwoVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                    }
                    break;
            }
            Unique(blockedCells);
            return blockedCells;
        }
        public int[] HitBox1Cell(int[] blockedCells)
        {
            int increase = ChooseLine(choosen_line);
            int integerColumn = int.Parse(choosen_column);
            int mainPosition = increase + integerColumn - 1;

            switch (choosen_position)
            {
                case "H":
                    switch (choosen_line)
                    {
                        case "A":
                            if (choosen_column == "0")
                            {
                                mainPosition = 9;
                                NotAboveBackOnOneHOR(blockedCells, ref index, mainPosition);
                                NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    NotAboveBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotAboveOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotAboveForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        case "J":
                            if (choosen_column == "0")
                            {
                                mainPosition = 9 + increase;
                                NotUnderBackOnOneHOR(blockedCells, ref index, mainPosition);
                                NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    NotUnderBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    NotUnderOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    NotUnderForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        default:
                            if (choosen_column == "0")
                            {
                                mainPosition = 9 + increase;
                                FullBackOnOneHOR(blockedCells, ref index, mainPosition);
                                FullOnCurrentHOR(blockedCells, ref index, mainPosition); break;
                            }
                            switch (choosen_column)
                            {
                                case "1":
                                    FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneHOR(blockedCells, ref index, mainPosition);
                                    FullOnCurrentHOR(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneHOR(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                    }
                    break;

                case "V":
                    switch (choosen_line)
                    {
                        case "A":
                            switch (choosen_column)
                            {
                                case "1":
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition = 9;
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        case "J":
                            switch (choosen_column)
                            {
                                case "1":
                                    NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition = 9 + increase;
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                        default:
                            switch (choosen_column)
                            {
                                case "1":
                                    NotLeftBackOnOne(blockedCells, ref index, mainPosition);
                                    NotLeftOnCurrent(blockedCells, ref index, mainPosition);
                                    NotLeftForwardOnOne(blockedCells, ref index, mainPosition); break;
                                case "0":
                                    mainPosition += 10;
                                    NotRightBackOnOne(blockedCells, ref index, mainPosition);
                                    NotRightOnCurrent(blockedCells, ref index, mainPosition);
                                    NotRightForwardOnOne(blockedCells, ref index, mainPosition); break;
                                default:
                                    FullBackOnOneVER(blockedCells, ref index, mainPosition);
                                    FullOnCurrentVER(blockedCells, ref index, mainPosition);
                                    FullForwardOnOneVER(blockedCells, ref index, mainPosition); break;
                            }
                            break;
                    }
                    break;
            }
            Unique(blockedCells);
            return blockedCells;
        }
        public void CheckForFreePlace(MainLogic logic, string[] cellsArray)
        {
            int helpToCheckVertical_1 = 10, helpToCheckVertical_2 = 20;
            int helpToCheckHorizon_1 = 1, helpToCheckHorizon_2 = 2;
            int integerColumn = int.Parse(choosen_column);
            if (integerColumn == 0)
                integerColumn = 10;
            int position = ChooseLine(choosen_line) + integerColumn - 1;
            bool conditionForCheck = false;

            for (int i = 0; i < logic.blocked_cells.Length; i++)
            {
                switch (what_ship_it_is)
                {
                    case "3V":
                        conditionForCheck =
                 logic.blocked_cells[i] == position ||
                 logic.blocked_cells[i] == position + helpToCheckVertical_1 ||
                 logic.blocked_cells[i] == position + helpToCheckVertical_2; break;
                    case "3H":
                        conditionForCheck =
                 logic.blocked_cells[i] == position ||
                 logic.blocked_cells[i] == position + helpToCheckHorizon_1 ||
                 logic.blocked_cells[i] == position + helpToCheckHorizon_2; break;
                    case "2V":
                        conditionForCheck =
                 logic.blocked_cells[i] == position ||
                 logic.blocked_cells[i] == position + helpToCheckVertical_1; break;
                    case "2H":
                        conditionForCheck =
                 logic.blocked_cells[i] == position ||
                 logic.blocked_cells[i] == position + helpToCheckHorizon_1; break;
                    default:
                        conditionForCheck = logic.blocked_cells[i] == position; break;
                }
                if (conditionForCheck)
                {
                    Console.Clear();
                    Console.Write("\n\n\n\t\tThat place is blocked!\n\t      Please choose another one.\n\n");
                    Console.WriteLine("\t      Press \"ENTER\" to continue: ");
                    Console.ReadLine();
                    Console.Clear();
                    logic.DisplayPlayerBattlefield(cellsArray);

                    switch (getKnowAboutShip)
                    {
                        case '3': Ship3CellManagement(logic, cellsArray); break;
                        case '2': Ship2CellManagement(logic, cellsArray); break;
                        case '1': Ship1CellManagement(logic, cellsArray); break;
                    }
                }
            }
        }
        // Next method is changes dublicates in "blockedCells" for avoiding "the multiple ship" bug:
        public int[] Unique(int[] blockedCells)
        {
            byte compare_with = 1;
            for (byte i = compare_with; i < blockedCells.Length; i++)
            {
                for (byte j = 0; j < compare_with; j++)
                {
                    // User input can't be with minus, so dublicates are become "-1" and code ignores that numbers
                    if (blockedCells[compare_with] == blockedCells[j])
                        blockedCells[compare_with] = -1;
                }
                compare_with++;
            }
            return blockedCells;
        }

        public int[] BotShipRandomPlacement(int[] botShipsPlaces)
        {
            Random random = new Random();
            int chooseVariant = random.Next(0, 6);

            switch (chooseVariant)
            {
                case 0:
                    /*[> > > >]*/
                    botShipsPlaces[0] = 1; botShipsPlaces[1] = 2; botShipsPlaces[2] = 3; botShipsPlaces[3] = 4;
                    /*[> > >]*/
                    botShipsPlaces[4] = 16; botShipsPlaces[5] = 26; botShipsPlaces[6] = 36;
                    botShipsPlaces[7] = 40; botShipsPlaces[8] = 41; botShipsPlaces[9] = 42;
                    /*[> >]*/
                    botShipsPlaces[10] = 10; botShipsPlaces[11] = 11;
                    botShipsPlaces[12] = 93; botShipsPlaces[13] = 94;
                    botShipsPlaces[14] = 77; botShipsPlaces[15] = 87;
                    /*[>]*/
                    botShipsPlaces[16] = 99;
                    botShipsPlaces[17] = 19;
                    botShipsPlaces[18] = 79;
                    botShipsPlaces[19] = 74;
                    break;
                case 1:
                    /*[> > > >]*/
                    botShipsPlaces[0] = 66; botShipsPlaces[1] = 67; botShipsPlaces[2] = 68; botShipsPlaces[3] = 69;
                    /*[> > >]*/
                    botShipsPlaces[4] = 51; botShipsPlaces[5] = 52; botShipsPlaces[6] = 53;
                    botShipsPlaces[7] = 17; botShipsPlaces[8] = 27; botShipsPlaces[9] = 37;
                    /*[> >]*/
                    botShipsPlaces[10] = 9; botShipsPlaces[11] = 19;
                    botShipsPlaces[12] = 39; botShipsPlaces[13] = 49;
                    botShipsPlaces[14] = 11; botShipsPlaces[15] = 12;
                    /*[>]*/
                    botShipsPlaces[16] = 81;
                    botShipsPlaces[17] = 84;
                    botShipsPlaces[18] = 7;
                    botShipsPlaces[19] = 97;
                    break;
                case 2:
                    /*[> > > >]*/
                    botShipsPlaces[0] = 41; botShipsPlaces[1] = 51; botShipsPlaces[2] = 61; botShipsPlaces[3] = 71;
                    /*[> > >]*/
                    botShipsPlaces[4] = 12; botShipsPlaces[5] = 13; botShipsPlaces[6] = 14;
                    botShipsPlaces[7] = 0; botShipsPlaces[8] = 10; botShipsPlaces[9] = 20;
                    /*[> >]*/
                    botShipsPlaces[10] = 91; botShipsPlaces[11] = 92;
                    botShipsPlaces[12] = 56; botShipsPlaces[13] = 66;
                    botShipsPlaces[14] = 58; botShipsPlaces[15] = 68;
                    /*[>]*/
                    botShipsPlaces[16] = 18;
                    botShipsPlaces[17] = 36;
                    botShipsPlaces[18] = 44;
                    botShipsPlaces[19] = 84;
                    break;
                case 3:
                    /*[> > > >]*/
                    botShipsPlaces[0] = 22; botShipsPlaces[1] = 32; botShipsPlaces[2] = 42; botShipsPlaces[3] = 52;
                    /*[> > >]*/
                    botShipsPlaces[4] = 16; botShipsPlaces[5] = 26; botShipsPlaces[6] = 36;
                    botShipsPlaces[7] = 66; botShipsPlaces[8] = 67; botShipsPlaces[9] = 68;
                    /*[> >]*/
                    botShipsPlaces[10] = 88; botShipsPlaces[11] = 98;
                    botShipsPlaces[12] = 85; botShipsPlaces[13] = 95;
                    botShipsPlaces[14] = 72; botShipsPlaces[15] = 82;
                    /*[>]*/
                    botShipsPlaces[16] = 11;
                    botShipsPlaces[17] = 40;
                    botShipsPlaces[18] = 70;
                    botShipsPlaces[19] = 28;
                    break;
                case 4:
                    /*[> > > >]*/
                    botShipsPlaces[0] = 93; botShipsPlaces[1] = 94; botShipsPlaces[2] = 95; botShipsPlaces[3] = 96;
                    /*[> > >]*/
                    botShipsPlaces[4] = 53; botShipsPlaces[5] = 63; botShipsPlaces[6] = 73;
                    botShipsPlaces[7] = 56; botShipsPlaces[8] = 66; botShipsPlaces[9] = 76;
                    /*[> >]*/
                    botShipsPlaces[10] = 21; botShipsPlaces[11] = 22;
                    botShipsPlaces[12] = 24; botShipsPlaces[13] = 25;
                    botShipsPlaces[14] = 27; botShipsPlaces[15] = 28;
                    /*[>]*/
                    botShipsPlaces[16] = 50;
                    botShipsPlaces[17] = 80;
                    botShipsPlaces[18] = 59;
                    botShipsPlaces[19] = 89;
                    break;
                case 5:
                    /*[> > > >]*/
                    botShipsPlaces[0] = 74; botShipsPlaces[1] = 75; botShipsPlaces[2] = 76; botShipsPlaces[3] = 77;
                    /*[> > >]*/
                    botShipsPlaces[4] = 61; botShipsPlaces[5] = 71; botShipsPlaces[6] = 81;
                    botShipsPlaces[7] = 25; botShipsPlaces[8] = 26; botShipsPlaces[9] = 27;
                    /*[> >]*/
                    botShipsPlaces[10] = 11; botShipsPlaces[11] = 12;
                    botShipsPlaces[12] = 31; botShipsPlaces[13] = 32;
                    botShipsPlaces[14] = 49; botShipsPlaces[15] = 59;
                    /*[>]*/
                    botShipsPlaces[16] = 9;
                    botShipsPlaces[17] = 99;
                    botShipsPlaces[18] = 79;
                    botShipsPlaces[19] = 45;
                    break;
            }
            return botShipsPlaces;
        }
        public int[] BotShootingOrder(int Nq, int Nm, int[] unique)
        {
            Random r = new Random();
            int p;
            int k = 0;
            if (Nq > Nm)
            {
                p = Nm;
                Nm = Nq;
                Nq = p;
            }
            while (k < Nq)
            {
                p = r.Next(Nm);
                bool next = true;
                for (int i = 0; i < k; i++)
                {
                    if (p == unique[i])
                    {
                        next = false;
                        break;
                    }
                }
                if (next)
                {
                    unique[k] = p;
                    k++;
                }
            }
            return unique;
        }

        public byte BotShooting(MainLogic logic, string[] playerCellsArray, string[] botCellsArray, int[] unique)
        {
            if (logic.is_somebody_win == true)
                return 0;
            if (logic.is_somebody_win == false)
            {
                DisplayEnemyBattlefield(botCellsArray);
                DisplayPlayerBattlefield(playerCellsArray);

                for (int i = bot_shoot_order; i < 100 & logic.is_somebody_win == false; i++)
                {
                    int shoot_place = unique[i];
                    if (playerCellsArray[shoot_place] == ">" || playerCellsArray[shoot_place] == "v")
                    {
                        Console.Clear();
                        playerCellsArray[shoot_place] = "X";
                        DisplayEnemyBattlefield(botCellsArray);
                        DisplayPlayerBattlefield(playerCellsArray);
                        System.Threading.Thread.Sleep(500);

                        bot_shoot_order++;
                        bot_hits_counter++;
                        // Condition for to lose the game:
                        if (bot_hits_counter == 20)
                        {
                            Console.Clear();
                            Console.WriteLine("\n\n\n\n\t\t      You're lose!");
                            Console.WriteLine("\t\tPress \"ENTER\" to continue:");
                            Console.ReadLine();
                            logic.is_somebody_win = true;
                            break;
                        }
                        Console.WriteLine("\t   Bot is hit you!");
                        Console.Write("\t   Bot's next hit in: ");
                        for (double j = 1.9; j > 0; j -= 0.1)
                        {
                            Console.SetCursorPosition(31, 15);
                            Console.WriteLine(Math.Round(j, 1));
                            System.Threading.Thread.Sleep(100);
                        }
                        BotShooting(logic, playerCellsArray, botCellsArray, unique);
                    }
                    playerCellsArray[shoot_place] = "o";
                    bot_shoot_order++;
                    PlayerShooting(logic, playerCellsArray, botCellsArray, unique);
                }
            }
            return 1;
        }
        public byte PlayerShooting(MainLogic logic, string[] playerCellsArray, string[] botCellsArray, int[] unique)
        {
            if (logic.is_somebody_win == true)
                return 0;
            Console.Clear();
            DisplayEnemyBattlefield(botCellsArray);
            DisplayPlayerBattlefield(playerCellsArray);
            AskAboutLine();
            AskAboutColumn();
            if (choosen_line != "A" & choosen_line != "B" & choosen_line != "C" & choosen_line != "D" & choosen_line != "E" &
                choosen_line != "F" & choosen_line != "G" & choosen_line != "H" & choosen_line != "I" & choosen_line != "J" ||
                choosen_column != "1" & choosen_column != "2" & choosen_column != "3" & choosen_column != "4" & choosen_column != "5" &
                choosen_column != "6" & choosen_column != "7" & choosen_column != "8" & choosen_column != "9" & choosen_column != "0")
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\t\t     Incorrect input!");
                Console.WriteLine("\t\tPlease check it next time!");
                Console.WriteLine("\t\tPress \"ENTER\" to continue:");
                Console.ReadLine();
                PlayerShooting(logic, playerCellsArray, botCellsArray, unique);
            }
                int columnIs = int.Parse(choosen_column);
            if (choosen_column == "0")
                columnIs = 10;
            int cellChoosenToShoot = ChooseLine(choosen_line) + columnIs - 1;
            bool hitten = false;

            for (byte i = 0; i < player_hit_order; i++)
            {
                if (cellChoosenToShoot == player_hitten_cells[i])
                {
                    Console.Clear();
                    Console.WriteLine("\n\n\n\t      You're choosen a hitten cell!");
                    Console.WriteLine("\t\t   Choose another one!");
                    Console.WriteLine("\t\tPress \"ENTER\" to continue:");
                    Console.ReadLine();
                    PlayerShooting(logic, playerCellsArray, botCellsArray, unique);
                }
            }
            logic.player_hitten_cells[player_hit_order] = cellChoosenToShoot;
            player_hit_order++;

            Console.ForegroundColor = ConsoleColor.White;
            for (byte i = 0; i < 20; i++)
            {
                if (bot_ship_places[i] == cellChoosenToShoot)
                {
                    hitten = true;
                    botCellsArray[cellChoosenToShoot] = "X";
                    Console.Clear();
                    DisplayEnemyBattlefield(botCellsArray);
                    DisplayPlayerBattlefield(playerCellsArray);

                    player_hits_counter++;
                    // Condition for to win the game:
                    if (player_hits_counter == 20)
                    {
                        Console.Clear();
                        Console.WriteLine("\n\n\n\n\t\t   You're win! Congrats!");
                        Console.WriteLine("\t\tPress \"ENTER\" to continue:");
                        Console.ReadLine();
                        logic.is_somebody_win = true;
                        break;
                    }
                    Console.WriteLine("\n\n\t   You are hit the enemy!");
                    Console.WriteLine("\t Choose next cell to shoot!");
                    Console.WriteLine("\t Press \"ENTER\" to continue: ");
                    Console.ReadLine();
                    Console.Clear();

                    DisplayPlayerBattlefield(playerCellsArray);
                    DisplayEnemyBattlefield(botCellsArray);
                    PlayerShooting(logic, playerCellsArray, botCellsArray, unique);
                }
            }
            if (hitten == false & logic.is_somebody_win == false)
            {
                botCellsArray[cellChoosenToShoot] = "o";

                Console.Write("\t   Enemy's hit in: ");
                for (double i = 1.9; i > 0; i -= 0.1)
                {
                    Console.SetCursorPosition(27, 16);
                    Console.WriteLine(Math.Round(i, 1));
                    System.Threading.Thread.Sleep(100);
                }
                Console.Clear();
                BotShooting(logic, playerCellsArray, botCellsArray, unique);
            }
            return 1;
        }
    }
    class GameplayLogic
    {
        static void Main(string[] args)
        {
            MainLogic logic = new MainLogic();
            {// Line A ===//===//===//===//===//===
                logic.A1 = logic.A2 = logic.A3 = logic.A4 = logic.A5
              = logic.A6 = logic.A7 = logic.A8 = logic.A9 = logic.A0
              // Line B ===//===//===//===//===//===
              = logic.B1 = logic.B2 = logic.B3 = logic.B4 = logic.B5
              = logic.B6 = logic.B7 = logic.B8 = logic.B9 = logic.B0
              // Line C ===//===//===//===//===//===
              = logic.C1 = logic.C2 = logic.C3 = logic.C4 = logic.C5
              = logic.C6 = logic.C7 = logic.C8 = logic.C9 = logic.C0
              // Line D ===//===//===//===//===//===
              = logic.D1 = logic.D2 = logic.D3 = logic.D4 = logic.D5
              = logic.D6 = logic.D7 = logic.D8 = logic.D9 = logic.D0
              // Line E ===//===//===//===//===//===
              = logic.E1 = logic.E2 = logic.E3 = logic.E4 = logic.E5
              = logic.E6 = logic.E7 = logic.E8 = logic.E9 = logic.E0
              // Line F ===//===//===//===//===//===
              = logic.F1 = logic.F2 = logic.F3 = logic.F4 = logic.F5
              = logic.F6 = logic.F7 = logic.F8 = logic.F9 = logic.F0
              // Line G ===//===//===//===//===//===
              = logic.G1 = logic.G2 = logic.G3 = logic.G4 = logic.G5
              = logic.G6 = logic.G7 = logic.G8 = logic.G9 = logic.G0
              // Line H ===//===//===//===//===//===
              = logic.H1 = logic.H2 = logic.H3 = logic.H4 = logic.H5
              = logic.H6 = logic.H7 = logic.H8 = logic.H9 = logic.H0
              // Line I ===//===//===//===//===//===
              = logic.I1 = logic.I2 = logic.I3 = logic.I4 = logic.I5
              = logic.I6 = logic.I7 = logic.I8 = logic.I9 = logic.I0
              // Line J ===//===//===//===//===//===
              = logic.J1 = logic.J2 = logic.J3 = logic.J4 = logic.J5
              = logic.J6 = logic.J7 = logic.J8 = logic.J9 = logic.J0 = ".";
            } // <-- Water is here! 

            string[] playerCellsArray =            
            // Cells of line A ===//===//===//===//===//===
           {logic.A1, logic.A2, logic.A3, logic.A4, logic.A5,
            logic.A6, logic.A7, logic.A8, logic.A9, logic.A0,
            // Cells of line B ===//===//===//===//===//===
            logic.B1, logic.B2, logic.B3, logic.B4, logic.B5,
            logic.B6, logic.B7, logic.B8, logic.B9, logic.B0,
            // Cells of line C ===//===//===//===//===//===
            logic.C1, logic.C2, logic.C3, logic.C4, logic.C5,
            logic.C6, logic.C7, logic.C8, logic.C9, logic.C0,
            // Cells of line D ===//===//===//===//===//===
            logic.D1, logic.D2, logic.D3, logic.D4, logic.D5,
            logic.D6, logic.D7, logic.D8, logic.D9, logic.D0,
            // Cells of line E ===//===//===//===//===//===
            logic.E1, logic.E2, logic.E3, logic.E4, logic.E5,
            logic.E6, logic.A7, logic.E8, logic.E9, logic.E0,
            // Cells of line F ===//===//===//===//===//===
            logic.F1, logic.F2, logic.F3, logic.F4, logic.F5,
            logic.F6, logic.F7, logic.F8, logic.F9, logic.F0,
            // Cells of line G ===//===//===//===//===//===
            logic.G1, logic.G2, logic.G3, logic.G4, logic.G5,
            logic.G6, logic.G7, logic.G8, logic.G9, logic.G0,
            // Cells of line H ===//===//===//===//===//===
            logic.H1, logic.H2, logic.H3, logic.H4, logic.H5,
            logic.H6, logic.H7, logic.H8, logic.H9, logic.H0,
            // Cells of line I ===//===//===//===//===//===
            logic.I1, logic.I2, logic.I3, logic.I4, logic.I5,
            logic.I6, logic.I7, logic.I8, logic.I9, logic.I0,
            // Cells of line J ===//===//===//===//===//===
            logic.J1, logic.J2, logic.J3, logic.J4, logic.J5,
            logic.J6, logic.J7, logic.J8, logic.J9, logic.J0};
            string[] botCellsArray =            
            // Cells of line A ===//===//===//===//===//===
           {logic.A1, logic.A2, logic.A3, logic.A4, logic.A5,
            logic.A6, logic.A7, logic.A8, logic.A9, logic.A0,
            // Cells of line B ===//===//===//===//===//===
            logic.B1, logic.B2, logic.B3, logic.B4, logic.B5,
            logic.B6, logic.B7, logic.B8, logic.B9, logic.B0,
            // Cells of line C ===//===//===//===//===//===
            logic.C1, logic.C2, logic.C3, logic.C4, logic.C5,
            logic.C6, logic.C7, logic.C8, logic.C9, logic.C0,
            // Cells of line D ===//===//===//===//===//===
            logic.D1, logic.D2, logic.D3, logic.D4, logic.D5,
            logic.D6, logic.D7, logic.D8, logic.D9, logic.D0,
            // Cells of line E ===//===//===//===//===//===
            logic.E1, logic.E2, logic.E3, logic.E4, logic.E5,
            logic.E6, logic.A7, logic.E8, logic.E9, logic.E0,
            // Cells of line F ===//===//===//===//===//===
            logic.F1, logic.F2, logic.F3, logic.F4, logic.F5,
            logic.F6, logic.F7, logic.F8, logic.F9, logic.F0,
            // Cells of line G ===//===//===//===//===//===
            logic.G1, logic.G2, logic.G3, logic.G4, logic.G5,
            logic.G6, logic.G7, logic.G8, logic.G9, logic.G0,
            // Cells of line H ===//===//===//===//===//===
            logic.H1, logic.H2, logic.H3, logic.H4, logic.H5,
            logic.H6, logic.H7, logic.H8, logic.H9, logic.H0,
            // Cells of line I ===//===//===//===//===//===
            logic.I1, logic.I2, logic.I3, logic.I4, logic.I5,
            logic.I6, logic.I7, logic.I8, logic.I9, logic.I0,
            // Cells of line J ===//===//===//===//===//===
            logic.J1, logic.J2, logic.J3, logic.J4, logic.J5,
            logic.J6, logic.J7, logic.J8, logic.J9, logic.J0};

            // "-1" is everywhere in array because default "0" is can to cause bugs with placement and shooting
            for (byte i = 0; i < 130; i++)
                logic.blocked_cells[i] = -1;
            for (byte i = 0; i < 20; i++)
                logic.bot_ship_places[i] = -1;
            for (byte i = 0; i < 100; i++)
                logic.player_hitten_cells[i] = -1;
            System.Threading.Thread.Sleep(1000);

            logic.DisplayPlayerBattlefield(playerCellsArray);

            logic.Ship4CellManagement(logic, playerCellsArray);
            logic.Ship3CellManagement(logic, playerCellsArray);
            logic.Ship2CellManagement(logic, playerCellsArray);
            logic.Ship1CellManagement(logic, playerCellsArray);

            logic.BotShipRandomPlacement(logic.bot_ship_places);
            logic.BotShootingOrder(logic.Nq, logic.Nm, logic.unique);

            logic.PlayerShooting(logic, playerCellsArray, botCellsArray, logic.unique);
        }
    }
}