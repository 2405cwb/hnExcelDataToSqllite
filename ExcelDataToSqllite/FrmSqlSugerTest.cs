using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelDataToSqllite
{
    public class SqlSugarHelper
    {



    public SqlSugarClient db;

        //构造函数
        public SqlSugarHelper(string connectionString)
        {
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
            });
        }
        //查询表中所有数据




        public List<SysAdmin> LoadSysAdmin(SqlSugarHelper sqlSugarHelper)
        {
            List<SysAdmin> ListAdmin = sqlSugarHelper.db.Queryable<SysAdmin>().ToList();
            return ListAdmin;
        }
        //按要求删除某一数据
        public void DetelData(SqlSugarHelper sqlSugarHelper, int LoginID)
        {
            sqlSugarHelper.db.Deleteable<SysAdmin>().Where(C => C.LoginID == LoginID).ExecuteCommand();
        }
        //增加数据
        public void AddData(SqlSugarHelper sqlSugarHelper, SysAdmin objAdmin)
        {
            sqlSugarHelper.db.Insertable<SysAdmin>(objAdmin).ExecuteCommand();
        }
        //改变某一数据内容
        public void Update(SqlSugarHelper sqlSugarHelper, SysAdmin objAdmin)
        {
            sqlSugarHelper.db.Updateable<SysAdmin>(objAdmin).WhereColumns(c => c.LoginName).ExecuteCommand();
        }

        public List<SysAdmin> Queryable(SqlSugarHelper sqlSugarHelper, SysAdmin objAdmin)
        {
            List<SysAdmin> List = sqlSugarHelper.db.Queryable<SysAdmin>().Where(c => c.LoginID == objAdmin.LoginID).ToList();
            return List;
        }
    }
    public class SysAdmin
    {
        public int LoginID { get; set; }
        public string LoginName { get; set; }
        public string LoginPwd { get; set; }
        public int Role { get; set; }
    }

}
