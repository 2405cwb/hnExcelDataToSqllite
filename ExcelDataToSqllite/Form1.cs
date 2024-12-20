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
            // ȷ���̰߳�ȫ
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
            // �������룺���û�ѡ�����ݿ��ļ�
            OpenFileDialog dbFileDialog = new OpenFileDialog();
            dbFileDialog.Title = "��ѡ�����ݿ��ļ�";
            dbFileDialog.Filter = "SQLite ���ݿ��ļ� (*.db)|*.db";
            DialogResult dbResult = dbFileDialog.ShowDialog();

            string dbFileName = "";
            if (dbResult == DialogResult.OK)
            {
                dbFileName = dbFileDialog.FileName;
            }

            // ʹ���û�ѡ������ݿ��ļ������������ַ���
            string connectStr = "";
            if (!string.IsNullOrWhiteSpace(dbFileName))
            {
                connectStr = $@"Data Source={dbFileName};";
            }
            else
            {
                MessageBox.Show("δѡ�����ݿ��ļ����޷�����������");
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

            //����db�ļ����û�ѡ��Ŀ¼��
            // ʹ���û�ѡ������ݿ��ļ������������ַ���
            string connectStr = "";
            if (!string.IsNullOrWhiteSpace(dirName))
            {
                connectStr = $@"Data Source={dirName};";
            }
            else
            {
                MessageBox.Show("δѡ�����ݿ��ļ����޷�����������");
                return "";
            }
            return connectStr;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            this.progressBar1.Maximum = 100;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "��ѡ���ʲ�����xlsx";
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
                            entity.IsPub = "��";
                            entity.PubRoadNum = pubRoadStr;
                        }
                        else
                        {
                            entity.IsPub = "��";
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
            MessageBox.Show("����ɵ���");
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

            return -1; // ���û�������������У��򷵻� -1 ��ʾδ�ҵ�
        }
        private static void ExcelToCsv(string excelPath, string outPath)
        {
            //string excelPath = @"341_ũ�幫·��ϸ��_202208.30(3).xlsx";

            Workbook wb = new Workbook();
            wb.LoadFromFile(excelPath);
            Worksheet ws = wb.Worksheets[0];
            ws.SaveToFile(outPath, ",", encoding: Encoding.UTF8);
        }

        //����һ���յ����ݿ�
        void createNewDatabase()
        {
            SQLiteConnection.CreateFile("MyDatabase.db");//���Բ�Ҫ�˾�
        }

        //����һ�����ӵ�ָ�����ݿ�
        void connectToDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.db;Version=3;");//û�����ݿ����Զ�����
            m_dbConnection.Open();
        }

        //��ָ�����ݿ��д���һ��table
        void createTable()
        {
            string sql = "create table  if not exists highscores (name varchar(20), score int)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        //����һЩ����
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

        //ʹ��sql��ѯ��䣬����ʾ���
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
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "��׼��ʽ2023.xlsx";

            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "��ѡ���ʲ�����xlsx";
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
                            entity.IsPub = "��";
                            entity.PubRoadNum = pubRoadStr;
                        }
                        else
                        {
                            entity.IsPub = "��";
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
            openFileDialog.Title = "��ѡ���ʲ�����xlsx";
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
                            entity.IsPub = "��";
                            entity.PubRoadNum = pubRoadStr;
                        }
                        else
                        {
                            entity.IsPub = "��";
                            entity.PubRoadNum = "";
                        }
                        entity.Unit = line[6];
                        datas.Add(entity);
                    }
                }
                this.progressBar1.Maximum = datas.Count;
                //��ɾ������ͬ����·��ŵ�����

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
            // ���� button1 ��������ʾ����
            toolTip1.SetToolTip(this.button1, "����û�ѡ������ݿ���������,����ȡ���ı��д��");
            toolTip1.SetToolTip(this.button2, "ֱ����ӱ������ݵ����ݿ���");
            toolTip1.SetToolTip(this.button3, "���û�ѡ�����ݿ�����µ�����");
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private async void button5_Click(object sender, EventArgs e)
        {
         
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "��ѡ���ʲ�����xlsx";
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
                            entity.IsPub = "��";
                            entity.PubRoadNum = pubRoadStr;
                        }
                        else
                        {
                            entity.IsPub = "��";
                            entity.PubRoadNum = "";
                        }
                        entity.Unit = line[6];
                        datas.Add(entity);
                    }
                }
                sqlScope.DetelData();
                //��ɾ������ͬ����·��ŵ�����

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