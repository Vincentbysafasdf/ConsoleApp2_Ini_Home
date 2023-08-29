using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using System.Data;

namespace ConsoleApp2_Ini
{
    public class Program
    {
        static void Main(string[] args)
        {
            SQLReadWriteModel conn = new SQLReadWriteModel("FCARPW3Y8UPHN2G\\CITADEL", "test99", "sa", "123456");

            Console.WriteLine(conn.IsConnected);  //是否连接成功


            DataTable dt = new DataTable();
            dt = conn.GetAllTableMessage();

            string cmdStr = "";

            cmdStr = "Insert Into Student(姓名,手机号,学号)values('张三6','男','31')";//添加一条新的信息到一个表里
            //cmdStr = "insert into Student01(姓名, 学号) SELECT 姓名, 学号 FROM Student"; //将一个表里的内容复制到另一个已经创建好的表中，复制内容的列名必须对应。
            cmdStr = "update Student set 姓名='马小兰',性别='女' where 学号='001'";//更新一条已经存在的数据信息的内容
            //Console.WriteLine(conn.ExecuteNonQuery(cmdStr));

            //int bbbb = conn.ExecuteNonQuery(cmdStr);//不可以

            //cmdStr = "SELECT * FROM Student";//选择表 Student 的全部内容
            //dt = conn.ExecuteQuery(cmdStr);  //正常
           // dt = conn.ExecuteDataTable(cmdStr); //正常


            conn.PrintDatatableToScreen(dt);

            int[] intArray = new int[] { 1, 10, 2, 13, 3, 4, 5, 6, 7, 8, 17, 9, 11, 12, 14, 15, 16, 18, 19, 20,21 };
           var saa= intArray.Append(100);
            Console.WriteLine(saa.Max());
        }
    }
}
