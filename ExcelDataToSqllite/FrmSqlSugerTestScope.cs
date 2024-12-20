using ExcelDataToSqllite.Entitys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExcelDataToSqllite
{
    public delegate void ProgressUpdateDelegate(int progress);
    public class FrmSqlSugerTestScope<T> where T : EntityBase, new()
    {

        private ProgressUpdateDelegate progressUpdate;

        private DbType m_dbType;
        public FrmSqlSugerTestScope(string connStr,DbType dbType, ProgressUpdateDelegate progressUpdateDelegate)
        {
            this.progressUpdate = progressUpdateDelegate;
            m_dbType = dbType;
            Db = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = connStr,//连接符字串
                DbType = dbType,//数据库类型
                IsAutoCloseConnection = true //不设成true要手动close
                 
                
            },
           db =>
           {
               //(A)全局生效配置点，一般AOP和程序启动的配置扔这里面 ，所有上下文生效
               //调试SQL事件，可以删掉
               db.Aop.OnLogExecuting = (sql, pars) =>
               {
                   Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响


                   //5.0.8.2 获取无参数化 SQL  对性能有影响，特别大的SQL参数多的，调试使用
                   //UtilMethods.GetSqlString(DbType.SqlServer,sql,pars)
               };

               //多个配置就写下面
               //db.Ado.IsDisableMasterSlaveSeparation=true;

               //注意多租户 有几个设置几个
               //db.GetConnection(i).Aop
           });
        }

        public  SqlSugarScope Db = null;


        public List<T> LoadSysAdmin()
        {
            List<T> ListAdmin = Db.Queryable<T>().ToList();
            return ListAdmin;
        }

        public void QuickClearAllData( )
        {
            // 使用 SQL 命令直接清空数据表
            // 使用 SQL 命令直接清空数据表
            string tableName = Db.EntityMaintenance.GetEntityInfo<T>().DbTableName;
            string sql = "";
            switch (m_dbType)
            {
                case DbType.MySql:
                    
                case DbType.SqlServer:
                    // 对于 MySQL 和 SQL Server 使用 TRUNCATE TABLE
                    sql = $"TRUNCATE TABLE {tableName}";
                    break;
                case DbType.Sqlite:
                    // 对于 SQLite 使用 DELETE FROM，然后可选地使用 VACUUM
                    sql = $"DELETE FROM {tableName}";
                    Db.Ado.ExecuteCommand(sql);
                    sql = "VACUUM";

                    break;
                case DbType.Oracle:
                    break;
                case DbType.PostgreSQL:
                    break;
                case DbType.Dm:
                    break;
                case DbType.Kdbndp:
                    break;
                case DbType.Oscar:
                    break;
                case DbType.MySqlConnector:
                    break;
                case DbType.Access:
                    break;
                case DbType.OpenGauss:
                    break;
                case DbType.QuestDB:
                    break;
                case DbType.HG:
                    break;
                case DbType.ClickHouse:
                    break;
                case DbType.GBase:
                    break;
                case DbType.Odbc:
                    break;
                case DbType.Custom:
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(sql))
            {
                Db.Ado.ExecuteCommand(sql);
            }
        }


        /// <summary>
        /// 非常慢
        /// </summary>
        /// <returns></returns>
        public async Task ClearAllDataAsync()
        {
            var allData = Db.Queryable<T>().ToList();
            int totalCount = allData.Count;
            int progress = 0;

            await Task.Run(() =>
            {
                foreach (var item in allData)
                {
                    Db.Deleteable<T>().Where(it => it.Id == item.Id).ExecuteCommand();

                    progress++;
                    int currentProgress = (int)((double)progress / totalCount * 100);
                    progressUpdate?.Invoke(currentProgress);
                }
            });
        }

        //按要求删除某一数据
        public void DetelData( string LoginID)
        {
           
            // 检查是否存在匹配的记录
            bool exists = Db.Queryable<T>().Where(c => c.RoadNum.Contains( LoginID)).Any();
            if (exists)
            {
                // 如果存在匹配的记录，执行更新操作
                Db.Deleteable<T>().Where(C => C.RoadNum.Contains( LoginID)).ExecuteCommand();
            }
            else
            {

            }


        }
        public void DetelData()
        {

            // 检查是否存在匹配的记录
            bool exists = Db.Queryable<T>().Where( t=>t.RoadNum.Length<8).Any();
            if (exists)
            {
                // 如果存在匹配的记录，执行更新操作
                Db.Deleteable<T>().Where(C => C.RoadNum.Length<8).ExecuteCommand();
            }
            else
            {

            }


        }
        public  List<T>  FindData(string LoginID)
        {
            List<T> datas = new List<T>();
            // 检查是否存在匹配的记录
            bool exists = Db.Queryable<T>().Where(c => c.RoadNum == LoginID).Any();
            if (exists)
            {
                // 如果存在匹配的记录，执行更新操作
                 datas =  Db.Queryable<T>().Where(c => c.RoadNum == LoginID).ToList(); 
            }
            else
            {

            }
            return datas;

        }
        //增加数据
        public void AddData( T objAdmin)
        {
            Db.Insertable<T>(objAdmin).ExecuteCommand();
        }
        public Task AddAllData(List<T> lstData)
        {
        return Task.Run(
                () => {
                    int i = 0;
                    foreach (var objAdmin in lstData)
                    {
                        i++;
                        Db.Insertable<T>(objAdmin).ExecuteCommand();
                        progressUpdate?.Invoke(i);
                    }
                }
                );
            
        }

        //改变某一数据内容
        public void Update( T objAdmin)
        {
            Db.Updateable<T>(objAdmin).WhereColumns(c => c.RoadNum).ExecuteCommand();
        }
        public async Task UpAlldate(List<T> lstData)
        {
          await  Task.Run(
                ()=>
                {
                    int i = 0;
                    foreach (var objAdmin in lstData)
                    {
                        i++;
                       
                        // 检查是否存在匹配的记录
                        bool exists = Db.Queryable<T>().Where(c => c.RoadNum == objAdmin.RoadNum).Any();
                        if (exists)
                        {
                            // 如果存在匹配的记录，执行更新操作
                            Db.Updateable<T>(objAdmin).WhereColumns(c => c.RoadNum).ExecuteCommand();
                        }
                        else
                        {

                        }
                       
                        progressUpdate?.Invoke(i);
                    }
                }
                );
           
               
        }


        public List<T> Queryable( T objAdmin)
        {
            List<T> List = Db.Queryable<T>().Where(c => c.RoadNum == objAdmin.RoadNum).ToList();
            return List;
        }

        public void InsertAll(List<T> lstData)
        {
            //WinFrom中 不能直接用，会出现卡住不动，因为WinFrom不支持直接调用异步 ，需要加委托 
            //bulkCopy 和 bulkCopyAsync 底层都是异步实现
            //Func<int> func = () => Db.Fastest<T>().BulkCopy(lstData);
            //func.BeginInvoke(x =>
            //{
            //    var result = func.EndInvoke(x);//获取返回值
            //    MessageBox.Show($"成功添加数据：{result}条");
            //}, null);

            Db.Fastest<T>().BulkCopy(lstData);
            
        }

    }
}
