using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace from_game5
{
    public partial class Form1 : Form
    {
        private const int gridSize = 21; // 그리드 크기 21x21
        private const int tileSize = 20; // 타일 크기
        private List<Point> snake; // 뱀의 몸
        private Point food; // 먹이 위치
        private Direction direction; // 뱀의 방향
        private Timer timer; // 게임 타이머
        private Random random; // 랜덤 생성기
        private int score; // 점수
        private bool gameOver; // 게임 종료 상태
        private const string scoreFilePath = "score.txt"; // 점수를 저장할 파일 경로

        private enum Direction { Up, Down, Left, Right } // 방향 열거형

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            this.DoubleBuffered = true; // 더블 버퍼링 활성화
            random = new Random();
            snake = new List<Point> { new Point(5, 5) }; // 초기 뱀 위치
            direction = Direction.Right; // 초기 방향
            score = 0;
            gameOver = false;

            timer = new Timer();
            timer.Interval = 100; // 100ms마다 타이머 실행
            timer.Tick += Timer_Tick;
            timer.Start();

            this.Paint += Form1_Paint;
            this.KeyDown += Form1_KeyDown; // 키 입력 이벤트 핸들러 등록
            GenerateFood(); // 먹이 생성
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
            this.Invalidate(); // 화면 업데이트
        }

        private void MoveSnake()
        {
            Point head = snake[0];

            // 뱀의 머리 위치 업데이트
            switch (direction)
            {
                case Direction.Up:
                    head.Y--;
                    break;
                case Direction.Down:
                    head.Y++;
                    break;
                case Direction.Left:
                    head.X--;
                    break;
                case Direction.Right:
                    head.X++;
                    break;
            }

            // 벽 충돌 확인
            if (head.X < 0 || head.X >= gridSize || head.Y < 0 || head.Y >= gridSize)
            {
                // 게임 종료 시각적 표시
                gameOver = true;
                timer.Stop(); // 타이머 중지
                SaveScore(); // 점수 저장
                MessageBox.Show("게임 오버! 점수: " + score);
                return; // 더 이상 진행하지 않음
            }

            // 먹이를 먹었는지 확인
            if (head == food)
            {
                snake.Insert(0, head); // 머리를 추가
                score += 10; // 점수 증가
                GenerateFood(); // 새로운 먹이 생성
            }
            else
            {
                snake.Insert(0, head); // 머리를 추가
                snake.RemoveAt(snake.Count - 1); // 꼬리 제거
            }

            // 자기 자신과 충돌 확인
            if (snake.Skip(1).Contains(head))
            {
                // 게임 종료 시각적 표시
                gameOver = true;
                timer.Stop(); // 타이머 중지
                SaveScore(); // 점수 저장
                MessageBox.Show("게임 오버! 점수: " + score);
                return; // 더 이상 진행하지 않음
            }
        }

        private void GenerateFood()
        {
            do
            {
                // 먹이는 1~19 범위에서 생성 (0,0) ~ (20,20)의 경계 내에서
                food = new Point(random.Next(1, gridSize - 1), random.Next(1, gridSize - 1));
            } while (snake.Contains(food)); // 뱀의 위치와 겹치지 않도록
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 벽 그리기
            for (int i = 0; i < gridSize; i++)
            {
                // 상단 경계
                g.FillRectangle(Brushes.Black, i * tileSize, 0, tileSize, tileSize);
                // 하단 경계
                g.FillRectangle(Brushes.Black, i * tileSize, (gridSize - 1) * tileSize, tileSize, tileSize);
                // 왼쪽 경계
                g.FillRectangle(Brushes.Black, 0, i * tileSize, tileSize, tileSize);
                // 오른쪽 경계
                g.FillRectangle(Brushes.Black, (gridSize - 1) * tileSize, i * tileSize, tileSize, tileSize);
            }

            // 뱀 그리기
            for (int i = 0; i < snake.Count; i++)
            {
                g.FillRectangle(Brushes.Green, snake[i].X * tileSize, snake[i].Y * tileSize, tileSize, tileSize);
            }

            // 먹이 그리기
            g.FillRectangle(Brushes.Red, food.X * tileSize, food.Y * tileSize, tileSize, tileSize);

            // 점수 표시
            g.DrawString($"점수: {score}", new Font("Arial", 12), Brushes.Black, new PointF(10, 10));

            // 게임 오버 시 메시지 표시
            if (gameOver)
            {
                g.DrawString("게임 오버!", new Font("Arial", 24), Brushes.Red, new PointF(100, 100));
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // 방향 변경
            if (e.KeyCode == Keys.Up && direction != Direction.Down) direction = Direction.Up;
            if (e.KeyCode == Keys.Down && direction != Direction.Up) direction = Direction.Down;
            if (e.KeyCode == Keys.Left && direction != Direction.Right) direction = Direction.Left;
            if (e.KeyCode == Keys.Right && direction != Direction.Left) direction = Direction.Right;
        }

        private void SaveScore()
        {
            File.WriteAllText(scoreFilePath, score.ToString()); // 점수를 파일에 저장
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
