using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using Microsoft.CSharp;
using Newtonsoft.Json;

namespace AIRBox
{
    /// <summary>
    /// 助手类
    /// </summary>
    public static class Helper
    {
        public static readonly DateTime DateTimeMinValue = new DateTime(1903, 1, 1, 0, 0, 0, 0);

        static string CodeNameReplaceRegx = @"([^\u4e00-\u9fa5a-zA-z0-9].*?)";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns>
        /// the Words Before Register,
        /// Register,
        /// DateTime,
        /// M Flag,
        /// Wgl Flag,
        /// Zip Flag,
        /// </returns>
        public static Tuple<string,string,string,string,string,string> GetInfoFromQarName(string path)
        {
            var ret = string.Empty;
            var m =
                new Regex(
                    @"(?<F>[A-Za-z0-9_\\. #:]+)?(?<S>[A-Za-z]-?[A-Za-z0-9]{1,5})_(?<D>\d{14})[_]?(?<M>[Mm]?)[.]?(?<W>[wgl]{0,3})[.]?(?<Z>[zip]{0,3})",
                    RegexOptions.IgnoreCase).Match(path);
            if (m.Success)
            {
                return new Tuple<string, string, string, string, string,string>(
                    m.Groups["F"].Value,
                    m.Groups["S"].Value,
                    m.Groups["D"].Value,
                    m.Groups["M"].Value,
                    m.Groups["W"].Value,
                    m.Groups["Z"].Value
                    );
                //F = m.Groups["F"].Value;
                //S = m.Groups["S"].Value;
                //D = m.Groups["D"].Value;
                //M = m.Groups["M"].Value;
                //Z = m.Groups["Z"].Value;
            }
            return null;
        }
        /// <summary>
        /// 字符串转为数字；如果失败则返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int TryGetInt(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            int i;
            int.TryParse(s.Trim(), out i);
            return i;
        }

        /// <summary>
        /// 字符串转为Hex；如果失败则返回0
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int TryGetHex(string s)
        {
            var i = 0;
            try
            {
                i = Convert.ToInt32(s.Trim(), 16);
            }
            catch
            {
                // ignored
            }
            return i;
        }
        /// <summary>
        /// 字符串转为Double；如果失败则返回0d
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double TryGetDouble(string s)
        {
            double d;
            double.TryParse(s.Trim(), out d);
            return d;
        }
        
        /// <summary>
        /// 将父类转化成子类,并赋值
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static TOut ToChild<TOut>(this object parent) where TOut : new()
        {
            var t = new TOut();
            var proplist = parent.GetType().GetProperties();
            foreach (var p in proplist.Where(p => p.CanRead && p.CanWrite))
            //foreach (var p in proplist.Where(p => p.CanRead))
            {
                if (t.GetType().GetProperty(p.Name) != null)
                {
                    p.SetValue(t, p.GetValue(parent, null), null);
                }
                else
                {
                    //LogHelper.Warn("{0}.{1} NOT Exist.", nameof(t),p.Name);
                }
            }
            return t;
        }
 

        ///// <summary>
        ///// 从dynamic对象中获取DateTime
        ///// </summary>
        ///// <param name="d"></param>
        ///// <returns></returns>
        //public static DateTime? Dynamic2DateTime(dynamic d)
        //{
        //    if (d == null)
        //    {
        //        return null;
        //    }
        //    return d < Global.DateTimeMinValue ? Global.DateTimeMinValue : d;
        //}

        /// <summary>
        /// 将字符串转化为Enum
        /// </summary>
        /// <typeparam name="T">Enumerate</typeparam>
        /// <param name="value">字符串值</param>
        /// <returns></returns>
        public static dynamic GetEnum<T>(string value)
            where T : struct
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }
            T t;
            Enum.TryParse(value, true, out t);
            return t;
        }

        /// <summary>
        /// 将Int转化Enum对象
        /// 如果输入为空，则输出为空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        public static dynamic GetEnum<T>(int? code)
            where T : struct
        {
            if (code == null)
            {
                return null;
            }
            else
            {
                var i = (int)code;
                return (T)Enum.ToObject(typeof(T), i);
            }
        }

        /// <summary>
        /// 获取文件的MD5值
        /// </summary>
        /// <param name="bytes">文件的字节数组</param>
        /// <returns>无连字符的MD5值</returns>
        public static string ComputeMd5(byte[] bytes)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(bytes)).Replace("-", "");
        }
        
        /// <summary>
        /// 计算文件的Hash代码
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ComputeFileHash(string filePath)
        {
            return ComputeMd5(File.ReadAllBytes(filePath));
        }

        /// <summary>
        /// 获取字符串的MD5值
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns>无连字符的MD5值</returns>
        public static string ComputeMd5(string s)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", "");
        }
        

        /// <summary>
        /// 以当前UTC时间为参数，获取一个51个字符长度的唯一ID，形如ticks + guid.ToUpper()
        /// </summary>
        /// <returns></returns>
        public static string CreateId()
        {
            return DateTime.UtcNow.Ticks.ToString().PadLeft(19, '0') + Guid.NewGuid().ToString("N").ToUpper();
        }


        /// <summary>
        /// 判断一个Double是否是2的n次方
        /// </summary>
        /// <param name="d">双精度浮点数</param>
        /// <returns></returns>
        public static bool IsPower2(double d)
        {
            // 如果输入非正
            if (!(d > 0))
            {
                return false;
            }

            // 如果输入小于1 则取其倒数
            if (d < 1)
            {
                d = 1 / d;
            }

            // 如果输入非整数
            if (d % 1 != 0)
            {
                return false;
            }

            // 如果超过整形的最大值
            if (d > Int32.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(d) , d.ToString(CultureInfo.InvariantCulture));
            }

            // 将输入转为整数
            var i = Int32.Parse(d.ToString(CultureInfo.CurrentCulture));

            // 与其前一个数字进行与运算 如果为0 则输入为2的N次方，否则输入非2的N次方
            return (i & (i - 1)) == 0;
        }
        /// <summary>
        /// 获取时间
        /// 如果输入所代表的时间小于本系统定义的最小时间，则返回本系统定义的最小时间
        /// </summary>
        /// <param name="dt">输入时间</param>
        /// <returns>输出时间</returns>
        public static DateTime GetDateTime(this DateTime dt)
        {
            if (dt < DateTimeMinValue)
            {
                return DateTimeMinValue;
            }
            else
            {
                return dt;
            }
        }
        /// <summary>
        /// 获取一个整型数字转化为二进制后,"1"的数量
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int GetEffectiveLength(int position)
        {
            int res = 0;

            while (position > 0)
            {
                position &= (position - 1);
                res++;
            }

            return res;
        }

        /// <summary>
        /// 判断给定的字符串是否是一个IP地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIpAddress(string s)
        {
            return !String.IsNullOrEmpty(s) && Regex.IsMatch(s, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 获取整型转化为二进制后 右边“0”的个数
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int GetRightZeroCount(int i)
        {
            var c = 0;
            // 如果一个整型转化为二进制后尾数为0，那么他一定能够被2整除
            while (i % 2 == 0) // 如果该数字能够被2整除
            {
                // 向右位移一位
                i = i >> 1;
                c++;
            }
            return c;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public static object Deserialize(byte[] bytes)
        {
            if (bytes == null || bytes.Count() < 1)
            {
                return null;
            }
            using (var ms = new MemoryStream(bytes))
            {
                var bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// 利用MemoryStream进行深拷贝
        /// </summary>
        /// <typeparam name="T">要拷贝的对象类型</typeparam>
        /// <param name="obj">源对象</param>
        /// <returns></returns>
        public static T DeepCopy<T>(T obj)
        {
            object retval;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);

                ms.Flush();

                ms.Seek(0, SeekOrigin.Begin);
                
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }

        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        /// <param name="removeReadOnly"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool removeReadOnly = false)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);



            var dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);

                if (removeReadOnly)
                {
                    file.IsReadOnly = false;
                }
            }

            if (removeReadOnly)
            {
                dir.Attributes = FileAttributes.Normal;
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (!copySubDirs) return;
            foreach (var subdir in dirs)
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, true);
            }
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static long GetDirectorySize(DirectoryInfo dir)
        {
            return dir.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }
        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static long GetDirectorySize(string folderPath)
        {
            return GetDirectorySize(new DirectoryInfo(folderPath));
        }

        /// <summary>
        /// 获取带文件路径的文件名（如“D:\AML\JHG\abc.txt”，获取abc.txt）
        /// </summary>
        /// <param name="path">文件名</param>
        /// <returns></returns>
        public static string GetFileOrDirName(string path)
        {
            //传进来的 filePath 应 filePath.TrimEnd('\\')
            //该方法也可以用split('\\')，但是建议用 LastIndexOf
            var dirNameIndex = path.LastIndexOf("\\", StringComparison.Ordinal);
            if (dirNameIndex != -1)
            {
                return path.Substring(dirNameIndex + 1);
            }
            throw new ArgumentException(String.Format("Path {0} Invalid.", path), nameof(path));
        }

        /// <summary>
        /// 获取除了扩展名之外的文件名（如“abc.txt”，获取abc）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            int dirNameIndex = path.LastIndexOf(".", StringComparison.Ordinal);
            if (dirNameIndex != -1)
            {
                return path.Substring(0, dirNameIndex);
            }
            throw new ArgumentException(String.Format("Path {0} Invalid.", path), nameof(path));
        }
        /// <summary>
        /// 通过 FileSystemInfo 获取文件或者目录名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileOrDirectoryName(string path)
        {
            return IsDir(path) ? new DirectoryInfo(path).Name : new FileInfo(path).Name;
        }

        /// <summary>
        /// 获取不包含文件路径的文件名（如“D:\AML\JHG\abc.txt”，获取abc.txt）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="includeExt"></param>
        /// <returns></returns>
        public static string GetFileOrDirectoryName(string path, bool includeExt = true)
        {
            var name = string.Empty;
            if (Directory.Exists(path))
            {
                return new DirectoryInfo(path).Name;
            }

            if (!File.Exists(path)) return name;

            var fi = new FileInfo(path);
            name = fi.Name;
            var ext = fi.Extension;

            if (!includeExt)
            {
                name = name.Substring(0, name.Length - ext.Length);
            }
            return name;
        }
        /// <summary>
        /// 移除文件或文件夹的只读属性
        /// </summary>
        /// <param name="path">目标文件、文件夹的路径</param>
        public static void RemoveFileOrDirectoryReadonlyProperty(string path)
        {
            if (File.Exists(path))
            {
                new FileInfo(path).IsReadOnly = false;
                return;
            }
            if (!Directory.Exists(path)) return;

            var di = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };
            foreach (var f in di.EnumerateFileSystemInfos("*", SearchOption.AllDirectories))
            {
                f.Attributes = FileAttributes.Normal;
            }
        }
        ///// <summary>
        ///// 通过 FileSystemInfo 获取文件或者目录名
        ///// </summary>
        ///// <param name="path"></param>
        ///// <returns></returns>
        //public static string GetFileOrDirectoryExtName(string path)
        //{
        //    return IsDir(path) ? new DirectoryInfo(path).Name : new FileInfo(path).Name;
        //}

        /// <summary>
        /// 判断给定地址是否为文件夹
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool IsDir(string filepath)
        {
            var fi = new FileInfo(filepath);
            return (fi.Attributes & FileAttributes.Directory) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ResolveMountPath(string path)
        {
            var mountPath = path.Replace("\\\\", "/").Replace("\\", "/");
            return mountPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ResolveUncPath(string path)
        {
            var oriPath = path.Replace("\\\\", "//").Replace("\\", "/");
            return oriPath;
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="extractPath"></param>
        /// <param name="overWrite"></param>
        public static string Unzip(string zipPath , string extractPath = null,bool overWrite=false)
        {
            if (!zipPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(extractPath))
            {
                extractPath = zipPath.Remove(zipPath.Length - 4);
            }

            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine(extractPath, entry.FullName), overWrite);
                }
            }
            return extractPath;
        }

        /// <summary>
        /// 压缩内存对象或流对象到压缩包
        /// </summary>
        /// <param name="zipEntryName">压缩包中的子文件名</param>
        /// <param name="targetZipFile">压缩包文件名</param>
        /// <param name="doZipContent">回调委托,用于具体实现向压缩包中具体写文件内容的实现</param>
        public static void Zip(string zipEntryName, string targetZipFile,Action<Stream> doZipContent)
        {
            var isExists = File.Exists(targetZipFile);
            
            using (var fs = new FileStream(targetZipFile, FileMode.OpenOrCreate))
            {
                using (var archive = new ZipArchive(fs, isExists?ZipArchiveMode.Update: ZipArchiveMode.Create))
                {
                    var entry = isExists?archive.GetEntry(zipEntryName):archive.CreateEntry(zipEntryName, CompressionLevel.Fastest);
                    using (var entryStream = entry.Open())
                    {
                        doZipContent(entryStream);
                    }
                }
            }
        }

        /// <summary>
        /// 压缩文件夹到目标压缩文件
        /// </summary>
        /// <param name="sourceFolderPath">源文件夹路径</param>
        /// <param name="targetZipFile">目标压缩包文件名</param>
        /// <param name="deleteSource">压缩完成后是否删除源文件夹</param>
        public static void Zip(string sourceFolderPath, string targetZipFile, bool deleteSource = false)
        {
             ZipFile.CreateFromDirectory(sourceFolderPath, targetZipFile);
            if (deleteSource)
            {
                Directory.Delete(sourceFolderPath, true);
            }
        }

        /// <summary>
        /// 是否是文件夹
        /// </summary>
        /// <param name="filepath">路径</param>
        /// <returns>如果路径无法访问，返回空值</returns>
        public static bool? IsDirectory(string filepath)
        {
            // 如果是文件夹
            if (Directory.Exists(filepath))
            {
                return true;
            }
            // 如果是文件
            if (File.Exists(filepath))
            {
                return false;
            }
            // 否则 返回空值
            return null;
        }

        /// <summary>
        /// 开启cmd.exe进程,并执行cmd命令
        /// </summary>
        /// <param name="strCmd"></param>
        public static void RunCmdProcess(string strCmd)
        {
            var fileName = @"cmd.exe";

            if (Environment.OSVersion.Platform == PlatformID.Unix ||
                Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                fileName = @"bash";
            }

            var prc = new Process
            {
                StartInfo =
                    {
                        FileName = fileName,
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = false,
                        CreateNoWindow = false
                    }
            };
            prc.Start();

            prc.StandardInput.WriteLine(strCmd.TrimEnd('&'));
            prc.StandardInput.Close();
            prc.WaitForExit();
        }
        

        ///// <summary>
        ///// 异步事件触发
        ///// </summary>
        ///// <param name="handler"></param>
        ///// <param name="sender"></param>
        ///// <param name="args"></param>
        ///// <param name="callback"></param>
        ///// <param name="object"></param>
        //public static List<IAsyncResult> AsyncInvoke<T>(EventHandler<T> handler, object sender, T args , AsyncCallback callback = null, object @object = null)
        //{
        //    var results = new List<IAsyncResult>();
        //    var invocationList = handler?.GetInvocationList();
        //    if (invocationList?.Length > 0)
        //    {
        //        results.AddRange(invocationList.Cast<EventHandler<T>>().Select(invocat =>
        //        {
        //          return invocat?.BeginInvoke(sender, args, callback, @object);
        //        }));
        //    }
        //    return results;
        //}

        /// <summary>
        /// 异步事件触发
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void AsyncInvoke<T>(this EventHandler<T> handler, object sender, T args)
        {
            //var results = new List<IAsyncResult>();
            var invocationList = handler?.GetInvocationList();
            if (invocationList?.Length > 0)
            {
                invocationList.Cast<EventHandler<T>>().Select(invocat =>
                 {
                    //                    return invocat?.BeginInvoke(sender, args, null, null);
                    return Task.Run(() =>
                    {
                        invocat?.Invoke(sender, args); 
                    });
                 });
            }
            //return results;
        }

        /// <summary>
        /// 异步事件触发
        /// </summary>
        /// <param name="action"></param>
        /// <param name="args"></param>
        public static void AsyncInvoke<T>(this Action<T> action,  T args)
        {
            //var results = new List<IAsyncResult>();
            var invocationList = action?.GetInvocationList();
            if (invocationList?.Length > 0)
            {
                invocationList.Cast<Action<T>>().Select(invocat =>
                {
                    //                    return invocat?.BeginInvoke(sender, args, null, null);
                    return Task.Run(() =>
                    {
                        invocat?.Invoke(args);
                    });
                });
            }
            //return results;
        }

        /// <summary>
        /// 获取一个字符串数组内所有字符串共有的起始字符以及结束字符
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        public static string GetCommonWith(IEnumerable<string> stringArray)
        {
            var enumerable = stringArray as IList<string> ?? stringArray.ToList();
            if (!enumerable.Any())
            {
                return string.Empty;
            }
            var sFirstString = enumerable.FirstOrDefault();
            if (string.IsNullOrEmpty(sFirstString))
            {
                return string.Empty;
            }
            var index = 0;
            while (true)
            {
                if (index >= sFirstString.Length)
                {
                    break;
                }
                var sFirstStart = sFirstString.Substring(0, index + 1);
                var isStartWith =
                    enumerable.All(s => s.StartsWith(sFirstStart, StringComparison.InvariantCultureIgnoreCase));
                if (isStartWith)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            var endIndex = 0;
            while (true)
            {
                if (endIndex >= sFirstString.Length)
                {
                    break;
                }
                var sFirstEnd = sFirstString.Substring(sFirstString.Length - endIndex - 1, endIndex + 1);
                var isEndWith = enumerable.All(s => s.EndsWith(sFirstEnd, StringComparison.InvariantCultureIgnoreCase));
                if (isEndWith)
                {
                    endIndex++;
                }
                else
                {
                    break;
                }
            }
            return sFirstString.Substring(0, index) + sFirstString.Substring(sFirstString.Length - endIndex, endIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FormatCodeName(string name)
        {
            // replace all non programing charactor to iso92001 char

            //1. replace < to 'less' , > to 'more'
            //2. replace other char and space to '_'
            //3. add prefix 'a_' if start with number char or '_'.
            //4. toupper

            var vname = name.Trim();

            //1
            vname = vname.Replace("<", "less").Replace(">","more");

            // 2

            vname =Regex.Replace(vname, CodeNameReplaceRegx, "_");

            // 3
            var firstChar = vname.FirstOrDefault();
            if (char.IsNumber(firstChar))
            {
                vname = "a_"+vname;
            }

            //4
            return vname.Trim().ToUpper();
        }
    }
}
