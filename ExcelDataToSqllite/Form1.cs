using Dm;
using ExcelDataToSqllite.Entitys;
using Spire.Xls;
using SqlSugar;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ExcelDataToSqllite
{
    public partial class Form1 : Form
    {
        SQLiteConnection m_dbConnection;
        public Form1()
        {
            InitializeComponent();
        }
        public void UpdateProgressBar(int progress)
        {
            // 确保线程安全
            if (this.InvokeRequired)
            {
                this.Invoke(new ProgressUpdateDelegate(UpdateProgressBar), new object[] { progress });
            }
            else
            {
                progressBar1.Value = progress;
            }
        }
        string getDbConnectStr()
        {
            // 新增代码：让用户选择数据库文件
            OpenFileDialog dbFileDialog = new OpenFileDialog();
            dbFileDialog.Title = "请选择数据库文件";
            dbFileDialog.Filter = "SQLite 数据库文件 (*.db)|*.db";
            DialogResult dbResult = dbFileDialog.ShowDialog();

            string dbFileName = "";
            if (dbResult == DialogResult.OK)
            {
                dbFileName = dbFileDialog.FileName;
            }

            // 使用用户选择的数据库文件来构建连接字符串
            string connectStr = "";
            if (!string.IsNullOrWhiteSpace(dbFileName))
            {
                connectStr = $@"Data Source={dbFileName};";
            }
            else
            {
                MessageBox.Show("未选择数据库文件，无法继续操作。");
                return "";
            }
            return connectStr;
        }

        string getNewDbConnectStr(string FileName)
        { 
            FileInfo file = new FileInfo(FileName);
            string dirName =  file.Directory.FullName + "\\propertyInfo.db";

            string dbFileName =  AppDomain.CurrentDomain.BaseDirectory + "propertyInfo.db";

            File.Copy(dbFileName, dirName, true);

            //复制db文件到用户选择目录下
            // 使用用户选择的数据库文件来构建连接字符串
            string connectStr = "";
            if (!string.IsNullOrWhiteSpace(dirName))
            {
                connectStr = $@"Data Source={dirName};";
            }
            else
            {
                MessageBox.Show("未选择数据库文件，无法继续操作。");
                return "";
            }
            return connectStr;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.progressBar1.Maximum = 100;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择资产报表xlsx";
            openFileDialog.Filter = "(*.excel)|*.xlsx";
            var result = openFileDialog.ShowDialog();

            string fileName = "";
            if (result == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string outPath = "result.xls";
                ExcelToCsv(fileName, outPath);
                // ExcelToCsv(outPath);
                string[] lines = await File.ReadAllLinesAsync(outPath);
                int lineCount = GetFirstMatchingLineIndex(lines, s => ValidateFormat(s.Split(',').First()));
                //string connectStr = @" Data Source=hefeiDB.db;";

                string connectStr = getNewDbConnectStr(fileName);
                if (string.IsNullOrEmpty(connectStr))
                {
                    return;
                }
                var sqlScope = new FrmSqlSugerTestScope<HeFeiEntity>(connectStr, SqlSugar.DbType.Sqlite, UpdateProgressBar);
                List<HeFeiEntity> datas = new List<HeFeiEntity>();
                for (int i = lineCount; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');
                    if (line.Length > 1)
                    {
                        HeFeiEntity entity = new HeFeiEntity();
                        entity.RoadNum = line[0];
                        entity.StartMile = double.Parse(line[1].ToString());
                        entity.EndMile = double.Parse(line[2].ToString());
                        entity.Grad = line[3];
                        entity.Width = line[4];
                        entity.RoadType = line[7];
                        string pubRoadStr = line[5];
                        if (pubRoadStr.StartsWith("S") || pubRoadStr.StartsWith("G"))
                        {
                            entity.IsPub = "是";
                            entity.PubRoadNum = pubRoadStr;
                        }
                        else
                        {
                            entity.IsPub = "否";
                            entity.PubRoadNum = "";
                        }
                        entity.Unit = line[6];
                        datas.Add(entity);
                    }
                }


                //
                sqlScope.QuickClearAllData();

                sqlScope.InsertAll(datas);
                messageZiChanBiao();
            }


        }

        private void messageZiChanBiao()
        {
            MessageBox.Show("已完成导入");
        }
        public static bool ValidateFormat(string input)
        {
            string pattern = @"^[A-Z]\d{9}$";
            return Regex.IsMatch(input, pattern);
        }
        int GetFirstMatchingLineIndex(string[] lines, Func<string, bool> condition)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (condition(lines[i]))
                {
                    return i;
                }
            }

            return -1; // 如果没有满足条件的行，则返回 -1 表示未找到
        }
        private static void ExcelToCsv(string excelPath, string outPath)
        {
            //string excelPath = @"341_农村公路明细表_202208.30(3).xlsx";

            Workbook wb = new Workbook();
            wb.LoadFromFile(excelPath);
            Worksheet ws = wb.Worksheets[0];
            ws.SaveToFile(outPath, ",", encoding: Encoding.UTF8);
        }

        //创建一个空的数据库
        void createNewDatabase()
        {
            SQLiteConnection.CreateFile("MyDatabase.db");//可以不要此句
        }

        //创建一个连接到指定数据库
        void connectToDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");//没有数据库则自动创建
            m_dbConnection.Open();
        }

        //在指定数据库中创建一个table
        void createTable()
        {
            string sql = "create table  if not exists highscores (name varchar(20), score int)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        //插入一些数据
        void fillTable()
        {
            string sql = "insert into highscores (name, score) values ('Me', 3000)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            sql = "insert into highscores (name, score) values ('Myself', 6000)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();

            sql = "insert into highscores (name, score) values ('And I', 9001)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        //使用sql查询语句，并显示结果
        void printHighscores()
        {
            string sql = "select * from highscores order by score desc";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("Name: " + reader["name"] + "\tScore: " + reader["score"]);
            Console.ReadLine();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "标准样式2023.xlsx";

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择资产报表xlsx";
            openFileDialog.Filter = "(*.excel)|*.xlsx";
            var result = openFileDialog.ShowDialog();

            string fileName = "";
            if (result == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string outPath = "result.xls";
                ExcelToCsv(fileName, outPath);
                // ExcelToCsv(outPath);
                string[] lines = await File.ReadAllLinesAsync(outPath);
                int lineCount = GetFirstMatchingLineIndex(lines, s => ValidateFormat(s.Split(',').First()));
                string connectStr = getDbConnectStr();
                if (string.IsNullOrEmpty(connectStr))
                {
                    return;
                }
                var sqlScope = new FrmSqlSugerTestScope<HeFeiEntity>(connectStr, SqlSugar.DbType.Sqlite, UpdateProgressBar);
                List<HeFeiEntity> datas = new List<HeFeiEntity>();
                for (int i = lineCount; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');
                    if (line.Length > 1)
                    {
                        HeFeiEntity entity = new HeFeiEntity();
                        entity.RoadNum = line[0];
                        entity.StartMile = double.Parse(line[1].ToString());
                        entity.EndMile = double.Parse(line[2].ToString());
                        entity.Grad = line[3];
                        entity.Width = line[4];
                        entity.RoadType = line[7];
                        string pubRoadStr = line[5];
                        if (pubRoadStr.StartsWith("S") || pubRoadStr.StartsWith("G"))
                        {
                            entity.IsPub = "是";
                            entity.PubRoadNum = pubRoadStr;
                        }
                        else
                        {
                            entity.IsPub = "否";
                            entity.PubRoadNum = "";
                        }
                        entity.Unit = line[6];
                        datas.Add(entity);
                    }
                }
                this.progressBar1.Maximum = datas.Count;
                sqlScope.AddAllData(datas);
                messageZiChanBiao();
            }

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择资产报表xlsx";
            openFileDialog.Filter = "(*.excel)|*.xlsx";
            var result = openFileDialog.ShowDialog();

            string fileName = "";
            if (result == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string outPath = "result.xls";
                ExcelToCsv(fileName, outPath);
                // ExcelToCsv(outPath);
                string[] lines = await File.ReadAllLinesAsync(outPath);
                int lineCount = GetFirstMatchingLineIndex(lines, s => ValidateFormat(s.Split(',').First()));
                string connectStr = getDbConnectStr();
                if (string.IsNullOrEmpty(connectStr))
                {
                    return;
                }
                lineCount = 11;
                var sqlScope = new FrmSqlSugerTestScope<HeFeiEntity>(connectStr, SqlSugar.DbType.Sqlite, UpdateProgressBar);
                List<HeFeiEntity> datas = new List<HeFeiEntity>();
                for (int i = lineCount; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');
                    if (line.Length > 1)
                    {
                        HeFeiEntity entity = new HeFeiEntity();
                        entity.RoadNum = line[0];
                        entity.StartMile = double.Parse(line[1].ToString());
                        entity.EndMile = double.Parse(line[2].ToString());
                        entity.Grad = line[3];
                        entity.Width = line[4];
                         entity.RoadType = line[7];
                        string pubRoadStr = line[5];
                        if (pubRoadStr.StartsWith("S") || pubRoadStr.StartsWith("G"))
                        {
                            entity.IsPub = "是";
                            entity.PubRoadNum = pubRoadStr;
                        }
                        else
                        {
                            entity.IsPub = "否";
                            entity.PubRoadNum = "";
                        }
                        entity.Unit = line[6];
                        datas.Add(entity);
                    }
                }
                this.progressBar1.Maximum = datas.Count;
                //先删除表中同样道路编号的数据

                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = datas.Count;
                int d = 0;
                foreach (var item in datas)
                {
                    d++;
                    this.progressBar1.Value = d;
                    sqlScope.DetelData(item.RoadNum);
                    Application.DoEvents();
                }
                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = datas.Count;
                await sqlScope.AddAllData(datas);
                //  await sqlScope.UpAlldate(datas);
                messageZiChanBiao();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 设置 button1 的悬浮提示文字
            toolTip1.SetToolTip(this.button1, "清空用户选择的数据库所有数据,将读取到的表格写入");
            toolTip1.SetToolTip(this.button2, "直接添加表中数据到数据库中");
            toolTip1.SetToolTip(this.button3, "向用户选择数据库添加新的数据");
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private async void button5_Click(object sender, EventArgs e)
        {
         
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择资产报表xlsx";
            openFileDialog.Filter = "(*.excel)|*.xlsx";
            var result = openFileDialog.ShowDialog();

            string fileName = "";
            if (result == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string outPath = "result.xls";
                ExcelToCsv(fileName, outPath);
                // ExcelToCsv(outPath);
                string[] lines = await File.ReadAllLinesAsync(outPath);
                int lineCount = GetFirstMatchingLineIndex(lines, s => ValidateFormat(s.Split(',').First()));
                string connectStr = getDbConnectStr();
                if (string.IsNullOrEmpty(connectStr))
                {
                    return;
                }
                lineCount = 11;
                var sqlScope = new FrmSqlSugerTestScope<HeFeiEntity>(connectStr, SqlSugar.DbType.Sqlite, UpdateProgressBar);
                List<HeFeiEntity> datas = new List<HeFeiEntity>();
                for (int i = lineCount; i < lines.Length; i++)
                {
                    string[] line = lines[i].Split(',');
                    if (line.Length > 1)
                    {
                        HeFeiEntity entity = new HeFeiEntity();
                        entity.RoadNum = line[0];
                        entity.StartMile = double.Parse(line[1].ToString());
                        entity.EndMile = double.Parse(line[2].ToString());
                        entity.Grad = line[3];
                        entity.Width = line[4];
                        // entity.RoadType = line[7];
                        string pubRoadStr = line[5];
                        if (pubRoadStr.StartsWith("S") || pubRoadStr.StartsWith("G"))
                        {
                            entity.IsPub = "是";
                            entity.PubRoadNum = pubRoadStr;
                        }
                        else
                        {
                            entity.IsPub = "否";
                            entity.PubRoadNum = "";
                        }
                        entity.Unit = line[6];
                        datas.Add(entity);
                    }
                }
                sqlScope.DetelData();
                //先删除表中同样道路编号的数据

                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = datas.Count;
                int d = 0;
                List<HeFeiEntity> datass = new List<HeFeiEntity>();
                foreach (var item in datas)
                {
                    d++;
                    this.progressBar1.Value = d;
                    datass.AddRange(sqlScope.FindData(item.RoadNum));
                    Application.DoEvents();
                }
                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = datas.Count;
               
                //  await sqlScope.UpAlldate(datas);

            }
        }
    }
}