using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace A3_Lib
{
    public class Main
    {
        private const string Version = "1.0.0";
        private static Main _main;
        
         #region Arma3  (Version 1.61) - RVExtension The first stable version of this functionality arrived.
#if IS_x64
        [DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
#endif
        public static string RvExtension(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function)
        {
            //outputSize--;
            try
            {
                if (_main == null) _main = new Main();
                output.Append(Invoke(function));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "[Exception]";
        }
        #endregion

        #region Arma3  (Version 1.68) - RVExtensionArgs The ability to pass multiple arguments to callExtension and the interface RVExtensionArgs got added.
#if IS_x64
        [DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionArgs@20", CallingConvention = CallingConvention.Winapi)]
#endif
        public static string RvExtensionArgs(StringBuilder output, int outputSize, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)] string[] args, int argCount)
        {
            //outputSize--;
            try
            {
                if (_main == null) return "[NotInitialized]";
                output.Append(Invoke(function, args));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return "[Exception]";
        }
        #endregion

        #region Arma3  (Version 1.70) - RVExtensionVersion became available. It is called when the extension loads. Will be printed into the RPT log.
#if IS_x64
        [DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionVersion@8", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtensionVersion(StringBuilder output, int outputSize)
        {
            //outputSize--;
            output.Append(Version);
        }
        #endregion

        #region Arma3  (Version 2.00) - RVExtensionRegisterCallback became available. The abilty to register a callback function was added.
#if IS_x64
        [DllExport("RVExtensionRegisterCallback", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionRegisterCallback@4", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtensionRegisterCallback([MarshalAs(UnmanagedType.FunctionPtr)] ExtensionCallback func) => _callback = func;
        public delegate string ExtensionCallback([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string data);

        private static ExtensionCallback _callback;
        #endregion

        #region Arma3  (Version 2.11) - RVExtensionContext became available. Passes steamID, fileSource, missionName, serverName
#if IS_x64
        [DllExport("RVExtensionContext", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionContext@4", CallingConvention = CallingConvention.Winapi)]
# endif
        public static void RvExtensionContext([MarshalAs(UnmanagedType.LPStr)] string args, int argsCnt)
        {
            var context = new ExtensionContext(args);
            Console.WriteLine(context.ToString());
        }

        private class ExtensionContext
        {
            private string SteamId { get; set; }
            private string FileSource { get; set; }
            private string MissionName { get; set; }
            private string ServerName { get; set; }

            public ExtensionContext(string args)
            {
                var split = args.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 4) return;
                SteamId = split[0];
                FileSource = split[1];
                MissionName = split[2];
                ServerName = split[3];
            }

            public override string ToString() => $"SteamID: {SteamId}, FileSource: {FileSource}, MissionName: {MissionName}, ServerName: {ServerName}";
        }
        #endregion
        
        // "a3_lib" callExtension "init";
        private static string Invoke(string method)
        {
            switch (method)
            {
                case "init": 
                    return (_main == null ? "[Initialized]": "[Failure]");
                default:
                    return "[InvalidMethod]";
            } 
        }

        // "a3_lib" callExtension ["test", ["arg1", "arg2", "arg3"]];
        private static string Invoke(string method, string[] args)
        {
            switch (method)
            {
                case "test":
                    Console.WriteLine("Test method called with args: " + string.Join(", ", args));
                    return "[Success]";
                case "callback": 
                    
                    _callback.Invoke("a3_lib", "run", "['MPServer_fnc_test', [true,1,'test string',['another string in an another array']]]");
                    return "[Success]";
                default:
                    return "[InvalidMethod]";
            }
        }
 
    }
}