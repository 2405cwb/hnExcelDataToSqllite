using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ExcelDataToSqllite.Entitys
{
    [SugarTable("T_hefei2")]//当和数据库名称不一样可以设置表别名 指定表明
    public class HeFeiEntity: EntityBase
    {
    

        public string Unit { get; set; }
        [SugarColumn(IsNullable = false)]
        public double StartMile { get; set; }
        [SugarColumn(IsNullable = false)]
        public double EndMile { get; set; }

        public string Grad { get; set; }    

        public string IsPub { get; set; }   

        public string PubRoadNum { get; set; }

        public string Width { get; set; }
        public string RoadType { get; set;
        }



    }
}
