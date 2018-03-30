using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace PgReplication.command
{
    public class Kernel
    {
        private static readonly string dataBaseFrom = ConfigurationManager.AppSettings["databasefrom"];
        private static readonly string dataBaseTo = ConfigurationManager.AppSettings["databaseto"];
        public void CopyToEmptyDataBase()
        {
            var tables = ConfigurationManager.AppSettings["tables"];
            if (!string.IsNullOrWhiteSpace(tables))
            {
                var tableArray = tables.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                
            }
        }
        private void CopyTableByName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return;
            }
            //创建表
            var structure_sql = $@"SELECT a.attname, pg_type.typname, a.attnotnull
                                FROM pg_class as c join pg_attribute as a on a.attrelid = c.oid join pg_type on pg_type.oid = a.atttypid 
                                where c.relname = '{tableName}' and a.attnum>0";
            var constraint_sql =$@"select pg_constraint.contype,pg_attribute.attname,pg_constraint.conname from 
                                pg_constraint inner join pg_class
                                on pg_constraint.conrelid = pg_class.oid
                                inner join pg_attribute on pg_attribute.attrelid = pg_class.oid
                                and pg_attribute.attnum = pg_constraint.conkey[1]
                                where pg_class.relname = '{tableName}'";
            var structure_table = SqlHelper.GetDataTable(true,structure_sql);
            var constraint_table = SqlHelper.GetDataTable(true, constraint_sql);
            if (structure_table==null|| constraint_table==null)
            {
                return;
            }
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"CREATE TABLE public.{tableName}");
            stringBuilder.Append("(");
            for (int i = 0; i < structure_table.Rows.Count; i++)
            {
                if (structure_table.Rows[i][2].ToString()=="t")
                {
                    stringBuilder.Append($"{structure_table.Rows[i][0].ToString()} {structure_table.Rows[i][1].ToString()} not null");
                }
                if (structure_table.Rows[i][2].ToString() == "f")
                {
                    stringBuilder.Append($"{structure_table.Rows[i][0].ToString()} {structure_table.Rows[i][1].ToString()}");
                }
                if (structure_table.Rows[i][1].ToString()== "mt_last_modified")
                {
                    stringBuilder.Append(" DEFAULT transaction_timestamp()");
                }
                if (structure_table.Rows[i][1].ToString() == "mt_version")
                {
                    stringBuilder.Append(" DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid");
                }
                stringBuilder.Append(",");
            }
            for (int i = 0; i < constraint_table.Rows.Count; i++)
            {
                if (constraint_table.Rows[i][0].ToString()=="p")
                {
                    stringBuilder.Append($"CONSTRAINT {constraint_table.Rows[i][2].ToString()} PRIMARY KEY ({constraint_table.Rows[i][1].ToString()})");
                }
            }
            stringBuilder.Append(")");
            //分批添加数据
        }
    }
}
