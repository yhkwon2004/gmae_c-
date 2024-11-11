using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace from_game9
{
    public partial class Form1 : Form
    {
        public const int BoardWidth = 10;
        public const int BoardHeight = 20;
        private const int BlockSize = 30;
        private const int NextBlockWidth = 4; // 다음 블록의 너비
        private const int NextBlockHeight = 2; // 다음 블록의 높이

        private Timer gameTimer;
        private int[,] board;
        private Tetromino currentTetromino;
        private Tetromino[] nextTetrominos;
        private int score;

        // 중복된 테트로미노를 방지하기 위한 집합
        private HashSet<string> usedTetrominos;

        public Form1()
        {
            InitializeComponent();
            usedTetrominos = new HashSet<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            board = new int[BoardWidth, BoardHeight];
            nextTetrominos = new Tetromino[1]; // 다음 블록 3개
            gameTimer = new Timer();
            gameTimer.Interval = 500; // 0.5초마다 블록 떨어짐
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
            SpawnTetromino();
        }

        private void SpawnTetromino()
        {
            if (nextTetrominos[0] == null)
            {
                for (int i = 0; i < nextTetrominos.Length; i++)
                {
                    Tetromino newTetromino;
                    do
                    {
                        newTetromino = Tetromino.CreateRandomTetromino();
                    } while (usedTetrominos.Contains(newTetromino.GetHashCode().ToString()));

                    nextTetrominos[i] = newTetromino;
                    usedTetrominos.Add(newTetromino.GetHashCode().ToString());
                }
            }

            currentTetromino = nextTetrominos[nextTetrominos.Length - 1];

            for (int i = nextTetrominos.Length - 1; i > 0; i--)
            {
                nextTetrominos[i] = nextTetrominos[i - 1];
            }

            Tetromino nextTetrominoToAdd;
            do
            {
                nextTetrominoToAdd = Tetromino.CreateRandomTetromino();
            } while (usedTetrominos.Contains(nextTetrominoToAdd.GetHashCode().ToString()));
            nextTetrominos[0] = nextTetrominoToAdd;
            usedTetrominos.Add(nextTetrominoToAdd.GetHashCode().ToString());

            if (!IsValidPosition(currentTetromino))
            {
                gameTimer.Stop();
                MessageBox.Show("게임 오버");
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveDown();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBoard(e.Graphics);
            DrawTetromino(e.Graphics);
            DrawNextTetrominos(e.Graphics);
            DrawScore(e.Graphics);
            DrawBorder(e.Graphics);
        }

        private void DrawBorder(Graphics g)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, 0, 0, BoardWidth * BlockSize, BoardHeight * BlockSize);
            }
        }

        private void DrawBoard(Graphics g)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                for (int y = 0; y < BoardHeight; y++)
                {
                    if (board[x, y] != 0)
                    {
                        g.FillRectangle(Brushes.Blue, x * BlockSize, y * BlockSize, BlockSize, BlockSize);
                    }
                }
            }
        }

        private void DrawTetromino(Graphics g)
        {
            for (int y = 0; y < currentTetromino.Shape.GetLength(0); y++)
            {
                for (int x = 0; x < currentTetromino.Shape.GetLength(1); x++)
                {
                    if (currentTetromino.Shape[y, x] != 0)
                    {
                        g.FillRectangle(Brushes.Red,
                            (currentTetromino.X + x) * BlockSize,
                            (currentTetromino.Y + y) * BlockSize,
                            BlockSize, BlockSize);
                    }
                }
            }
        }

        private void DrawNextTetrominos(Graphics g)
        {
            using (Pen pen = new Pen(Color.Black, 2))
            {
                int nextBlockX = BoardWidth * BlockSize + 10;
                int nextBlockY = 100;

                // 전체 영역 테두리 그리기
                g.DrawRectangle(pen, nextBlockX - 2, nextBlockY - 2, NextBlockWidth * BlockSize + 4, NextBlockHeight * nextTetrominos.Length * BlockSize + 4);

                // 각 다음 블록을 그리기
                for (int i = 0; i < nextTetrominos.Length; i++)
                {
                    for (int y = 0; y < nextTetrominos[i].Shape.GetLength(0); y++)
                    {
                        for (int x = 0; x < nextTetrominos[i].Shape.GetLength(1); x++)
                        {
                            if (nextTetrominos[i].Shape[y, x] != 0)
                            {
                                g.FillRectangle(Brushes.Green,
                                    nextBlockX + x * BlockSize,
                                    nextBlockY + (nextTetrominos.Length - 1 - i) * (NextBlockHeight + 1) * BlockSize + y * BlockSize,
                                    BlockSize, BlockSize);
                            }
                        }
                    }
                }
            }
        }

        private void DrawScore(Graphics g)
        {
            g.DrawString($"점수: {score}", new Font("Arial", 16), Brushes.Black, new PointF(BoardWidth * BlockSize + 10, 10));
        }

        private void MoveDown()
        {
            if (IsValidPosition(currentTetromino, 0, 1))
            {
                currentTetromino.Y++;
            }
            else
            {
                MergeTetromino();
                ClearLines();
                SpawnTetromino();
            }
        }

        private bool IsValidPosition(Tetromino tetromino, int offsetX = 0, int offsetY = 0)
        {
            for (int y = 0; y < tetromino.Shape.GetLength(0); y++)
            {
                for (int x = 0; x < tetromino.Shape.GetLength(1); x++)
                {
                    if (tetromino.Shape[y, x] != 0)
                    {
                        int newX = tetromino.X + x + offsetX;
                        int newY = tetromino.Y + y + offsetY;

                        if (newX < 0 || newX >= BoardWidth || newY >= BoardHeight || (newY >= 0 && board[newX, newY] != 0))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void MergeTetromino()
        {
            for (int y = 0; y < currentTetromino.Shape.GetLength(0); y++)
            {
                for (int x = 0; x < currentTetromino.Shape.GetLength(1); x++)
                {
                    if (currentTetromino.Shape[y, x] != 0)
                    {
                        board[currentTetromino.X + x, currentTetromino.Y + y] = 1;
                    }
                }
            }
        }

        private void ClearLines()
        {
            for (int y = BoardHeight - 1; y >= 0; y--)
            {
                bool fullLine = true;
                for (int x = 0; x < BoardWidth; x++)
                {
                    if (board[x, y] == 0)
                    {
                        fullLine = false;
                        break;
                    }
                }

                if (fullLine)
                {
                    // 라인 삭제
                    for (int shiftY = y; shiftY > 0; shiftY--)
                    {
                        for (int x = 0; x < BoardWidth; x++)
                        {
                            board[x, shiftY] = board[x, shiftY - 1];
                        }
                    }
                    score += 100; // 점수 추가
                    y++; // 다시 검사
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    if (IsValidPosition(currentTetromino, -1, 0))
                    {
                        currentTetromino.X--;
                    }
                    break;
                case Keys.Right:
                    if (IsValidPosition(currentTetromino, 1, 0))
                    {
                        currentTetromino.X++;
                    }
                    break;
                case Keys.Down:
                    MoveDown();
                    break;
                case Keys.Up:
                    currentTetromino.Rotate();
                    if (!IsValidPosition(currentTetromino))
                    {
                        currentTetromino.Rotate(); // 원래 상태로 되돌림
                        currentTetromino.Rotate();
                        currentTetromino.Rotate();
                    }
                    break;
                case Keys.Space: // 스페이스바를 눌렀을 때
                    while (IsValidPosition(currentTetromino, 0, 1))
                    {
                        currentTetromino.Y++;
                    }
                    MoveDown(); // 블록이 바닥에 닿았을 때 처리
                    break;
            }
            Invalidate(); // 폼을 다시 그리기
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    public class Tetromino
    {
        public int[,] Shape { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }

        private static readonly int[][,] TetrominoShapes = new int[][,]
        {
            new int[,] { { 1, 1, 1, 1 } }, // I
            new int[,] { { 1, 1, 1 }, { 0, 1, 0 } }, // T
            new int[,] { { 1, 1 }, { 1, 1 } }, // O
            new int[,] { { 0, 1, 1 }, { 1, 1, 0 } }, // S
            new int[,] { { 1, 1, 0 }, { 0, 1, 1 } }, // Z
            new int[,] { { 1, 0, 0 }, { 1, 1, 1 } }, // L
            new int[,] { { 0, 0, 1 }, { 1, 1, 1 } }  // J
        };

        public static Tetromino CreateRandomTetromino()
        {
            Random random = new Random();
            int index = random.Next(TetrominoShapes.Length);
            return new Tetromino
            {
                Shape = TetrominoShapes[index],
                X = Form1.BoardWidth / 2 - TetrominoShapes[index].GetLength(1) / 2,
                Y = 0
            };
        }

        public void Rotate()
        {
            int width = Shape.GetLength(1);
            int height = Shape.GetLength(0);
            int[,] newShape = new int[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newShape[x, height - 1 - y] = Shape[y, x];
                }
            }
            Shape = newShape;
        }
    }
}
