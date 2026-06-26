 using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;



namespace CyberSecurityChatbot
{
   
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

  // DATABASE
    static class Database
    {
        
        private const string ConnStr =
            "server=localhost;user=root;password=;database=cyberchatbot;";

        public static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(ConnStr);
            conn.Open();
            return conn;
        }

        
        public static void Initialise()
        {
            try
            {
                using var conn = GetConnection();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS tasks (
                        id          INT AUTO_INCREMENT PRIMARY KEY,
                        title       VARCHAR(200)  NOT NULL,
                        description TEXT,
                        reminder    VARCHAR(100),
                        completed   TINYINT(1)    DEFAULT 0,
                        created_at  DATETIME      DEFAULT CURRENT_TIMESTAMP
                    );";
                new MySqlCommand(sql, conn).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB init error: " + ex.Message,
                                "Database", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void AddTask(string title, string description, string reminder)
        {
            using var conn = GetConnection();
            string sql = "INSERT INTO tasks (title,description,reminder) VALUES (@t,@d,@r)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@t", title);
            cmd.Parameters.AddWithValue("@d", description);
            cmd.Parameters.AddWithValue("@r", reminder ?? "");
            cmd.ExecuteNonQuery();
        }

        public static List<TaskItem> GetTasks()
        {
            var list = new List<TaskItem>();
            using var conn = GetConnection();
            string sql = "SELECT id,title,description,reminder,completed FROM tasks ORDER BY id DESC";
            using var rdr = new MySqlCommand(sql, conn).ExecuteReader();
            while (rdr.Read())
                list.Add(new TaskItem
                {
                    Id          = rdr.GetInt32(0),
                    Title       = rdr.GetString(1),
                    Description = rdr.GetString(2),
                    Reminder    = rdr.GetString(3),
                    Completed   = rdr.GetBoolean(4)
                });
            return list;
        }

        public static void MarkCompleted(int id)
        {
            using var conn = GetConnection();
            new MySqlCommand($"UPDATE tasks SET completed=1 WHERE id={id}", conn)
                .ExecuteNonQuery();
        }

        public static void DeleteTask(int id)
        {
            using var conn = GetConnection();
            new MySqlCommand($"DELETE FROM tasks WHERE id={id}", conn)
                .ExecuteNonQuery();
        }
    }

   
    class TaskItem
    {
        public int    Id          { get; set; }
        public string Title       { get; set; }
        public string Description { get; set; }
        public string Reminder    { get; set; }
        public bool   Completed   { get; set; }

        public override string ToString() =>
            $"[{(Completed ? "✓" : " ")}] {Title}" +
            (string.IsNullOrWhiteSpace(Reminder) ? "" : $"  (Reminder: {Reminder})");
    }

    // QUIZ with 10 questions. 
    class QuizQuestion
    {
        public string   Question    { get; set; }
        public string[] Options     { get; set; }   // null = true/false
        public int      AnswerIndex { get; set; }   // 0-based; for T/F: 0=True,1=False
        public string   Explanation { get; set; }
    }

    static class QuizData
    {
        public static readonly List<QuizQuestion> Questions = new()
        {
            new() { Question    = "What should you do if you receive an email asking for your password?",
                    Options     = new[]{"Reply with your password","Delete the email",
                                        "Report the email as phishing","Ignore it"},
                    AnswerIndex = 2,
                    Explanation = "Reporting phishing emails helps prevent scams." },

            new() { Question    = "True or False: Using the same password for every account is safe.",
                    Options     = new[]{"True","False"},
                    AnswerIndex = 1,
                    Explanation = "Reusing passwords means one breach compromises all accounts." },

            new() { Question    = "Which of these is an example of a strong password?",
                    Options     = new[]{"password123","John1990","T#9vL!2qZ&","ilovecats"},
                    AnswerIndex = 2,
                    Explanation = "Strong passwords mix uppercase, lowercase, numbers and symbols." },

            new() { Question    = "True or False: HTTPS websites are always completely secure.",
                    Options     = new[]{"True","False"},
                    AnswerIndex = 1,
                    Explanation = "HTTPS encrypts traffic but doesn't guarantee the site is legitimate." },

            new() { Question    = "What is two-factor authentication (2FA)?",
                    Options     = new[]{"Logging in twice","Using two passwords",
                                        "A second verification step after your password",
                                        "Encrypting your email"},
                    AnswerIndex = 2,
                    Explanation = "2FA adds a second layer, making accounts much harder to compromise." },

            new() { Question    = "True or False: Public Wi-Fi is safe for online banking.",
                    Options     = new[]{"True","False"},
                    AnswerIndex = 1,
                    Explanation = "Public Wi-Fi can be monitored. Use a VPN for sensitive activity." },

            new() { Question    = "What does 'social engineering' mean in cybersecurity?",
                    Options     = new[]{"Building social-media apps","Hacking via physical hardware",
                                        "Manipulating people into revealing confidential info",
                                        "Updating your privacy settings"},
                    AnswerIndex = 2,
                    Explanation = "Social engineering exploits human trust rather than technical flaws." },

            new() { Question    = "True or False: Antivirus software alone guarantees full protection.",
                    Options     = new[]{"True","False"},
                    AnswerIndex = 1,
                    Explanation = "Antivirus is one layer; safe habits and updates are equally important." },

            new() { Question    = "Which action best protects against ransomware?",
                    Options     = new[]{"Opening all email attachments","Regular offline backups",
                                        "Disabling your firewall","Using only free software"},
                    AnswerIndex = 1,
                    Explanation = "Regular backups let you recover without paying a ransom." },

            new() { Question    = "True or False: Software updates should be postponed as long as possible.",
                    Options     = new[]{"True","False"},
                    AnswerIndex = 1,
                    Explanation = "Updates patch security vulnerabilities; install them promptly." },

            new() { Question    = "What is a VPN primarily used for?",
                    Options     = new[]{"Speeding up your internet","Encrypting and anonymising traffic",
                                        "Blocking advertisements","Boosting Wi-Fi signal"},
                    AnswerIndex = 1,
                    Explanation = "A VPN creates an encrypted tunnel, protecting your data in transit." },

            new() { Question    = "True or False: Clicking 'unsubscribe' in spam emails is always safe.",
                    Options     = new[]{"True","False"},
                    AnswerIndex = 1,
                    Explanation = "Spam unsubscribe links can confirm your address is active or install malware." }
        };
    }


    static class Nlp
    {
     
        private static readonly Dictionary<string, string[]> Intents = new()
        {
            ["add_task"]    = new[]{"add task","create task","new task","add a task","set a task",
                                    "add reminder","set reminder","remind me","create reminder"},
            ["view_tasks"]  = new[]{"view tasks","show tasks","list tasks","my tasks","show my tasks",
                                    "what tasks","pending tasks"},
            ["start_quiz"]  = new[]{"quiz","start quiz","play quiz","test me","test my knowledge",
                                    "cyber quiz","take quiz"},
            ["show_log"]    = new[]{"show log","activity log","what have you done","show activity",
                                    "recent actions","show history","what have you done for me"},
            ["password"]    = new[]{"password","passwords","passphrase"},
            ["phishing"]    = new[]{"phishing","phish","scam email","fake email"},
            ["safe_browse"] = new[]{"safe browsing","browse safely","safe internet","secure browsing"},
            ["2fa"]         = new[]{"two factor","2fa","two-factor","multi factor","mfa"},
            ["malware"]     = new[]{"malware","virus","ransomware","spyware","trojan"},
            ["vpn"]         = new[]{"vpn","virtual private network"},
            ["how_are_you"] = new[]{"how are you","how r u","how are you doing"},
            ["purpose"]     = new[]{"purpose","what do you do","what are you","your purpose"},
            ["help"]        = new[]{"help","what can i ask","commands","what can you do"},
            ["exit"]        = new[]{"exit","quit","bye","goodbye","close"},
        };

       
        public static string Detect(string input)
        {
            string lower = input.ToLower();
            foreach (var kv in Intents)
                foreach (string phrase in kv.Value)
                    if (lower.Contains(phrase))
                        return kv.Key;
            return null;
        }
    }


    class MainForm : Form
    {
        private RichTextBox chatBox;
        private TextBox     inputBox;
        private Button      sendBtn;
        private TabControl  tabs;
        private TabPage     chatTab, tasksTab, quizTab, logTab;

        // Tasks tab
        private ListBox  taskListBox;
        private Button   completeBtn, deleteBtn;

        // Quiz tab
        private Label    quizQuestionLabel;
        private Panel    quizOptionsPanel;
        private Label    quizFeedbackLabel;
        private Label    quizScoreLabel;
        private Button   startQuizBtn;

        // Log tab
        private ListBox logListBox;

        // ── State ───────────────────────────────────────────
        private string           userName        = "User";
        private List<string>     activityLog     = new();
        private bool             awaitingReminder = false;
        private string           pendingTaskTitle = "";
        private string           pendingTaskDesc  = "";

        // Quiz state
        private List<QuizQuestion> quizQuestions;
        private int                quizIndex   = 0;
        private int                quizScore   = 0;
        private bool               quizActive  = false;

        // ── Constructor ─────────────────────────────────────
        public MainForm()
        {
            Database.Initialise();
            BuildUI();
            AskName();
        }

        // ════════════════════════════════════════════════════
        //  UI BUILDER
        // ════════════════════════════════════════════════════
        private void BuildUI()
        {
            Text            = "Cybersecurity Awareness Chatbot";
            Size            = new Size(820, 640);
            MinimumSize     = new Size(700, 560);
            BackColor       = Color.FromArgb(18, 18, 30);
            Font            = new Font("Segoe UI", 9.5f);
            StartPosition   = FormStartPosition.CenterScreen;

            // ── Tab control ─────────────────────────────────
            tabs = new TabControl
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 18, 30),
                ForeColor = Color.Cyan
            };

            chatTab  = MakeTab("💬  Chat");
            tasksTab = MakeTab("📋  Tasks");
            quizTab  = MakeTab("🎮  Quiz");
            logTab   = MakeTab("📜  Activity Log");

            tabs.TabPages.AddRange(new[]{ chatTab, tasksTab, quizTab, logTab });
            Controls.Add(tabs);

            BuildChatTab();
            BuildTasksTab();
            BuildQuizTab();
            BuildLogTab();
        }

        private TabPage MakeTab(string title)
        {
            return new TabPage(title)
            {
                BackColor = Color.FromArgb(18, 18, 30),
                ForeColor = Color.Cyan,
                Padding   = new Padding(8)
            };
        }

        // ── Chat Tab ────────────────────────────────────────
        private void BuildChatTab()
        {
            chatBox = new RichTextBox
            {
                Dock        = DockStyle.Fill,
                ReadOnly    = true,
                BackColor   = Color.FromArgb(12, 12, 24),
                ForeColor   = Color.LightGray,
                BorderStyle = BorderStyle.None,
                Font        = new Font("Consolas", 10f),
                ScrollBars  = RichTextBoxScrollBars.Vertical
            };

            inputBox = new TextBox
            {
                Dock        = DockStyle.Fill,
                BackColor   = Color.FromArgb(30, 30, 50),
                ForeColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font        = new Font("Segoe UI", 10f)
            };
            inputBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ProcessInput(); } };

            sendBtn = new Button
            {
                Text      = "Send",
                Dock      = DockStyle.Right,
                Width     = 80,
                BackColor = Color.FromArgb(0, 150, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            sendBtn.FlatAppearance.BorderSize = 0;
            sendBtn.Click += (s, e) => ProcessInput();

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            bottomPanel.Controls.Add(inputBox);
            bottomPanel.Controls.Add(sendBtn);

            chatTab.Controls.Add(chatBox);
            chatTab.Controls.Add(bottomPanel);
        }

        // ── Tasks Tab ───────────────────────────────────────
        private void BuildTasksTab()
        {
            var lbl = new Label
            {
                Text      = "Your Cybersecurity Tasks",
                Dock      = DockStyle.Top,
                Height    = 30,
                ForeColor = Color.Cyan,
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(4,0,0,0)
            };

            taskListBox = new ListBox
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(12, 12, 24),
                ForeColor = Color.LightGray,
                Font      = new Font("Consolas", 9.5f),
                BorderStyle = BorderStyle.None
            };

            completeBtn = MakeButton("✓  Mark Complete", Color.FromArgb(0, 140, 80));
            deleteBtn   = MakeButton("✕  Delete Task",   Color.FromArgb(180, 40, 40));
            var refreshBtn = MakeButton("↻  Refresh",    Color.FromArgb(60, 60, 120));

            completeBtn.Click += (s, e) => CompleteSelectedTask();
            deleteBtn.Click   += (s, e) => DeleteSelectedTask();
            refreshBtn.Click  += (s, e) => RefreshTaskList();

            var btnPanel = new FlowLayoutPanel
            {
                Dock      = DockStyle.Bottom,
                Height    = 44,
                BackColor = Color.FromArgb(18, 18, 30),
                Padding   = new Padding(4)
            };
            btnPanel.Controls.AddRange(new Control[]{ completeBtn, deleteBtn, refreshBtn });

            tasksTab.Controls.Add(taskListBox);
            tasksTab.Controls.Add(btnPanel);
            tasksTab.Controls.Add(lbl);

            RefreshTaskList();
        }

        // ── Quiz Tab ────────────────────────────────────────
        private void BuildQuizTab()
        {
            startQuizBtn = MakeButton("▶  Start Quiz", Color.FromArgb(0, 150, 200));
            startQuizBtn.Width  = 160;
            startQuizBtn.Height = 38;
            startQuizBtn.Click += (s, e) => StartQuiz();

            quizQuestionLabel = new Label
            {
                Text      = "Press \"Start Quiz\" to begin the cybersecurity knowledge quiz!",
                Dock      = DockStyle.Top,
                Height    = 80,
                ForeColor = Color.Cyan,
                Font      = new Font("Segoe UI", 11f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(8, 0, 8, 0)
            };

            quizOptionsPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 180,
                BackColor = Color.FromArgb(18, 18, 30)
            };

            quizFeedbackLabel = new Label
            {
                Text      = "",
                Dock      = DockStyle.Top,
                Height    = 50,
                ForeColor = Color.Yellow,
                Font      = new Font("Segoe UI", 10f, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(8,0,8,0)
            };

            quizScoreLabel = new Label
            {
                Text      = "",
                Dock      = DockStyle.Top,
                Height    = 30,
                ForeColor = Color.LightGreen,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(8,0,8,0)
            };

            var startPanel = new FlowLayoutPanel
            {
                Dock      = DockStyle.Bottom,
                Height    = 50,
                BackColor = Color.FromArgb(18, 18, 30),
                Padding   = new Padding(8)
            };
            startPanel.Controls.Add(startQuizBtn);

            quizTab.Controls.Add(quizFeedbackLabel);
            quizTab.Controls.Add(quizOptionsPanel);
            quizTab.Controls.Add(quizQuestionLabel);
            quizTab.Controls.Add(quizScoreLabel);
            quizTab.Controls.Add(startPanel);
        }

        // ── Log Tab ─────────────────────────────────────────
        private void BuildLogTab()
        {
            var lbl = new Label
            {
                Text      = "Activity Log  (last 10 actions)",
                Dock      = DockStyle.Top,
                Height    = 30,
                ForeColor = Color.Cyan,
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(4,0,0,0)
            };

            logListBox = new ListBox
            {
                Dock        = DockStyle.Fill,
                BackColor   = Color.FromArgb(12, 12, 24),
                ForeColor   = Color.LightGray,
                Font        = new Font("Consolas", 9.5f),
                BorderStyle = BorderStyle.None
            };

            logTab.Controls.Add(logListBox);
            logTab.Controls.Add(lbl);
        }

        // ── Helper ──────────────────────────────────────────
        private Button MakeButton(string text, Color bg)
        {
            var btn = new Button
            {
                Text      = text,
                Height    = 34,
                Width     = 150,
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                Margin    = new Padding(2)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ════════════════════════════════════════════════════
        //  STARTUP
        // ════════════════════════════════════════════════════
        private void AskName()
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Welcome! Please enter your name:", "Cybersecurity Chatbot", "User");
            userName = string.IsNullOrWhiteSpace(name) ? "User" : name.Trim();
            BotSay($"Hello, {userName}! 👋  I'm your Cybersecurity Awareness Bot.");
            BotSay("You can chat with me, manage tasks, take a quiz, or view your activity log.");
            BotSay("Type 'help' to see what I can do.");
        }

        // ════════════════════════════════════════════════════
        //  CHAT ENGINE
        // ════════════════════════════════════════════════════
        private void ProcessInput()
        {
            string raw = inputBox.Text.Trim();
            inputBox.Clear();
            if (string.IsNullOrWhiteSpace(raw)) return;

            UserSay(raw);

            // ── Reminder follow-up ──────────────────────────
            if (awaitingReminder)
            {
                HandleReminderReply(raw);
                return;
            }

            // ── NLP intent detection ────────────────────────
            string intent = Nlp.Detect(raw);

            switch (intent)
            {
                case "add_task":    PromptAddTask(raw);          break;
                case "view_tasks":  ShowTasksSummary();          break;
                case "start_quiz":  tabs.SelectedTab = quizTab; StartQuiz(); break;
                case "show_log":    ShowActivityLog();           break;
                case "password":    RespondPassword();           break;
                case "phishing":    RespondPhishing();           break;
                case "safe_browse": RespondSafeBrowsing();       break;
                case "2fa":         Respond2FA();                break;
                case "malware":     RespondMalware();            break;
                case "vpn":         RespondVpn();                break;
                case "how_are_you": BotSay("I'm running smoothly and keeping your data safe! 😊"); break;
                case "purpose":     BotSay("I'm here to help you learn about cybersecurity and manage your security tasks."); break;
                case "help":        RespondHelp();               break;
                case "exit":        BotSay("Goodbye! Stay safe online! 🛡️"); break;
                default:            BotSay("I didn't quite understand that. Could you rephrase? (Type 'help' for options)"); break;
            }
        }

        // ── Keyword responses ────────────────────────────────
        private void RespondPassword()
        {
            BotSay("🔑 Use strong passwords with uppercase, lowercase, numbers and symbols.\n" +
                   "   Never reuse passwords across sites. Consider a password manager.");
            LogAction("NLP: Responded to password query");
        }

        private void RespondPhishing()
        {
            BotSay("🎣 Phishing is when attackers trick you into revealing personal info.\n" +
                   "   Always verify sender addresses and never click suspicious links.");
            LogAction("NLP: Responded to phishing query");
        }

        private void RespondSafeBrowsing()
        {
            BotSay("🌐 Only visit HTTPS sites, avoid suspicious links, and keep your browser updated.");
            LogAction("NLP: Responded to safe browsing query");
        }

        private void Respond2FA()
        {
            BotSay("🔐 Two-Factor Authentication adds a second verification step to logins.\n" +
                   "   Enable it on all accounts that support it — especially email and banking.");
            LogAction("NLP: Responded to 2FA query");
        }

        private void RespondMalware()
        {
            BotSay("🦠 Malware includes viruses, ransomware and spyware.\n" +
                   "   Keep antivirus updated, avoid unknown downloads, and back up your data regularly.");
            LogAction("NLP: Responded to malware query");
        }

        private void RespondVpn()
        {
            BotSay("🔒 A VPN encrypts your internet traffic, protecting you on public Wi-Fi.\n" +
                   "   Choose a reputable provider that doesn't log your activity.");
            LogAction("NLP: Responded to VPN query");
        }

        private void RespondHelp()
        {
            BotSay("📖 Here's what you can ask me:\n" +
                   "  • password / phishing / safe browsing / 2FA / malware / VPN\n" +
                   "  • 'add task <title>' – add a cybersecurity task\n" +
                   "  • 'view tasks'       – see your task list\n" +
                   "  • 'start quiz'       – test your knowledge\n" +
                   "  • 'show activity log'– see recent bot actions\n" +
                   "  • 'how are you' / 'what is your purpose'");
        }

        // ════════════════════════════════════════════════════
        //  TASK 1 – TASK ASSISTANT
        // ════════════════════════════════════════════════════
        private void PromptAddTask(string input)
        {
            // Try to extract a title after "add task" / "remind me to" etc.
            string title = ExtractTaskTitle(input);

            if (string.IsNullOrWhiteSpace(title))
            {
                title = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter a task title (e.g. 'Enable two-factor authentication'):",
                    "Add Task", "");
            }

            if (string.IsNullOrWhiteSpace(title)) { BotSay("No task title provided. Task not added."); return; }

            string desc = GetAutoDescription(title);
            pendingTaskTitle = title;
            pendingTaskDesc  = desc;
            awaitingReminder = true;

            BotSay($"Task added: '{title}'\nDescription: {desc}\nWould you like a reminder? If so, type a timeframe (e.g. 'in 3 days') or 'no'.");
            LogAction($"Task created: '{title}'");
            tabs.SelectedTab = tasksTab;
        }

        private void HandleReminderReply(string input)
        {
            awaitingReminder = false;
            string reminder = "";

            if (!input.ToLower().Contains("no"))
            {
                reminder = input;
                BotSay($"Got it! I'll remind you: {reminder}");
                LogAction($"Reminder set: '{pendingTaskTitle}' – {reminder}");
            }
            else
            {
                BotSay("No reminder set. Task saved!");
            }

            try
            {
                Database.AddTask(pendingTaskTitle, pendingTaskDesc, reminder);
                RefreshTaskList();
            }
            catch (Exception ex)
            {
                BotSay("(Note: Could not save to database – " + ex.Message + ")");
            }

            pendingTaskTitle = pendingTaskDesc = "";
        }

        private void ShowTasksSummary()
        {
            try
            {
                var tasks = Database.GetTasks();
                if (tasks.Count == 0) { BotSay("You have no tasks yet. Say 'add task' to create one!"); return; }
                BotSay($"You have {tasks.Count} task(s). Switching to the Tasks tab...");
                tabs.SelectedTab = tasksTab;
                RefreshTaskList();
                LogAction("Viewed task list");
            }
            catch { BotSay("Could not retrieve tasks from the database."); }
        }

        private void RefreshTaskList()
        {
            taskListBox.Items.Clear();
            try
            {
                foreach (var t in Database.GetTasks())
                    taskListBox.Items.Add(t);
            }
            catch { taskListBox.Items.Add("(Database unavailable)"); }
        }

        private void CompleteSelectedTask()
        {
            if (taskListBox.SelectedItem is TaskItem t)
            {
                try
                {
                    Database.MarkCompleted(t.Id);
                    LogAction($"Task completed: '{t.Title}'");
                    RefreshTaskList();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void DeleteSelectedTask()
        {
            if (taskListBox.SelectedItem is TaskItem t &&
                MessageBox.Show($"Delete task '{t.Title}'?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    Database.DeleteTask(t.Id);
                    LogAction($"Task deleted: '{t.Title}'");
                    RefreshTaskList();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        // Auto-generate a description for common task keywords
        private string GetAutoDescription(string title)
        {
            string t = title.ToLower();
            if (t.Contains("password"))    return "Update your password to a strong, unique one.";
            if (t.Contains("2fa") || t.Contains("two-factor") || t.Contains("two factor"))
                                           return "Enable two-factor authentication to secure your account.";
            if (t.Contains("privacy"))     return "Review account privacy settings to ensure your data is protected.";
            if (t.Contains("backup"))      return "Back up your important data to prevent loss.";
            if (t.Contains("antivirus") || t.Contains("anti-virus"))
                                           return "Install or update antivirus software on your device.";
            if (t.Contains("vpn"))         return "Set up a VPN for secure browsing on public networks.";
            return $"{title} – a cybersecurity task to help keep you safe online.";
        }

        private string ExtractTaskTitle(string input)
        {
            string lower = input.ToLower();
            foreach (string kw in new[]{ "add task","create task","new task","set a task","add a task","remind me to","remind me" })
                if (lower.Contains(kw))
                {
                    int idx = lower.IndexOf(kw) + kw.Length;
                    string rest = input.Substring(idx).Trim().TrimStart('-', ':').Trim();
                    if (rest.Length > 2) return rest;
                }
            return "";
        }

        // ════════════════════════════════════════════════════
        //  TASK 2 – QUIZ
        // ════════════════════════════════════════════════════
        private void StartQuiz()
        {
            quizQuestions = QuizData.Questions.OrderBy(_ => Guid.NewGuid()).ToList();
            quizIndex     = 0;
            quizScore     = 0;
            quizActive    = true;

            startQuizBtn.Text    = "Restart Quiz";
            quizFeedbackLabel.Text = "";
            quizScoreLabel.Text  = "";

            LogAction("Quiz started");
            tabs.SelectedTab = quizTab;
            ShowQuizQuestion();
        }

        private void ShowQuizQuestion()
        {
            if (quizIndex >= quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            var q = quizQuestions[quizIndex];
            quizQuestionLabel.Text = $"Q{quizIndex + 1}/{quizQuestions.Count}:  {q.Question}";
            quizFeedbackLabel.Text = "";
            quizOptionsPanel.Controls.Clear();

            int y = 4;
            for (int i = 0; i < q.Options.Length; i++)
            {
                int captured = i;
                var btn = new Button
                {
                    Text      = $"{(char)('A' + i)})  {q.Options[i]}",
                    Location  = new Point(8, y),
                    Size      = new Size(quizOptionsPanel.Width - 20, 36),
                    BackColor = Color.FromArgb(30, 30, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font      = new Font("Segoe UI", 9.5f),
                    Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 120);
                btn.Click += (s, e) => HandleQuizAnswer(captured);
                quizOptionsPanel.Controls.Add(btn);
                y += 42;
            }
        }

        private void HandleQuizAnswer(int chosen)
        {
            var q       = quizQuestions[quizIndex];
            bool correct = chosen == q.AnswerIndex;

            if (correct)
            {
                quizScore++;
                quizFeedbackLabel.ForeColor = Color.LightGreen;
                quizFeedbackLabel.Text = $"✓ Correct!  {q.Explanation}";
            }
            else
            {
                quizFeedbackLabel.ForeColor = Color.Tomato;
                quizFeedbackLabel.Text =
                    $"✗ Incorrect. The answer was '{q.Options[q.AnswerIndex]}'.  {q.Explanation}";
            }

            // Disable all option buttons
            foreach (Control c in quizOptionsPanel.Controls)
                c.Enabled = false;

            quizScoreLabel.Text = $"Score: {quizScore} / {quizIndex + 1}";
            quizIndex++;

            // Auto-advance after 1.8 s
            var t = new System.Windows.Forms.Timer { Interval = 1800 };
            t.Tick += (s, e) => { t.Stop(); if (quizActive) ShowQuizQuestion(); };
            t.Start();
        }

        private void EndQuiz()
        {
            quizActive = false;
            quizOptionsPanel.Controls.Clear();
            quizFeedbackLabel.ForeColor = Color.Cyan;

            string grade = quizScore >= quizQuestions.Count * 0.8
                ? "Great job! You're a cybersecurity pro! 🏆"
                : quizScore >= quizQuestions.Count * 0.5
                    ? "Good effort! Keep practising to stay safe online. 📚"
                    : "Keep learning to stay safe online! 🔐";

            quizQuestionLabel.Text   = "Quiz complete!";
            quizFeedbackLabel.Text   = grade;
            quizScoreLabel.Text      = $"Final score: {quizScore} / {quizQuestions.Count}";

            LogAction($"Quiz completed – score {quizScore}/{quizQuestions.Count}");
            BotSay($"Quiz done! You scored {quizScore}/{quizQuestions.Count}. {grade}");
            tabs.SelectedTab = chatTab;
        }

        // ════════════════════════════════════════════════════
        //  TASK 4 – ACTIVITY LOG
        // ════════════════════════════════════════════════════
        private void LogAction(string description)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}]  {description}";
            activityLog.Add(entry);
            if (activityLog.Count > 50) activityLog.RemoveAt(0);
            RefreshLogDisplay();
        }

        private void RefreshLogDisplay()
        {
            logListBox.Items.Clear();
            var recent = activityLog.TakeLast(10).Reverse();
            foreach (string e in recent)
                logListBox.Items.Add(e);
        }

        private void ShowActivityLog()
        {
            if (activityLog.Count == 0) { BotSay("No activity recorded yet."); return; }
            BotSay("Here's a summary of recent actions:");
            int n = 1;
            foreach (string e in activityLog.TakeLast(10))
                BotSay($"  {n++}. {e}");
            tabs.SelectedTab = logTab;
        }

        // ════════════════════════════════════════════════════
        //  CHAT DISPLAY HELPERS
        // ════════════════════════════════════════════════════
        private void UserSay(string text)
        {
            AppendChat($"{userName}: ", Color.Yellow, bold: true);
            AppendChat(text + "\n", Color.White);
        }

        private void BotSay(string text)
        {
            AppendChat("Bot: ", Color.Cyan, bold: true);
            AppendChat(text + "\n\n", Color.LightGray);
        }

        private void AppendChat(string text, Color colour, bool bold = false)
        {
            chatBox.SelectionStart  = chatBox.TextLength;
            chatBox.SelectionLength = 0;
            chatBox.SelectionColor  = colour;
            chatBox.SelectionFont   = bold
                ? new Font(chatBox.Font, FontStyle.Bold)
                : chatBox.Font;
            chatBox.AppendText(text);
            chatBox.ScrollToCaret();
        }
    }
}
