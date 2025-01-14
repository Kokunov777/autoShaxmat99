using autoShaxmat99;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private DataGridView dataGridView1;
        private TextBox moveTextBox;
        private Button makeMoveButton;
        private Label currentPlayerLabel;
        private Label moveInstructionsLabel;

        private Board board; // Логика шахматной доски
        private string currentPlayer; // Текущий игрок

        public Form1()
        {
            InitializeComponent();

            try
            {
                // Инициализация доски и игрока
                board = new Board(8, 8);
                currentPlayer = "Белый";

                // Создаем элементы интерфейса
                InitializeUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeUI()
        {
            try
            {
                // Создание DataGridView (шахматная доска)
                dataGridView1 = new DataGridView
                {
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToResizeColumns = false,
                    AllowUserToResizeRows = false,
                    RowHeadersWidth = 50,
                    ColumnHeadersHeight = 30,
                    Dock = DockStyle.Top,
                    Height = 600, // Задаем высоту шахматной доски
                    Width = 600,
                };
                this.Controls.Add(dataGridView1);

                // Создание метки текущего игрока
                currentPlayerLabel = new Label
                {
                    Text = "Ход: Белый",
                    Dock = DockStyle.Top,
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    Height = 40
                };
                this.Controls.Add(currentPlayerLabel);

                // Создание метки с инструкциями по вводу хода
                moveInstructionsLabel = new Label
                {
                    Text = "Введите ход (например, e2 e4):",
                    Dock = DockStyle.Top,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    Height = 40
                };
                this.Controls.Add(moveInstructionsLabel);

                // Создание текстового поля для ввода хода
                moveTextBox = new TextBox
                {
                    //  PlaceholderText = "Введите ход (например, e2 e4)",
                    Dock = DockStyle.Top
                };
                this.Controls.Add(moveTextBox);

                // Создание кнопки для выполнения хода
                makeMoveButton = new Button
                {
                    Text = "Сделать ход",
                    Dock = DockStyle.Top,
                    Height = 50,
                };
                makeMoveButton.Click += MakeMoveButton_Click;
                this.Controls.Add(makeMoveButton);

                // Настраиваем шахматную доску
                InitializeChessBoard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании UI: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeChessBoard()
        {
            try
            {
                // Устанавливаем размер DataGridView в зависимости от размеров доски
                dataGridView1.ColumnCount = board.Width;
                dataGridView1.RowCount = board.Height;

                // Настроим заголовки строк и столбцов
                for (int i = 0; i < board.Height; i++)
                {
                    // Меняем индексацию строк с 1 до 8 на 8 до 1
                    dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
                }

                for (int j = 0; j < board.Width; j++)
                {
                    dataGridView1.Columns[j].HeaderText = ((char)('a' + j)).ToString();
                    dataGridView1.Columns[j].Width = 70; // Фиксированная ширина столбцов
                }

                // Увеличиваем высоту строк для отображения более крупных фигур
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = 70; // Увеличиваем высоту строк
                }

                // Настроим шрифт для отображения фигур
                dataGridView1.DefaultCellStyle.Font = new Font("Arial", 30, FontStyle.Bold); // Увеличиваем размер шрифта


                // Отображаем начальное состояние доски
                UpdateBoardDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при настройке шахматной доски: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateBoardDisplay()
        {
            try
            {
                // Обновляем содержимое DataGridView с перевернутой доской
                for (int i = 0; i < board.Height; i++)
                {
                    for (int j = 0; j < board.Width; j++)
                    {
                        // Переворачиваем строки, меняем индексы i и j
                        dataGridView1.Rows[i].Cells[j].Value = board.Grid[board.Height - 1 - i, j];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления доски: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MakeMoveButton_Click(object sender, EventArgs e)
        {
            try
            {
                string move = moveTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(move))
                {
                    MessageBox.Show("Введите ход, например, 'e2 e4'.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var parts = move.Split(' ');
                if (parts.Length != 2)
                {
                    MessageBox.Show("Неверный формат. Используйте формат 'e2 e4'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var start = ParsePosition(parts[0]);
                var target = ParsePosition(parts[1]);
                if (!IsPositionValid(start) || !IsPositionValid(target))
                {
                    MessageBox.Show("Неверная позиция. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string piece = board.Grid[start.Item1, start.Item2];
                if (string.IsNullOrEmpty(piece) || IsOpponentPiece(piece))
                {
                    MessageBox.Show("Неверный ход: в начальной позиции нет фигуры или это фигура противника.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (board.IsMoveValid(piece, start.Item1, start.Item2, target.Item1, target.Item2))
                {
                    // Перемещаем фигуру
                    board.Grid[target.Item1, target.Item2] = piece;
                    board.Grid[start.Item1, start.Item2] = " ";

                    // Обновляем отображение доски
                    UpdateBoardDisplay();

                    // Переключаем игрока
                    currentPlayer = currentPlayer == "Белый" ? "Черный" : "Белый";
                    currentPlayerLabel.Text = $"Ход: {currentPlayer}";
                }
                else
                {
                    MessageBox.Show("Неверный ход. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения хода: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private (int, int) ParsePosition(string position)
        {
            if (position.Length != 2) return (-1, -1);

            int x = position[0] - 'a';
            if (!int.TryParse(position[1].ToString(), out int y)) return (-1, -1);

            // Переворачиваем строку
            return (board.Height - y, x);
        }

        private bool IsPositionValid((int, int) position)
        {
            return position.Item1 >= 0 && position.Item1 < board.Height && position.Item2 >= 0 && position.Item2 < board.Width;
        }

        private bool IsOpponentPiece(string piece)
        {
            return (currentPlayer == "Белый" && piece == "♟") || (currentPlayer == "Черный" && piece == "♙");
        }

        public class ChessLogic
        {
            // Проверка на допустимость хода для конкретной фигуры
            public bool IsMoveValid(string piece, int startX, int startY, int targetX, int targetY, string[,] board)
            {
                // Проверка границ доски
                if (!IsWithinBounds(targetX, targetY))
                    return false;

                switch (piece)
                {
                    case "♙": // Белая пешка
                        return IsWhitePawnMoveValid(startX, startY, targetX, targetY, board);
                    case "♟": // Черная пешка
                        return IsBlackPawnMoveValid(startX, startY, targetX, targetY, board);
                    case "♖": // Ладья
                        return IsRookMoveValid(startX, startY, targetX, targetY, board);
                    case "♘": // Конь
                        return IsKnightMoveValid(startX, startY, targetX, targetY);
                    case "♗": // Слон
                        return IsBishopMoveValid(startX, startY, targetX, targetY, board);
                    case "♕": // Ферзь
                        return IsQueenMoveValid(startX, startY, targetX, targetY, board);
                    case "♔": // Король
                        return IsKingMoveValid(startX, startY, targetX, targetY);
                    default:
                        return false;
                }
            }

            // Проверка на нахождение в пределах доски
            private bool IsWithinBounds(int x, int y)
            {
                return x >= 0 && x < 8 && y >= 0 && y < 8;
            }

            // Проверка для белой пешки
            private bool IsWhitePawnMoveValid(int startX, int startY, int targetX, int targetY, string[,] board)
            {
                // Пешка может двигаться на одну клетку вперед
                if (startX == targetX - 1 && startY == targetY && board[targetX, targetY] == " ")
                    return true;

                // Пешка может двигаться на две клетки вперед с начальной позиции
                if (startX == 1 && targetX == 3 && startY == targetY && board[2, targetY] == " " && board[targetX, targetY] == " ")
                    return true;

                // Пешка может бить противника по диагонали
                if (startX == targetX - 1 && Math.Abs(startY - targetY) == 1 && board[targetX, targetY] != " " && !IsSameColorPiece(board[startX, startY], board[targetX, targetY]))
                    return true;

                return false;
            }

            // Проверка для черной пешки
            private bool IsBlackPawnMoveValid(int startX, int startY, int targetX, int targetY, string[,] board)
            {
                // Черная пешка движется в обратном направлении
                if (startX == targetX + 1 && startY == targetY && board[targetX, targetY] == " ")
                    return true;

                // Черная пешка может двигаться на две клетки вперед с начальной позиции
                if (startX == 6 && targetX == 4 && startY == targetY && board[5, targetY] == " " && board[targetX, targetY] == " ")
                    return true;

                // Черная пешка может бить противника по диагонали
                if (startX == targetX + 1 && Math.Abs(startY - targetY) == 1 && board[targetX, targetY] != " " && !IsSameColorPiece(board[startX, startY], board[targetX, targetY]))
                    return true;

                return false;
            }

            // Проверка для ладьи
            private bool IsRookMoveValid(int startX, int startY, int targetX, int targetY, string[,] board)
            {
                if (startX == targetX) // Горизонтальное движение
                {
                    int min = Math.Min(startY, targetY);
                    int max = Math.Max(startY, targetY);
                    for (int i = min + 1; i < max; i++)
                    {
                        if (board[startX, i] != " ")
                            return false;
                    }
                    return true;
                }
                if (startY == targetY) // Вертикальное движение
                {
                    int min = Math.Min(startX, targetX);
                    int max = Math.Max(startX, targetX);
                    for (int i = min + 1; i < max; i++)
                    {
                        if (board[i, startY] != " ")
                            return false;
                    }
                    return true;
                }
                return false;
            }

            // Проверка для коня
            private bool IsKnightMoveValid(int startX, int startY, int targetX, int targetY)
            {
                // Конь двигается "буквой Г"
                return (Math.Abs(startX - targetX) == 2 && Math.Abs(startY - targetY) == 1) || (Math.Abs(startX - targetX) == 1 && Math.Abs(startY - targetY) == 2);
            }

            // Проверка для слона
            private bool IsBishopMoveValid(int startX, int startY, int targetX, int targetY, string[,] board)
            {
                if (Math.Abs(startX - targetX) != Math.Abs(startY - targetY))
                    return false;

                int xDirection = targetX > startX ? 1 : -1;
                int yDirection = targetY > startY ? 1 : -1;
                int x = startX + xDirection, y = startY + yDirection;

                while (x != targetX && y != targetY)
                {
                    if (board[x, y] != " ")
                        return false;
                    x += xDirection;
                    y += yDirection;
                }
                return true;
            }

            // Проверка для ферзя
            private bool IsQueenMoveValid(int startX, int startY, int targetX, int targetY, string[,] board)
            {
                return IsRookMoveValid(startX, startY, targetX, targetY, board) || IsBishopMoveValid(startX, startY, targetX, targetY, board);
            }

            // Проверка для короля
            private bool IsKingMoveValid(int startX, int startY, int targetX, int targetY)
            {
                return Math.Abs(startX - targetX) <= 1 && Math.Abs(startY - targetY) <= 1;
            }

            // Проверка на одинаковый цвет фигур
            private bool IsSameColorPiece(string piece1, string piece2)
            {
                if (piece1 == " " || piece2 == " ")
                    return false;
                return char.IsUpper(piece1[0]) == char.IsUpper(piece2[0]); // Проверка на одинаковый цвет
            }
        }

    } 
}
