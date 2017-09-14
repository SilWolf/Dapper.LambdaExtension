using System;
using System.Collections.Generic;
using System.Linq;
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
#endif
        }

      
        public static bool IsNetFX
        {
            get { return _isNetFx; }
        }
    }
}
