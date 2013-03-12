using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nms_excel_lib
{
    public class ExportExcel
    {
        /// <summary>
        /// 全局变量，保存Excel的决定路径
        /// </summary>
        private string _fullFileName = string.Empty;
        public string FullFileName
        {
            get { return _fullFileName; }
            set { _fullFileName = value; }
        }

        /// <summary>
        /// Excel构造函数,参数是Excel文件绝对路径
        /// </summary>
        /// <param name="pathname"></param>
        public ExportExcel(string pathname)
        {
            _fullFileName = pathname;
        }

        /// <summary>
        /// 将DataTable中的数据保存为Excel文件中，传入的参数为DataTable和表的标题名称
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="SheetName"></param>
        /// <returns>保存成功返回true， 出错返回false</returns>
        public bool ToExcel(DataTable dataTable, string SheetName)
        {
            if (null == dataTable || true == string.IsNullOrEmpty(SheetName))
            {
                return false;
            }

            // 列索引，行索引
            int colIndex = 0;
            int rowIndex = 0;
            //总可见列数，总可见行数          
            int rowCount = dataTable.Rows.Count;
            int colCount = dataTable.Columns.Count;

            //如果DataTable中没有行，返回
            if (rowCount == 0)
                return false;

            // 创建Excel对象                    
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                return false;
            }
            // 创建Excel工作薄
            Microsoft.Office.Interop.Excel.Workbook xlBook = xlApp.Workbooks.Add(true);
            Microsoft.Office.Interop.Excel.Worksheet xlSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets[1];
            
            xlSheet.Name = SheetName;
            xlApp.ActiveCell.FormulaR1C1 = SheetName;
            xlApp.ActiveCell.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;

            colIndex = 0;
            // 获取列标题，隐藏的列不处理
            for (int i = 0; i < colCount; i++)
            {
                xlApp.Cells[rowIndex + 1, colIndex + 1] = dataTable.Columns[i].ColumnName;
                colIndex++;
            }

            // 保存表格内容
            for (int item = 0; item < dataTable.Rows.Count; item++)
            {
                colIndex = 0;

                foreach (object element in dataTable.Rows[item].ItemArray)
                {
                    xlApp.Cells[item + 2, colIndex + 1] = element.ToString();
                    colIndex++;
                }
            }

            try
            {
                // 保存
                xlApp.Cells.EntireColumn.AutoFit();
                xlApp.Cells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                xlApp.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;  
                xlBook.Saved = true;
                xlBook.SaveCopyAs(FullFileName);
            }
            catch
            {
                return false;
            }
            finally
            {
                xlApp.Quit();
                GC.Collect();
            }
          
            return true;
        }

        /// <summary>
        /// 获取Excel数据库的连接字符串(ODBC)
        /// </summary>
        /// <param name="pathname"></param>
        /// <param name="readOnly"></param>
        /// <returns>返回链接Excel的链接字符串</returns>
        private string GetExcelConnection(string pathname, bool readOnly)
        {
            string driver = "Driver={Microsoft Excel Driver (*.xls)}";
            string dbpath = ";DBQ=" + pathname;
            string rwflag = ";Readonly=" + (readOnly ? "1" : "0");

            return (driver + dbpath + rwflag);
        }

        /// <summary>
        /// 对DataTable中的列名称和各个单元格数据进行预处理
        /// </summary>
        /// <param name="table"></param>
        private void ProcessDataTable(DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                column.ColumnName = column.ColumnName.Trim('\'', '{', '}').Trim();

                if (column.DataType != typeof(string))
                {
                    continue;
                }

                foreach (DataRow row in table.Rows)
                {
                    row[column] = row[column].ToString().Trim('\'', '{', '}').Trim();
                    row[column] = row[column].ToString().Replace("\r", "");
                    row[column] = row[column].ToString().Replace("\n", ";");
                }
            }
        }
        /// <summary>
        /// 从Excel文件中读取指定Sheet的表格内容
        /// </summary>
        /// <param name="SheetName"></param>
        /// <returns>成功返回dataTable对象，对象中包含excel表格内容，失败返回的表格中内容为空</returns>
        public DataTable FromExcel(string SheetName)
        {
            DataTable dataTable = null;
            OdbcConnection connection = null;

            try
            {
                connection = new OdbcConnection(GetExcelConnection(FullFileName, true));
                connection.Open();

                OdbcDataAdapter adapter = new OdbcDataAdapter(string.Format("select * from [{0}]", SheetName), connection);
                dataTable = new DataTable(SheetName.Trim('\'', '$'));
                adapter.Fill(dataTable);
                ProcessDataTable(dataTable);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return dataTable;
        }

        /// <summary>
        /// 获取Excel数据库的表名称列表
        /// </summary>
        /// <param name="pathname"></param>
        /// <returns>返回Excel文件中Sheet表格名称的数组</returns>
        public string[] GetExcelTableNames(string pathname)
        {
            OdbcConnection connection = null;

            try
            {
                connection = new OdbcConnection(GetExcelConnection(pathname, true));
                connection.Open();

                DataTable table = connection.GetSchema("Tables");
                List<string> nameList = new List<string>();

                foreach (DataRow row in table.Rows)
                {
                    string name = row["table_name"].ToString();

                    if (name.EndsWith("$") || name.EndsWith("$'"))
                    {
                        nameList.Add(name);
                    }
                }

                return nameList.ToArray();
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message);
                return new string[0];
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }
    }
}
