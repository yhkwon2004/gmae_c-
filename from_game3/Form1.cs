using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace from_game3
{
    public partial class Form1 : Form
    {
        private List<int> secretNumber;
        private int strikes;
        private int balls;
        private int clearCount;
        private int attemptCount;
        private ListViewItem initialStatusItem;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
            SetupListView();
        }

        private void InitializeGame()
        {
            secretNumber = GenerateRandomNumber();
            strikes = 0;
            balls = 0;
            attemptCount = 1;
            listView1.Items.Clear();
            initialStatusItem = new ListViewItem($"");
            listView1.Items.Add(initialStatusItem);
        }

        private void SetupListView()
        {
            listView1.View = View.List;
            listView1.HeaderStyle = ColumnHeaderStyle.None;
            listView1.Columns.Clear();
        }

        private List<int> GenerateRandomNumber()
        {
            Random random = new Random();
            return Enumerable.Range(0, 10)
                             .OrderBy(x => random.Next())
                             .Take(3)
                             .ToList();
        }

        private void hit_test(object sender, EventArgs e)
        {
            int guess1 = Convert.ToInt32(domainUpDown1.SelectedItem);
            int guess2 = Convert.ToInt32(domainUpDown2.SelectedItem);
            int guess3 = Convert.ToInt32(domainUpDown3.SelectedItem);
            List<int> guesses = new List<int> { guess1, guess2, guess3 };
            CalculateStrikeAndBall(guesses);
            listView1.Items.Insert(0, $"[{attemptCount}] {strikes} Strike {balls} Ball");
            attemptCount++;

            // 리스트 아이템 수가 9개를 넘으면 초기화
            if (listView1.Items.Count > 9)
            {
                listView1.Items.Clear(); // 리스트 초기화
                listView1.Items.Add(initialStatusItem); // 초기 상태 아이템 추가
            }

            // 시도 횟수가 30회를 초과하면 게임 실패 메시지 표시
            if (attemptCount > 30)
            {
                MessageBox.Show("게임 실패! 30회 시도했습니다.");
                InitializeGame(); // 게임 초기화
            }

            if (strikes == 3)
            {
                MessageBox.Show($"축하합니다! 게임을 이겼습니다! 클리어 횟수: {clearCount}");
                InitializeGame();
            }
        }

        private void CalculateStrikeAndBall(List<int> guesses)
        {
            strikes = 0;
            balls = 0;

            for (int i = 0; i < guesses.Count; i++)
            {
                if (guesses[i] == secretNumber[i])
                {
                    strikes++;
                }
                else if (secretNumber.Contains(guesses[i]))
                {
                    balls++;
                }
            }
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e) { }
        private void domainUpDown2_SelectedItemChanged(object sender, EventArgs e) { }
        private void domainUpDown3_SelectedItemChanged(object sender, EventArgs e) { }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}
