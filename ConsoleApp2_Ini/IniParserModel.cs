using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.SymbolStore;

namespace ConsoleApp2_Ini
{
    // 先下载安装IniParser
    //using IniParser.Model;
    //这里是对IniParser的再次封装
    public class IniParserModel
    {
        public IniParserModel(string filePath)
        {
            if (filePath != null && File.Exists(filePath))
            {
                this.Path = filePath;
            }
            else
            {
                throw new Exception("路径不存在");
            }
        }

        #region 方法：将全部的内容（不包含 comment）输出到字典 Dictionary<object, Dictionary<object, object>>
        /// <summary>
        /// 将全部的内容（不包含 comment）输出到字典 Dictionary<object, Dictionary<object, object>>
        /// </summary>
        /// <returns></returns>
        public Dictionary<object, Dictionary<object, object>> ReadAllContentToDic()
        {
            Dictionary<object, Dictionary<object, object>> dicTemp = new Dictionary<object, Dictionary<object, object>>();

            FileIniDataParser parser = new FileIniDataParser();    //构造一个FileIniDataParser
            IniData rData = parser.ReadFile(Path);        //声明一个 inidata 并存储读取的指定路径的 ini 文件

            foreach (var sectionData in rData.Sections)   //遍历 section，输出
            {
                Dictionary<object, object> keyValuePairs = new Dictionary<object, object>();

                foreach (var key in sectionData.Keys)
                {
                    keyValuePairs.Add(key.KeyName, key.Value);    //输出 key 的 name + key 的值
                }

                dicTemp.Add(sectionData.SectionName, keyValuePairs);   //输出所有 section 的 name
            }//遍历输出全部的内容(不包含 comment )

            return dicTemp;
        }
        #endregion


        #region 方法：从指定的 节-键 获取键值
        /// <summary>
        /// 从指定的 节-键 获取键值
        /// </summary>
        /// <param name="sectionName">节名</param>
        /// <param name="keyName">键名</param>
        /// <returns></returns>
        public object ReadAValueOfSectionKey(object sectionName, object keyName)
        {

            if (sectionName == null || keyName == null)
            {
                return null;
            }

            FileIniDataParser parser = new FileIniDataParser();    //构造一个FileIniDataParser
            IniData data = parser.ReadFile(Path);        //声明一个 inidata 并存储读取的指定路径的 ini 文件

            ////从指定Section  部分获取SectionData 
            KeyDataCollection keyCol = data[sectionName.ToString()];

            ////从指定 Section “sectionName”部分定义的键“keyName”获取 keyData
            string directValue = keyCol[keyName.ToString()];

            return directValue;

            ////注意,另一种方法
            //Console.WriteLine(rwData["chkLEDTxt16DI01"]["txt_07"]);   //输出指定的 section 下 key("txt_07") 的值,也可以写入
            //rwData["chkLEDTxt16DI01"]["txt_07"] = "Abc";    //也可以写入
        }
        #endregion


        #region 方法：按文本形式读取(包括注释在内的)全部内容到一个字符串，包含自动换行。
        /// <summary>
        /// 按文本形式读取(包括注释在内的)全部内容到一个字符串，包含自动换行。
        /// </summary>
        public object ReadContentToEndToStr()
        {
            if (path != null && File.Exists(path))
            {
                StreamReader sr = new StreamReader(path);

                string sTemp = sr.ReadToEnd();//读取 ini 文件的全部的内容包括注释
                sr.Close();

                return sTemp;
            }
            else
            {
                throw new Exception("Error");
            }
        }
        #endregion


        #region 方法：1.增加一个 节+键+键值，如果原先存在则覆盖原先的。2.也可以修改一个键值。返回一个 int= 写入成功的值的个数。
        /// <summary>
        /// 1.增加一个 节+键+键值，如果原先存在则覆盖原先的。2.也可以修改一个键值。返回一个 int= 写入成功的值的个数。
        /// </summary>
        /// <param name="newsSectionName">节名，写入成功后返回1</param>
        /// <param name="newKeyName">键名，写入成功后返回2</param>
        /// <param name="newKeyValue">键值，写入成功后返回3</param>
        /// <returns></returns>
        public int AddNewSectionKeyValue(string newsSectionName, string newKeyName, string newKeyValue)
        {
            FileIniDataParser parser = new FileIniDataParser();    //构造一个FileIniDataParser
            IniData data;

            int writeCounter = 0;//记录写入的值的个数

            if (path != null && File.Exists(path))
            {
                //先读取
                data = parser.ReadFile(Path);        //声明一个 inidata 并存储读取的指定路径的 ini 文件
            }
            else
            {
                writeCounter = 0;

                return writeCounter;
            }


            //增加 section  newSectionName 必须不能为 null
            if (newsSectionName != null)
            {
                data.Sections.AddSection(newsSectionName);
                writeCounter = 1;
            }

            //增加 key  newKeyName 必须不能为 null
            if (newKeyName != null)
            {
                data[newsSectionName].AddKey(newKeyName);
                writeCounter = 2;
            }

            //增加 key 的 value  newKeyValue 必须不能为 null
            if (newKeyValue != null)
            {
                //data[newsSectionName].AddKey(newKeyName, newKeyValue);//这个增加/修改不了键值，为什么？bug?
                data[newsSectionName][newKeyName] = newKeyValue;// 可以增加键值
                writeCounter = 3;
            }

            //将 所有数据写入 ini文件
            parser.WriteFile(path, data);

            return writeCounter;
        }
        #endregion


        #region 方法：修改一个键值，修改成功后返回 true,否则返回 false。
        /// <summary>
        /// 修改一个键值，修改成功后返回 true,否则返回 false。
        /// </summary>
        /// <param name="sectionName">指定节名</param>
        /// <param name="keyName">指定键名</param>
        /// <param name="newKeyValue">新的键值</param>
        /// <returns></returns>
        public bool UpdateValueOfAKey(string sectionName, string keyName, string newKeyValue)
        {
            FileIniDataParser parser = new FileIniDataParser();    //构造一个FileIniDataParser
            IniData data;

            if (path != null && File.Exists(path))
            {
                //先读取
                data = parser.ReadFile(Path);        //声明一个 inidata 并存储读取的指定路径的 ini 文件
            }
            else
            {
                return false;
            }

            //判断 sectionName 是否存在
            if (!data.Sections.ContainsSection(sectionName))
            {
                return false;
            }

            KeyDataCollection keyData = data.Sections[sectionName]; //提取 data 内 sectionNanme 节下的 keyData

            if (!keyData.ContainsKey(keyName))//判断 keyName 是否存在
            {
                return false;
            }

            //增加 key 的 value  newKeyValue 必须不能为 null
            if (newKeyValue != null)
            {
                //data[newsSectionName].AddKey(newKeyName, newKeyValue);//这个增加/修改不了键值，为什么？bug?
                data[sectionName][keyName] = newKeyValue;// 可以修改键值
            }

            //将 所有数据写入 ini文件
            parser.WriteFile(path, data);

            return true;
        }
        #endregion


        #region 方法：从文件中删除一个节的内容，包括节名和其下所有键和注释
        /// <summary>
        /// 从文件中删除一个节的内容，包括节名和其下所有键和注释
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public bool DeleteSection(string sectionName)
        {
            FileIniDataParser parser = new FileIniDataParser();    //构造一个FileIniDataParser
            IniData data;

            if (path != null && File.Exists(path))
            {
                //先读取
                data = parser.ReadFile(Path);        //声明一个 inidata 并存储读取的指定路径的 ini 文件
            }
            else
            {
                return false;
            }

            //从文件中删除“sectionName”部分以及与之关联的所有键和注释,节名也会被删除
            data.Sections.RemoveSection(sectionName);
            //data.Sections[sectionName].RemoveAllKeys();//删除节下的内容但是节名不会被删除。

            //将 所有数据写入 ini文件
            parser.WriteFile(path, data);

            return true;
        }
        #endregion


        #region 方法，删除节下的一对键和键值，返回bool
        /// <summary>
        /// 删除节下的一对键和键值，返回bool
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public bool DeleteValueAndKey(string sectionName, string keyName)
        {
            FileIniDataParser parser = new FileIniDataParser();    //构造一个FileIniDataParser
            IniData data;

            if (path != null && File.Exists(path))
            {
                //先读取
                data = parser.ReadFile(Path);        //声明一个 inidata 并存储读取的指定路径的 ini 文件
            }
            else
            {
                return false;
            }

            //判断 sectionName 是否存在
            if (!data.Sections.ContainsSection(sectionName))
            {
                return false;
            }

            KeyDataCollection keyData = data.Sections[sectionName]; //提取 data 内 sectionNanme 节下的 keyData

            if (!keyData.ContainsKey(keyName))//判断 keyName 是否存在
            {
                return false;
            }


            data.Sections[sectionName].RemoveKey(keyName);
            //将 所有数据写入 ini文件
            parser.WriteFile(path, data);

            return true;
        }
        #endregion


        #region 属性：路径

        string path = "D:\\aa00\\UnName\\UnName.ini";  //声明默认路径

        /// <summary>
        /// ini文件的路径。例： string path = "D:\\aa00\\UnName\\UnName.ini";
        /// </summary>
        public string Path { get => path; set => path = value; }  // 为了使用这个类时可以多次设置路径，将这个字段以属性的形式暴露出来
        #endregion


    }
}
