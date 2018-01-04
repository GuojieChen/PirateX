using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using PirateX.Core.Config;
using ExcelExporter;
using Excel;

namespace PirateX.ConfigImport
{
    public class ImportService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string _connectionString;
        private string _inputDir;
        private string _tempDir;
        private int _maxWorker;
        private bool isRunning = false;

        private NameValueCollection _ignore; 

        public ImportService(string inputDir, int maxWorker,NameValueCollection ignore,string connectionString)
        {
            if(string.IsNullOrEmpty(connectionString))
                _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; 
            else
                _connectionString = connectionString;

            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentNullException("ConnectionString");

            _inputDir = inputDir;
            _tempDir = string.Format("{0}\\temp", System.Environment.CurrentDirectory);
            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);

            _ignore = ignore; 
            if(_ignore == null)
                _ignore = new NameValueCollection();

            _maxWorker = maxWorker;
        }


        private Queue<WorkQueueItem> QueueItems = new Queue<WorkQueueItem>();
        private bool _hasErrors;

        public void Start(params string[] files)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(string.Format("TEMP:{0}", _tempDir));
            }

            var fileNames = GetFiles2Import(Directory.GetFiles(_inputDir).Select(Path.GetFileName));

            var types = new List<Type>();

            foreach (var file in files)
            {
                Assembly assembly = Assembly.LoadFrom(file);
                var list = assembly.GetTypes()
                .Where(item =>
                typeof(IConfigEntity) != item
                && typeof(IConfigEntity).IsAssignableFrom(item)
                && item.GetCustomAttributes(typeof(ExcelNameAttribute), false).Any()
                && fileNames.Contains(((ExcelNameAttribute)(item.GetCustomAttributes(typeof(ExcelNameAttribute), false)[0])).Name)
                );

                types.AddRange(list);
            }

            if (Logger.IsDebugEnabled)
                Logger.Debug($"types.cuont:{types.Count}");

            progressBar1.Maximum = types.Count();

            if (progressBar1.Maximum <= 0)
                return;

            //progressBar1.Value = fileNames.Count - types.Count();
            var typesQueue = new Queue<Type>();

            foreach (var type in types)
                typesQueue.Enqueue(type);

            // 创建临时文件，添加到工作队列中

            isRunning = true; 
            var task = Task.Factory.StartNew(Worker);
            Task.Factory.StartNew(() =>
            {
                while (typesQueue.Any())
                {
                    var type = typesQueue.Dequeue();

                    try
                    {
                        if (type.IsClass)
                        {
                            if (typeof(IConfigEntity).IsAssignableFrom(type))
                            {
                                var excelNameAttributes = type.GetCustomAttributes(typeof(ExcelNameAttribute), false);
                                if (!excelNameAttributes.Any())
                                    return;

                                var attr = (ExcelNameAttribute)excelNameAttributes[0];

                                if (!fileNames.Contains(attr.Name))
                                    return;

                                if (Logger.IsDebugEnabled)
                                    Logger.Debug(string.Format("SAVE\tS\t{0}", attr.Name));

                                var fileName = string.Format("{0}\\{1}", _inputDir, attr.Name);
                                var filename = SaveTempFile(fileName);

                                if (filename != null)
                                    QueueItems.Enqueue(new WorkQueueItem { TypeName = attr.Name, T = type, TempFileName = filename });

                                if (Logger.IsDebugEnabled)
                                    Logger.Debug(string.Format("SAVE\tE\t{0}", attr.Name));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Logger.IsErrorEnabled)
                            Logger.Error(ex);
                    }
                }
            });

            Task.WaitAll(task);
        }

        public void Stop()
        {
            isRunning = false; 
        }

        private IList<string> GetFiles2Import(IEnumerable<string> files)
        {
            IDbConnectionFactory connFactory = new OrmLiteConnectionFactory(_connectionString, MySqlDialect.Provider);
            //SqlServerOrmLiteDialectProvider.Instance.UseUnicode = true;

            IList<string> results = new List<string>();

            using (var db = connFactory.OpenDbConnection())
            {
                db.CreateTableIfNotExists<FileHash>();

                foreach (var file in files)
                {
                    var hash = GetMD5HashFromFile(string.Format("{0}{1}{2}",_inputDir,Path.DirectorySeparatorChar,file));

                    var filename = file;

                    var filehash = db.SingleById<FileHash>(filename);
                    if (filehash != null && Equals(filehash.Hash ,hash))
                        continue;

                    results.Add(file);
                }
                
            }

            return results; 
        }

        protected string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary> 导入excel数据到数据库中
        /// </summary>
        /// <returns></returns>
        private void ExcelToDatabase<T>(string originFileName, string tempFileName)
        {
            //if (Logger.IsDebugEnabled)
            //    Logger.Debug(string.Format("import\tS\t{0}", originFileName));

            string fileType = System.IO.Path.GetExtension(tempFileName);

            string connStr = null;

            if (fileType == ".xls")
                connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + tempFileName + ";" + "Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            else //临时文件都是 xls格式
                connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + tempFileName + ";" + "Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
    
            //链接文件
            using (var cnnxls = new OleDbConnection(connStr))
            {
                try
                {
                    cnnxls.Open();

                    var cmd = new OleDbCommand();
                    cmd.Connection = cnnxls;

                    // Get all Sheets in Excel File
                    var dtSheet = cnnxls.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    foreach (DataRow dr in dtSheet.Rows)
                    {
                        string sheetName = dr["TABLE_NAME"].ToString();

                        if (!sheetName.EndsWith("$"))
                            continue;

                        // Get all rows from the Sheet
                        cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                        var list = cmd.ConvertToList<T>();

                        IDictionary<string, IList<string>> dic = new Dictionary<string, IList<string>>();

                        var fileName = System.IO.Path.GetFileName(originFileName);
                        try
                        {
                            //TODO 检查复合模型
                            foreach (var item in list)
                            {
                                IList<string> names = new List<string>();

                                foreach (var propertyInfo in item.GetType().GetProperties())
                                {
                                    if (!(propertyInfo.PropertyType.IsPrimitive || typeof(string) == propertyInfo.PropertyType))
                                    {//字段是复合类型

                                        var val = propertyInfo.GetValue(item, null);
                                        if (val == null && _ignore[fileName] != null)
                                        {//填写不对了
                                            //if (Logger.IsWarnEnabled)
                                            //    Logger.Warn(typeof(T).Name + ":" + propertyInfo.Name);

                                            var ss = _ignore[fileName].Split(',');
                                            if(!ss.Contains(propertyInfo.Name))
                                                names.Add(propertyInfo.Name);
                                        }
                                    }
                                }

                                if (names.Count > 0 && !dic.ContainsKey(fileName))
                                    dic.Add(fileName, names);
                            }
                            foreach (var item in dic)
                            {
                                if(Logger.IsWarnEnabled)
                                    Logger.Warn("Field Null ,Check! -> " + item.Key + ":" + string.Join(",", item.Value));
                            }
                        }
                        catch (Exception exception)
                        {
                            if (Logger.IsErrorEnabled)
                                Logger.Error(exception);
                        }

                        IDbConnectionFactory connFactory = new OrmLiteConnectionFactory(_connectionString, MySqlDialect.Provider);
                        //SqlServerOrmLiteDialectProvider.Instance.UseUnicode = true; 
                      
                        using (var db = connFactory.OpenDbConnection())
                        {
                            //using (var trans = db.OpenTransaction())
                            {
                                try
                                {

                                    db.DropAndCreateTable<T>();

                                    var len = list.Count;
                                    var count = 500;
                                    var page = len % count == 0 ? len / count : (len / count) + 1;
                                    for (int i = 0; i < page; i++)
                                        db.InsertAll<T>(list.Skip(i * count).Take(count));

                                    var hash = GetMD5HashFromFile(string.Format("{0}{1}{2}", _inputDir, Path.DirectorySeparatorChar, originFileName)); 

                                    if (db.Exists<FileHash>("Id=@Id", new {Id = Path.GetFileName(originFileName)}))
                                    {
                                        db.Update<FileHash>(new { Hash = hash, UpdateAtUtc = DateTime.UtcNow },
                                            item => item.Id == Path.GetFileName(originFileName));
                                    }
                                    else
                                    {
                                        db.Insert<FileHash>(new FileHash()
                                        {
                                            FileName = Path.GetFileName(originFileName),
                                            Hash = hash,
                                            UpdateAtUtc = DateTime.UtcNow
                                        }); 
                                    }
                                }
                                catch (Exception exception)
                                {
                                    if (Logger.IsErrorEnabled)
                                    {
                                        Logger.Error(string.Format("{0} in {1}", sheetName, originFileName));
                                        Logger.Error(exception.Message);
                                    }

                                    _hasErrors = true;
                                }
                                finally
                                {
                                    progressBar1.Value += 1;
                                }
                            }
                        }
                    }

                    cnnxls.Close();
                }
                catch (Exception exception)
                {
                    if (Logger.IsErrorEnabled)
                        Logger.Error(exception.Message);
                }


                //if (Logger.IsDebugEnabled)
                //    Logger.Debug(string.Format("import\tE\t{0}", originFileName));
            }
        }

        /// <summary> 读取数据源 保存为可转换的临时文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string SaveTempFile(string filename)
        {
            if (!File.Exists(filename))
                return null;

            try
            {
                using (var stream = File.Open(filename, FileMode.Open, FileAccess.Read))
                {
                    string fileType = System.IO.Path.GetExtension(filename);

                    //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                    IExcelDataReader excelReader = null;

                    if (Equals(fileType, ".xls"))
                        excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    else //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                        excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    var dataset = excelReader.AsDataSet();

                    var excelexport = new ExcelExport();

                    if (dataset.Tables.Count == 0)
                        return null;

                    var table = dataset.Tables[0];

                    var tempTable = new DataTable(table.TableName);

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        if (i == 0)
                            continue;
                        try
                        {
                            if (i == 1)
                            {
                                for (int j = 0; j < table.Rows[1].ItemArray.Count(); j++)
                                {
                                    tempTable.Columns.Add(table.Rows[1].ItemArray[j].ToString());
                                }
                            }
                            else
                            {
                                var newrow = tempTable.NewRow();
                                newrow.ItemArray = table.Rows[i].ItemArray;

                                tempTable.Rows.Add(newrow);
                            }
                        }
                        catch (Exception exception)
                        {
                            if (Logger.IsErrorEnabled)
                                Logger.Error(string.Format("{0} - {1}", exception.Message, table.Rows[i]));
                        }
                    }

                    excelexport.AddSheet(tempTable);

                    var fileName = System.IO.Path.GetFileName(filename);

                    var tempFileName = string.Format("{0}\\{1}-{2}.xls", _tempDir, fileName.Substring(0, fileName.IndexOf('.')), DateTime.Now.Ticks);
                    excelexport.ExportTo(tempFileName);
                    return tempFileName;
                }
            }
            catch (Exception exception)
            {
                if (Logger.IsErrorEnabled)
                {
                    Logger.Error(filename, exception); 
                }
            }

            return null;
        }

        private ProgressBar progressBar1 = new ProgressBar();
        private void Worker()
        {
            while (isRunning)
            {
                if (QueueItems.Count>0)
                {
                    var item = QueueItems.Dequeue();

                    if (item == null)
                        Thread.Sleep(200);

                    Task.Factory.StartNew(() =>
                    {
                        if (item == null)
                            return;

                        try
                        {
                            if (!string.IsNullOrEmpty(item.TempFileName))
                            {
                                this.GetType()
                                    .GetMethod("ExcelToDatabase", BindingFlags.Instance | BindingFlags.NonPublic)
                                    .MakeGenericMethod(item.T)
                                    .Invoke(this, new[] {item.TypeName, item.TempFileName});

                                File.Delete(item.TempFileName);


                                if (Logger.IsInfoEnabled)
                                    Logger.Info(string.Format("IMPORT\t{0}({2}/{3})\t{1}", "SUCCESS", item.TypeName,progressBar1.Value, progressBar1.Maximum));
                            }
                        }
                        catch (Exception exception)
                        {

                            if (Logger.IsInfoEnabled)
                                Logger.Info(string.Format("IMPORT\t{0}}({2}/{3})\t{1}", "FAILED", item.TypeName, progressBar1.Value, progressBar1.Maximum));

                            if (Logger.IsErrorEnabled)
                                Logger.Error(exception);
                        }
                        finally
                        {
                            if (progressBar1.Value == progressBar1.Maximum)
                            {
                                if (_hasErrors)
                                {
                                    if (Logger.IsErrorEnabled)
                                        Logger.Error("导入完成,但期间发生点小问题");
                                    //System.Diagnostics.Process.Start("Explorer.exe", string.Format("{0}logs{1}errors", System.AppDomain.CurrentDomain.BaseDirectory, Path.DirectorySeparatorChar));
                                }
                                else
                                {
                                    if (Logger.IsInfoEnabled)
                                        Logger.Info("import ok!!");
                                }
                                isRunning = false;
                                progressBar1.Value = 0;

                                progressBar1.Dispose();
                            }
                        }
                    });
                }
            }
        }
    }

    class WorkQueueItem
    {
        public string TypeName { get; set; }

        public Type T { get; set; }

        public string TempFileName { get; set; }

        public string Hash { get; set; }
    }
}
