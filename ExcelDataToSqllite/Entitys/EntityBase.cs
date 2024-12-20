using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ExcelDataToSqllite.Entitys
{
    public class EntityBase
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true, IsNullable = false)]

        public int Id { get; set; }
        [SugarColumn(IsNullable = false)]
        public string RoadNum { get; set; }
    }
}
