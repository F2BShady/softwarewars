using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace QuizApp
{
    public class Question
    {
        public string Text { get; set; }
        public List<string> Choices { get; set; }
        public string CorrectAnswer { get; set; }
    }

    public class QuizApp : Form
    {
        private List<Question> questions;
        private Question currentQuestion;
        private int score;
        private Timer timer;
        private int remainingTime;

        private Label questionLabel;
        private List<Button> choiceButtons;
        private Label timerLabel;
        private Label scoreLabel;

        public QuizApp(List<Question> questions)
        {
            this.questions = questions;
            this.currentQuestion = null;
            this.score = 0;
            this.timer = null;
            this.remainingTime = 10;

            this.Text = "Shady software war";
            this.Size = new System.Drawing.Size(800, 600);
            this.BackColor = System.Drawing.Color.Black;

            this.questionLabel = new Label
            {
                Text = "",
                Font = new System.Drawing.Font("Arial", 14),
                ForeColor = System.Drawing.Color.White,
                BackColor = System.Drawing.Color.Black,
                Width = 700,
                Height = 100,
                Location = new System.Drawing.Point(50, 50),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(questionLabel);

            this.choiceButtons = new List<Button>();
            this.CreateChoiceButtons();

            this.timerLabel = new Label
            {
                Text = "",
                Font = new System.Drawing.Font("Arial", 12),
                ForeColor = System.Drawing.Color.White,
                BackColor = System.Drawing.Color.Black,
                Width = 200,
                Height = 30,
                Location = new System.Drawing.Point(300, 450),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(timerLabel);

            this.scoreLabel = new Label
            {
                Text = "Skor: 0",
                Font = new System.Drawing.Font("Arial", 12),
                ForeColor = System.Drawing.Color.White,
                BackColor = System.Drawing.Color.Black,
                Width = 200,
                Height = 30,
                Location = new System.Drawing.Point(300, 500),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(scoreLabel);

            this.NextQuestion();

            this.CreateMenu();
        }

        private void CreateChoiceButtons()
        {
            int startX = 100;
            int startY = 200;
            int buttonWidth = 300;
            int buttonHeight = 50;
            int spacingX = 50;
            int spacingY = 20;

            for (int i = 0; i < 4; i++)
            {
                Button button = new Button
                {
                    Text = "",
                    Font = new System.Drawing.Font("Arial", 12),
                    ForeColor = System.Drawing.Color.White,
                    BackColor = System.Drawing.Color.Gray,
                    Width = buttonWidth,
                    Height = buttonHeight,
                    Location = new System.Drawing.Point(startX + (i % 2) * (buttonWidth + spacingX),
                                                       startY + (i / 2) * (buttonHeight + spacingY)),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };
                button.Click += (sender, e) => CheckAnswer(i);
                this.Controls.Add(button);
                this.choiceButtons.Add(button);
            }
        }

        private void NextQuestion()
        {
            if (questions.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(questions.Count);
                currentQuestion = questions[randomIndex];
                questions.RemoveAt(randomIndex);

                questionLabel.Text = currentQuestion.Text;

                currentQuestion.Choices = currentQuestion.Choices.OrderBy(c => random.Next()).ToList();
                for (int i = 0; i < 4; i++)
                {
                    choiceButtons[i].Text = currentQuestion.Choices[i];
                    choiceButtons[i].Enabled = true;
                }

                StartTimer();
            }
            else
            {
                ShowResult();
            }
        }

        private void CheckAnswer(int choice)
        {
            string selectedChoice = currentQuestion.Choices[choice];
            if (selectedChoice == currentQuestion.CorrectAnswer)
            {
                score++;
                scoreLabel.Text = $"Skor: {score}";
            }

            StopTimer();
            NextQuestion();
        }

        private void StartTimer()
        {
            StopTimer();

            remainingTime = 10;
            UpdateTimerLabel();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Countdown;
            timer.Start();
        }

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        private void Countdown(object sender, EventArgs e)
        {
            remainingTime--;
            UpdateTimerLabel();

            if (remainingTime <= 0)
            {
                TimeUp();
            }
            else
            {
                timer.Start();
            }

            if (remainingTime <= 3 && remainingTime > 0)
            {
                if (remainingTime % 2 == 0)
                {
                    timerLabel.ForeColor = System.Drawing.Color.White;
                    timerLabel.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    timerLabel.ForeColor = System.Drawing.Color.Red;
                    timerLabel.BackColor = System.Drawing.Color.Black;
                }
            }
            else
            {
                timerLabel.ForeColor = System.Drawing.Color.White;
                timerLabel.BackColor = System.Drawing.Color.Black;
            }
        }

        private void UpdateTimerLabel()
        {
            timerLabel.Text = $"Kalan Süre: {remainingTime}";
        }

        private void TimeUp()
        {
            MessageBox.Show("Zaman Doldu! Bir sonraki soruya geçiliyor.", "Zaman Doldu");
            StopTimer();
            NextQuestion();
        }

        private void ShowResult()
        {
            MessageBox.Show($"Skorunuz: {score}/{questions.Count}", "Sonuç");
            this.Close();
        }

        private void CreateMenu()
        {
            MenuStrip menuStrip = new MenuStrip();
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Dosya");
            menuStrip.Items.Add(fileMenu);

            ToolStripMenuItem restartMenuItem = new ToolStripMenuItem("Yeniden Başlat");
            restartMenuItem.Click += RestartGame;
            fileMenu.DropDownItems.Add(restartMenuItem);

            ToolStripSeparator separator = new ToolStripSeparator();
            fileMenu.DropDownItems.Add(separator);

            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Çıkış");
            exitMenuItem.Click += ExitApplication;
            fileMenu.DropDownItems.Add(exitMenuItem);
        }

        private void RestartGame(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Oyunu yeniden başlatmak istiyor musunuz?", "Yeniden Başlat", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                this.Close();
                QuizApp quizApp = new QuizApp(questions);
                quizApp.ShowDialog();
            }
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            this.Close();
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            List<Question> questions = new List<Question>
            {
                new Question("Python hangi tür bir dildir?", new List<string> { "Derlemeli", "Yorumlamalı", "Nesne Tabanlı", "İşlemsel" }, "Yorumlamalı"),
                new Question("Java hangi platformda çalışır?", new List<string> { "Windows", "Linux", "iOS", "Android" }, "Android"),
                new Question("Hangisi bir web programlama dili değildir?", new List<string> { "HTML", "CSS", "Python", "JavaScript" }, "Python"),
                new Question("C# hangi şirket tarafından geliştirilmiştir?", new List<string> { "Microsoft", "Google", "Apple", "Facebook" }, "Microsoft"),
                new Question("Ruby hangi yıl geliştirilmiştir?", new List<string> { "1990", "1995", "2000", "2005" }, "1995"),
                new Question("Hangisi daha profösördür?", new List<string> { "Shady", "Arfelious", "Beyonder", "Sparrow" }, "Shady"),
                // Daha fazla soru eklenebilir...
            };

            QuizApp quizApp = new QuizApp(questions);
            Application.Run(quizApp);
        }
    }
}
