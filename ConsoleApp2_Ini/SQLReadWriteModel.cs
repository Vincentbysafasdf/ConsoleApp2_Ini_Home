using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2_Ini
{

    public class SQLReadWriteModel
    {
        //using System.Data.SqlClient;

        SqlConnection conn = null;

        public SQLReadWriteModel(string _server, string _dataBase, string _uid, string _pwd)
        {
            //读取配置文件
            //strConn = ConfigurationManager.AppSettings["Conn"].ToString();
            //strConn = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            server = _server;   //服务器名
            dataBase = _dataBase;  //数据库名，可以省略
            uid = _uid;   //登录到服务器的用户名
            pwd = _pwd;   //登录到服务器的用户密码

            //可以不指定连接到的数据库，而是连接到默认数据库
            if (dataBase == null || dataBase.ToString().Length <= 0)
            {
                connString = "Server=" + server + ";" + "Uid=" + uid + ";" + "Pwd=" + pwd;//连接到默认数据库
            }
            else
            {
                connString = "Server=" + server + ";" + "DataBase=" + dataBase + ";" + "Uid=" + uid + ";" + "Pwd=" + pwd;
            }

            conn = new SqlConnection(connString);//实例化

            try
            {
                conn.Open();   //打开连接

                //根据连接状态获取服务器的信息
                if (conn.State == ConnectionState.Open)
                {
                    isConnected = true; //可以连接成功
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                conn?.Close();//释放销毁,使用时建立，用后立即销毁。
            }
        }


        /// <summary>
        /// 执行增删改SQL语句,返回受影响的行数（一个 int 值）（NonQuery=不返回其他数据库数据？）。
        /// </summary>
        /// <param name="cmdText">sql命令字符串</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string cmdText)
        {
            if (!isConnected)
            {
                return -1;
            }

            int result;

            using (conn = new SqlConnection(connString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(cmdText, conn);

                cmd.CommandType = CommandType.Text;

                try
                {
                    result = cmd.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    result = -1;
                }
            }

            return result;
        }


        /// <summary>
        /// （大量）读取数据库的内容，返回一个可以快捷更新到数据库的dataTable。
        /// </summary>
        /// <param name="cmdText">sql命令字符串</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string cmdText)
        {
            if (!isConnected)
            {
                return null;
            }

            DataTable dt = new DataTable();

            using (conn = new SqlConnection(connString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(cmdText, conn);

                cmd.CommandType = CommandType.Text;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);

                sda.Fill(dt);



                // 注意,增删改sda后可以将其更新会数据库
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(dt);

                sda.Update(dataSet, dt.TableName);
                sda.Update(dt);
                //

            }
            DataSet ds = new DataSet();

            return dt;
        }


        /// <summary>
        /// （较快）读取数据库的内容,返回一个 dataTable。
        /// </summary>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string cmdText)
        {
            if (!isConnected)
            {
                return null;
            }

            DataTable dt = new DataTable();

            using (conn = new SqlConnection(connString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(cmdText, conn);

                SqlDataReader sdr = cmd.ExecuteReader();

                dt.Load(sdr);
                sdr.Close();
                sdr.Dispose();
            }

            return dt;
        }


        /// <summary>
        /// 获取当前数据库的所有表的信息，返回一个 dataTable。
        /// 通过使用架构名称的指定字符串，返回此 SqlConnection 的数据源的架构信息。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataTable GetAllTableMessage()
        {
            if (!isConnected)
            {
                return null;
            }

            DataTable dt = new DataTable();

            using (conn = new SqlConnection(connString))
            {
                conn.Open();
                //从当前连接到的数据库中获得所有表(参数是"Tables"),
                //dt = conn.GetSchema("Tables");//GetSchema:返回一个DataTable包含数据库中"Tables"的信息
                //dt=conn.GetSchema("MetaDataCollections");
                dt = conn.GetSchema("Databases");
                //ShowDataTable(databasesSchemaTable, 25);


            }


            #region 测试用
            //foreach (DataRow row in dt.Rows)
            //{
            //    string tableName = (string)row[3];//第0列是数据库名，第1列是（模式？）"dbo"，第2列是表名，第3列是表类型
            //                                      //如果直接想获得这个数据库下的所有表，可以直接添加；
            //                                      //同样可以添加条件，对需要的表格进行筛选
            //                                      //if (tableName.Contains("xxx"))  tableNameList.Add(tableName);
            //}

            ////显示dt这个table

            //string s_Colum = "";       //列的内容

            ////打印列
            //for (int i = 0; i < dt.Columns.Count; i++)  //Columns是所有列，Column是一个列格子
            //{
            //    if (i == dt.Columns.Count - 1)  //最后一个后面不加空格
            //    {
            //        s_Colum += dt.Columns[i].ColumnName;
            //    }
            //    else                  //除了最后一个外，其他后面加空格与其他的行格子隔开
            //    {
            //        s_Colum += dt.Columns[i].ColumnName + "\0\0\0\0\0";
            //    }
            //}
            //Console.WriteLine(s_Colum);

            ////打印行
            //foreach (DataRow row in dt.Rows)   //row 是一行
            //{
            //    string s_Row = "";   //行的内容

            //    for (int i = 0; i < row.ItemArray.Length; i++)
            //    {
            //        if (i == row.ItemArray.Length - 1)//最后一个后面不加空格
            //        {
            //            s_Row += row[i].ToString();
            //        }
            //        else  //除了最后一个外，其他后面加空格与其他的行格子隔开
            //        {
            //            s_Row += row[i].ToString() + "\0\0\0\0\0\0\0\0\0\0\0\0";
            //        }
            //    }

            //    Console.WriteLine(s_Row);
            //}
            #endregion
            return dt;//用于存放表格名称的表
        }

        /// <summary>
        /// 将一个datatable打印输出到屏幕,
        /// 不属于这个库,
        /// 暂时放到这里,
        /// </summary>
        /// <param name="dataTable">要打印的datatable</param>
        public void PrintDatatableToScreen(DataTable dataTable)
        {
            //显示dt这个table
            Console.WriteLine("  ");
            Console.WriteLine("-------Start----------------------------------");
            Console.WriteLine("  ");
            Console.WriteLine("  ");

            if (dataTable == null && dataTable.Columns.Count == 0) //检查datatable的列数是否为空
            {
                Console.WriteLine("      提示：表中不存在可以打印的内容！");
            }

            string s_Colum = "";       //列的内容

            // 先找到表中行的每个行格子的长度，方便对齐

            //求列名的最大长度
            int[] columnLengths = new int[dataTable.Columns.Count];  //声明一个数组，数组的长度为列的格子数。

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                columnLengths[i] = dataTable.Columns[i].ColumnName.Length;   //获取每一个列名的长度
            }


            //求行格子的最大长度

            int[] rowLengths = new int[dataTable.Rows[0].ItemArray.Length];  //声明一个数组，数组的长度第0行的行格子数。

            for (int i = 0; i < dataTable.Rows[0].ItemArray.Length; i++)
            {
                rowLengths[i] = dataTable.Rows[0][i].ToString().Length;  //获取第0行的每一个行格子的长度
            }

            int[] maxInt = new int[dataTable.Columns.Count]; //对应的列行中长度较大的数值

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                maxInt[i] = Math.Max(columnLengths[i],rowLengths[i]);  //求对应的列行中长度较大的数值
            }





              //int maxLength=rowLengths.Max();  //


            //打印列
            for (int i = 0; i < dataTable.Columns.Count; i++)  //Columns是所有列，Column是一个列格子
            {
                if (i == dataTable.Columns.Count - 1)  //最后一个后面不加空格
                {
                    s_Colum += dataTable.Columns[i].ColumnName;
                }
                else                  //除了最后一个外，其他后面加空格与其他的行格子隔开
                {
                    s_Colum += dataTable.Columns[i].ColumnName + "";
                }
            }
            Console.WriteLine(s_Colum); //打印输出列 

            //打印行
            foreach (DataRow row in dataTable.Rows)   //row 是一行
            {
                string s_Row = "";

                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == row.ItemArray.Length - 1)//最后一个后面不加空格
                    {
                        s_Row += row[i].ToString();
                    }
                    else  //除了最后一个外，其他后面加空格与其他的行格子隔开
                    {
                        s_Row += row[i].ToString() + "";
                    }
                }

                Console.WriteLine(s_Row);//打印输出 一行
            }

            Console.WriteLine("  ");
            Console.WriteLine("  ");
            Console.WriteLine("-------End-----" + DateTime.Now.ToString() + "------------");
            Console.WriteLine("  ");
        }





        private bool isConnected = false; //可以连接成功
        /// <summary>
        /// 可以连接成功
        /// </summary>
        public bool IsConnected
        {
            get => isConnected;
            set => isConnected = value;
        }



        #region 属性+字段： 登录信息

        /// <summary>
        /// 连接字符串格式：服务器名+数据库名（可缺省）+用户名+密码。如：string connString = "Server=MN02-L00008\\CITADEL;DataBase=databaseForUser;Uid=sa;Pwd=123456";
        /// </summary>
        private string connString = "Server=MN02-L00008\\CITADEL;DataBase=databaseForUser;Uid=sa;Pwd=123456"; //本地用的用户登录信息

        private string server = "MN02-L00008\\CITADEL";

        private string dataBase = "databaseForUser";

        private string uid = "sa";

        private string pwd = "123456";


        /// <summary>
        /// 服务器名称
        /// </summary>
        public string Server
        {
            get => server;
            set
            {
                server = value;
                connString = server + ";" + dataBase + ";" + uid + ";" + pwd;
            }
        }


        /// <summary>
        /// 要连接到的数据库，省略则会连接到服务器的默认数据库，再根据需要进行切换。
        /// </summary>
        public string DataBase
        {
            get => dataBase;
            set
            {
                dataBase = value;
                connString = server + ";" + dataBase + ";" + uid + ";" + pwd;
            }
        }


        /// <summary>
        /// 用户名
        /// </summary>
        public string Uid
        {
            get => uid;
            set
            {
                uid = value;
                connString = server + ";" + dataBase + ";" + uid + ";" + pwd;
            }
        }


        /// <summary>
        /// 用户密码
        /// </summary>
        public string Pwd
        {
            get => pwd;
            set
            {
                pwd = value;
                connString = server + ";" + dataBase + ";" + uid + ";" + pwd;
            }
        }
        #endregion

    }
}
