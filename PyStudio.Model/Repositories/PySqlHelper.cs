using PyStudio.Model.Models;
using PyStudio.Model.Models.Sys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyStudio.Model.Repositories
{
    /// <summary>
    /// 关于SQL的Help类
    /// </summary>
    public class PySqlHelper
    {
        private readonly PyStudioDBContext _context;

        public PySqlHelper(PyStudioDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="connection">连接语句</param>
        /// <param name="tableName">表名</param>
        /// <param name="list">数据集合</param>
        public static void BulkInsert<T>(string connection, string tableName, IList<T> list)
        {
            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.BatchSize = list.Count;
                bulkCopy.DestinationTableName = tableName;

                var table = new DataTable();
                var props = TypeDescriptor.GetProperties(typeof(T))
                                           .Cast<PropertyDescriptor>()
                                           .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                           .ToArray();

                foreach (var propertyInfo in props)
                {
                    bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                    table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                }

                var values = new object[props.Length];
                foreach (var item in list)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        values[i] = props[i].GetValue(item);
                    }

                    table.Rows.Add(values);
                }

                bulkCopy.WriteToServer(table);
            }
        }

        /// <summary>
        /// 录入日志信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="info">信息</param>
        /// <param name="operation">操作</param>
        /// <param name="ips">操作IP</param>
        /// <returns></returns>
        public async Task<bool> SaveLogInfo(int userId, string info, int operation, string ips)
        {
            var result = false;
            _context.Add(new SysLogger
            {
                LoggerUser = userId,
                LoggerDescription = info,
                LoggerOperation = operation,
                LoggerCreateTime = DateTime.Now,
                LoggerIps = ips
            });

            var save = await _context.SaveChangesAsync();

            if (save > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}
