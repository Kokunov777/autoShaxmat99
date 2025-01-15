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

        private string[,] board; // Состояние шахматной доски
        private string currentPlayer; // Текущий игрок

        public Form1()
        {
            InitializeComponent();

            try
            {
                // Инициализация доски и игрока
                board = InitializeBoard();
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
                    Height = 600,
                    Width = 600,
                };
                this.Controls.Add(dataGridView1);

                // Создание метки текущего игрока
                currentPlayerLabel = new Label
                {
                    Text = "Ход: Белый",
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Height = 40,
                    Font = new Font("Arial", 16, FontStyle.Bold)
                };
                this.Controls.Add(currentPlayerLabel);

                // Создание метки с инструкциями по вводу хода
                moveInstructionsLabel = new Label
                {
                    Text = "Введите ход (например, e2 e4):",
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Height = 40,
                    Font = new Font("Arial", 16, FontStyle.Bold)
                };
                this.Controls.Add(moveInstructionsLabel);

                // Создание текстового поля для ввода хода
                moveTextBox = new TextBox
                {
                    Dock = DockStyle.Top,
                    Font = new Font("Arial", 16, FontStyle.Bold)
                };
                this.Controls.Add(moveTextBox);

                // Создание кнопки для выполнения хода
                makeMoveButton = new Button
                {
                    Text = "Сделать ход",
                    Dock = DockStyle.Top,
                    Height = 50,
                    Font = new Font("Arial", 16, FontStyle.Bold)
                };
                makeMoveButton.Click += MakeMoveButton_Click;
                this.Controls.Add(makeMoveButton);
                this.KeyPreview = true;

                this.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter) // Связываем с клавишей Ентер
                    {
                        makeMoveButton.PerformClick(); // Нажатие
                    }
                };

                // Настраиваем шахматную доску
                InitializeChessBoard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании UI: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string[,] InitializeBoard()
        {
            string[,] initialBoard = new string[8, 8]
            {
                { "♜", "♞", "♝", "♛", "♚", "♝", "♞", "♜" },
                { "♟", "♟", "♟", "♟", "♟", "♟", "♟", "♟" },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { " ", " ", " ", " ", " ", " ", " ", " " },
                { "♙", "♙", "♙", "♙", "♙", "♙", "♙", "♙" },
                { "♖", "♘", "♗", "♕", "♔", "♗", "♘", "♖" }
            };
            return initialBoard;
        }

        private void InitializeChessBoard()
        {
            try
            {
                dataGridView1.ColumnCount = 8;
                dataGridView1.RowCount = 8;

                for (int i = 0; i < 8; i++)
                {
                    dataGridView1.Rows[i].HeaderCell.Value = (8 - i).ToString();
                    dataGridView1.Columns[i].HeaderText = ((char)('a' + i)).ToString();
                    dataGridView1.Columns[i].Width = 70;
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    row.Height = 70;
                }

                dataGridView1.DefaultCellStyle.Font = new Font("Arial", 30, FontStyle.Bold);

                UpdateBoardDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при настройке шахматной доски: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateBoardDisplay()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = board[i, j];
                }
            }
        }

        private void MakeMoveButton_Click(object sender, EventArgs e)
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

            string piece = board[start.Item1, start.Item2];
            if (string.IsNullOrEmpty(piece))
            {
                MessageBox.Show("В начальной позиции нет фигуры.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsMoveValid(piece, start, target))
            {
                board[target.Item1, target.Item2] = piece;
                board[start.Item1, start.Item2] = " ";
                UpdateBoardDisplay();

                currentPlayer = currentPlayer == "Белый" ? "Черный" : "Белый";
                currentPlayerLabel.Text = $"Ход: {currentPlayer}";
            }
            else
            {
                MessageBox.Show("Неверный ход. Попробуйте снова.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private (int, int) ParsePosition(string position)
        {
            if (position.Length != 2) return (-1, -1);

            int x = position[0] - 'a';
            int y = 8 - (position[1] - '0');

            return (y, x);
        }

        private bool IsPositionValid((int, int) position)
        {
            return position.Item1 >= 0 && position.Item1 < 8 && position.Item2 >= 0 && position.Item2 < 8;
        }

        public enum PieceType
        {
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King
        }

        public static PieceType GetPieceType(string piece)
        {
            switch (piece)
            {
                case "♙":
                case "♟":
                    return PieceType.Pawn;
                case "♘":
                case "♞":
                    return PieceType.Knight;
                case "♗":
                case "♝":
                    return PieceType.Bishop;
                case "♖":
                case "♜":
                    return PieceType.Rook;
                case "♕":
                case "♛":
                    return PieceType.Queen;
                case "♔":
                case "♚":
                    return PieceType.King;
                default:
                    throw new ArgumentException("Неизвестная фигура");

            }
        }

        private bool IsMoveValid(string piece, (int, int) start, (int, int) target)
        {
            try
            {
                PieceType pieceType = GetPieceType(piece);

                switch (pieceType)
                {
                    case PieceType.Pawn:
                        return IsPawnMoveValid(piece, start, target);
                    case PieceType.Knight:
                        return IsKnightMoveValid(start, target);
                    case PieceType.Bishop:
                        return IsBishopMoveValid(start, target);
                    case PieceType.Rook:
                        return IsRookMoveValid(start, target);
                    case PieceType.Queen:
                        return IsQueenMoveValid(start, target);
                    case PieceType.King:
                        return IsKingMoveValid(start, target);
                    default:
                        return false;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }

        }
        public event Action OnBoardChanged;


        public void MovePiece((int x, int y) start, (int x, int y) target)
        {
            string piece = board[start.x, start.y];
            if (piece == " ")
            {
                return;
            }

            if (IsMoveValid(piece, start, target)) // Проверяем ход
            {
                board[target.x, target.y] = piece;
                board[start.x, start.y] = " ";
                OnBoardChanged?.Invoke();
            }
        }


        private bool IsPawnMoveValid(string piece, (int x, int y) start, (int x, int y) target)
        {
            int direction = piece == "♙" ? -1 : 1;
            int startRow = piece == "♙" ? 6 : 1;

            // Простое движение на одну клетку вперед
            if (start.y == target.y && target.x - start.x == direction && board[target.x, target.y] == " ")
            {
                return true;
            }

            // Двойной шаг для пешки с начальной строки
            if (start.y == target.y && start.x == startRow && target.x - start.x == 2 * direction && board[target.x, target.y] == " " && board[start.x + direction, start.y] == " ")
            {
                return true;
            }

            // Захват по диагонали
            if (Math.Abs(start.y - target.y) == 1 && target.x - start.x == direction && board[target.x, target.y] != " " && !IsSameColorPiece(board[start.x, start.y], board[target.x, target.y]))
            {
                return true;
            }

            return false;
        }

        private bool IsKnightMoveValid((int x, int y) start, (int x, int y) target)
        {
            int dx = Math.Abs(start.y - target.y);
            int dy = Math.Abs(start.x - target.x);

            return (dx == 2 && dy == 1) || (dx == 1 && dy == 2);
        }

        private bool IsBishopMoveValid((int x, int y) start, (int x, int y) target)
        {
            int dx = Math.Abs(start.y - target.y);
            int dy = Math.Abs(start.x - target.x);

            if (dx != dy)
                return false; // Слон должен двигаться по диагонали

            // Проверка на наличие фигур на пути
            int xStep = start.y < target.y ? 1 : -1;
            int yStep = start.x < target.x ? 1 : -1;

            int x = start.y + xStep;
            int y = start.x + yStep;
            while (x != target.y && y != target.x)
            {
                if (board[y, x] != " ")
                    return false;
                x += xStep;
                y += yStep;
            }
            // проверка 
            if (board[target.x, target.y] != " " && IsSameColorPiece(board[start.x, start.y], board[target.x, target.y]))
            {
                return false;
            }

            return true;
        }

        private bool IsRookMoveValid((int x, int y) start, (int x, int y) target)
        {
            if (start.x != target.x && start.y != target.y)
                return false; // Ладья должна двигаться по прямой линии

            // Проверка на наличие фигур на пути
            int xStep = start.y == target.y ? 0 : (start.y < target.y ? 1 : -1);
            int yStep = start.x == target.x ? 0 : (start.x < target.x ? 1 : -1);

            int x = start.y + xStep;
            int y = start.x + yStep;
            while (x != target.y || y != target.x)
            {
                if (board[y, x] != " ")
                    return false;
                x += xStep;
                y += yStep;
            }
            //проверка на конечную клетку
            if (board[target.x, target.y] != " " && IsSameColorPiece(board[start.x, start.y], board[target.x, target.y]))
            {
                return false;
            }

            return true;
        }

        private bool IsQueenMoveValid((int x, int y) start, (int x, int y) target)
        {
            if (IsRookMoveValid(start, target))
            {
                if (board[target.x, target.y] != " " && IsSameColorPiece(board[start.x, start.y], board[target.x, target.y]))
                {
                    return false;
                }
                return true;
            }
            if (IsBishopMoveValid(start, target))
            {
                if (board[target.x, target.y] != " " && IsSameColorPiece(board[start.x, start.y], board[target.x, target.y]))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool IsKingMoveValid((int x, int y) start, (int x, int y) target)
        {
            int dx = Math.Abs(start.y - target.y);
            int dy = Math.Abs(start.x - target.x);

            if (dx > 1 || dy > 1)
            {
                return false; 
            }

            // Проверяем, не занята ли конечная клетка фигурой того же цвета
            if (board[target.x, target.y] != " " && IsSameColorPiece(board[start.x, start.y], board[target.x, target.y]))
            {
                return false; 
            }

            return true; 
        }


        // Проверка на одинаковый цвет фигур
        private bool IsSameColorPiece(string piece1, string piece2)
        {
            if (piece1 == " " || piece2 == " ")
                return false;
            return (piece1 == piece1.ToUpper() && piece2 == piece2.ToUpper()) || (piece1 == piece1.ToLower() && piece2 == piece2.ToLower());
        }

    }
}
