using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace from_game6
{
    public partial class Form1 : Form
    {
        private Button[,] buttons;
        private int[,] mineField;
        private int gridSize = 10; // 10x10 그리드
        private int mineCount = 10; // 지뢰 개수
        private int buttonSize = 40; // 버튼 크기
        private bool gameOver = false;
        private int score = 0; // 점수
        private const string scoreFilePath = "score.txt"; // 점수를 저장할 파일 경로

        public Form1()
        {
            InitializeComponent();
            LoadScore(); // 게임 시작 시 점수 불러오기
            InitializeGame();
        }

        private void InitializeGame()
        {
            buttons = new Button[gridSize, gridSize];
            mineField = new int[gridSize, gridSize];

            // 지뢰 배치
            PlaceMines();

            // 버튼 생성
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(buttonSize, buttonSize);
                    btn.Location = new Point(j * buttonSize, i * buttonSize);
                    btn.BackColor = Color.Black;
                    btn.ForeColor = Color.White;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.Tag = new Point(i, j);
                    btn.Click += new EventHandler(Button_Click);
                    btn.MouseDown += new MouseEventHandler(Button_MouseDown); // 우클릭 이벤트 추가
                    buttons[i, j] = btn;
                    panel1.Controls.Add(btn);
                }
            }
            panel1.Size = new Size(gridSize * buttonSize, gridSize * buttonSize);
        }

        private void PlaceMines()
        {
            Random random = new Random();
            int placedMines = 0;

            while (placedMines < mineCount)
            {
                int x = random.Next(gridSize);
                int y = random.Next(gridSize);
                if (mineField[x, y] != -1) // -1은 지뢰를 의미
                {
                    mineField[x, y] = -1;
                    placedMines++;

                    // 주변 지뢰 수 증가
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (x + i >= 0 && x + i < gridSize && y + j >= 0 && y + j < gridSize && mineField[x + i, y + j] != -1)
                            {
                                mineField[x + i, y + j]++;
                            }
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (gameOver) return;

            Button btn = sender as Button;
            Point pos = (Point)btn.Tag;
            int x = pos.X;
            int y = pos.Y;

            if (mineField[x, y] == -1) // 지뢰 클릭 시
            {
                btn.BackColor = Color.Red;
                MessageBox.Show($"게임 오버! 지뢰를 클릭했습니다. 점수: {score}");
                SaveScore(); // 게임 오버 시 점수 저장
                gameOver = true;
                RevealMines();
            }
            else
            {
                OpenCell(x, y);
                score++; // 안전한 칸 클릭 시 점수 증가
                if (GameCheck())
                {
                    MessageBox.Show($"성공! 모든 안전한 칸을 열었습니다. 점수: {score}");
                    SaveScore(); // 성공 시 점수 저장
                }
            }
        }

        private void OpenCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= gridSize || y >= gridSize) return;
            if (buttons[x, y].Enabled == false) return; // 이미 열린 버튼

            buttons[x, y].Enabled = false; // 버튼 비활성화
            buttons[x, y].BackColor = Color.White;

            if (mineField[x, y] > 0)
            {
                buttons[x, y].Text = mineField[x, y].ToString();
            }
            else
            {
                // 주변 셀 열기
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        OpenCell(x + i, y + j);
                    }
                }
            }
        }

        private void RevealMines()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (mineField[i, j] == -1)
                    {
                        buttons[i, j].BackColor = Color.Red; // 지뢰 표시
                    }
                }
            }
        }

        private bool GameCheck()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (mineField[i, j] != -1 && buttons[i, j].Enabled) // 지뢰가 아닌 버튼이 열리지 않았으면
                    {
                        return false;
                    }
                }
            }
            return true; // 모든 안전한 버튼이 열렸음
        }

        private void SaveScore()
        {
            File.WriteAllText(scoreFilePath, score.ToString());
        }

        private void LoadScore()
        {
            if (File.Exists(scoreFilePath))
            {
                string scoreText = File.ReadAllText(scoreFilePath);
                if (int.TryParse(scoreText, out int savedScore))
                {
                    score = savedScore; // 파일에서 점수를 불러오기
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // 패널에 그리기 기능 추가 시 사용할 수 있습니다.
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (gameOver) return;

            Button btn = sender as Button;
            Point pos = (Point)btn.Tag;
            int x = pos.X;
            int y = pos.Y;

            if (e.Button == MouseButtons.Right) // 우클릭 시
            {
                if (btn.Text == "🚩") // 이미 깃발이 있으면 제거
                {
                    btn.Text = ""; // 깃발 제거
                }
                else
                {
                    btn.Text = "🚩"; // 깃발 표시
                }
            }
        }

        // 점수를 반환하는 메서드
        public int GetScore()
        {
            return score;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
