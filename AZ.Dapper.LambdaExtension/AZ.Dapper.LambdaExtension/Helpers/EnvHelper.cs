using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#if NETCOREAPP1_0 || NETSTANDARD1_6
using Microsoft.Extensions.PlatformAbstractions;
#endif

namespace Dapper.LambdaExtension.Helpers
{
    public static class EnvHelper
    {
        static bool _isNetFx;

        static EnvHelper()
        {
#if NETCOREAPP1_0 || NETSTANDARD1_6
            var runtimeInfo = PlatformServices.Default.Application.RuntimeFramework;

            //runtimeInfo.Identifier; //.NETFramework // .NETCoreApp
            _isNetFx= runtimeInfo.Identifier.ToLower().Contains(".NETFramework".ToLower());
#else
            _isNetFx = true;
#endif
        }

      
        public static bool IsNetFX
        {
            get { return _isNetFx; }
        }

        private static Random _randSeed;
        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="len">字符串长度</param>
        /// <returns></returns>
        public static string GetRandomString(int len, bool useNum = true, bool useLow = true, bool useUpp = true, bool useSpecial = false)
        {
            if (_randSeed == null)
            {
                _randSeed = new Random(GetRandomSeed());
            }

            string s = "";//"123456789abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ";

            if (useNum)
            {
                s += "1234567890";
            }
            if (useLow)
            {
                s += "abcdefghijklmnopqrstuvwxyz";
            }
            if (useUpp)
            {
                s += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }
            if (useSpecial)
            {
                s += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            }

            string reValue = string.Empty;

            while (reValue.Length < len)
            {
                var s1 = s[_randSeed.Next(0, s.Length)].ToString();
                if (reValue.IndexOf(s1, StringComparison.Ordinal) == -1) reValue += s1;
            }
            return reValue;
        }

        /// <summary>
        /// 生成真随机种子
        /// </summary>
        /// <returns></returns>
        public static int GetRandomSeed()
        {
#if NETCOREAPP1_0 || NETSTANDARD1_6

            using (var rdm = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[4];
                rdm.GetBytes(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
#else
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);

#endif

        }
    }
}
