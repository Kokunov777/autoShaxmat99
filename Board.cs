using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autoShaxmat99
{
        public class Board
        {
            public string[,] Grid { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public Board(int rows, int columns)
            {
                Width = columns;
                Height = rows;
                Grid = new string[rows, columns];
                InitializeBoard();
            }

            private void InitializeBoard()
            {
                // Начальная расстановка фигур
                Grid[0, 0] = "♖"; Grid[0, 1] = "♘"; Grid[0, 2] = "♗"; Grid[0, 3] = "♕"; Grid[0, 4] = "♔"; Grid[0, 5] = "♗"; Grid[0, 6] = "♘"; Grid[0, 7] = "♖"; // Белые
                Grid[1, 0] = "♙"; Grid[1, 1] = "♙"; Grid[1, 2] = "♙"; Grid[1, 3] = "♙"; Grid[1, 4] = "♙"; Grid[1, 5] = "♙"; Grid[1, 6] = "♙"; Grid[1, 7] = "♙";

                Grid[7, 0] = "♜"; Grid[7, 1] = "♞"; Grid[7, 2] = "♝"; Grid[7, 3] = "♛"; Grid[7, 4] = "♚"; Grid[7, 5] = "♝"; Grid[7, 6] = "♞"; Grid[7, 7] = "♜";
                Grid[6, 0] = "♟"; Grid[6, 1] = "♟"; Grid[6, 2] = "♟"; Grid[6, 3] = "♟"; Grid[6, 4] = "♟"; Grid[6, 5] = "♟"; Grid[6, 6] = "♟"; Grid[6, 7] = "♟";
            }

            public bool IsMoveValid(string piece, int startX, int startY, int targetX, int targetY)
            {
                return true; // Простая проверка: здесь можно добавить логику ходов
            }
        }
    }
