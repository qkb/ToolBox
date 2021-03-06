﻿using System;
using ToolBox.Http;
using ToolBox.DateTimeTool;
using ToolBox.Log;
using System.Threading.Tasks;
using ToolBox.Socket;
using System.IO;
using ToolBox.UserAgentParse;
using System.Collections.Generic;

namespace ToolBox.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> aa = new List<string>();

            using (StreamReader sr = new StreamReader(@"C:\Users\Administrator\Desktop\tt.txt"))
            {
                var txt = sr.ReadLine();
                var count = 1;
                while (!string.IsNullOrEmpty(txt))
                {
                    var ua = new UaUnit(txt).Parse();
                    Console.WriteLine($"{count}.浏览器内核:{ua.BrowserKernel}\r\n浏览器名称:{ua.BrowserName}\r\n浏览器版本:{ua.BrowserVersion}\r\n手机型号:{ua.PhoneModelName}\r\n手机型号代码：{ua.PhoneModelCode}\r\n平台：{ua.Platform}\r\n操作系统：{ua.SystemName}\r\n操作系统版本:{ua.SystemVersion}\r\n");
                    count++;
                    txt = sr.ReadLine();
                }
            }
            using (StreamWriter sw = new StreamWriter(@"C:\Users\Administrator\Desktop\tt1.txt"))
            {
                foreach (var item in aa)
                {
                    sw.WriteLine(item);
                }
            }
            Console.WriteLine("wanshier");
            Console.ReadKey();
        }

        private static void TcpServer_OnRecMessage(object sender, SocketMsgArgs e)
        {
            Console.WriteLine($"接收到{e.clientRecInfo.ip} + {e.clientRecInfo.msg}");


        }

        private static void TcpServer_OnClientClose(object sender, SocketArgs e)
        {
            Console.WriteLine($"退出了{e.ClientInfo.id} + {e.ClientInfo.ip}");
        }

        private static void TcpServer_OnClientAdd(object sender, SocketArgs e)
        {
            Console.WriteLine($"添加了{e.ClientInfo.id} + {e.ClientInfo.ip}");
        }

        public enum test
        {
            test1,
            test2
        }

        private static void RunTask()
        {
            Task.Run(() =>
            {
                "test1".WriteErrorLog();
                "test2".WriteErrorLog();
                "test3".WriteErrorLog();
                "test4".WriteErrorLog();
                "test5".WriteErrorLog();
                "test6".WriteErrorLog();
                "test7".WriteErrorLog();
                "test8".WriteErrorLog();
                "test9".WriteErrorLog();
                "test10".WriteErrorLog();
                "test11".WriteErrorLog();
                "test1".WriteDebugLog();
                "test2".WriteDebugLog();
                "test3".WriteDebugLog();
                "test4".WriteDebugLog();
                "test5".WriteDebugLog();
                "test6".WriteDebugLog();
                "test7".WriteDebugLog();
                "test8".WriteDebugLog();
                "test9".WriteDebugLog();
                "test10".WriteDebugLog();
                "test11".WriteDebugLog();
            });
        }
    }
}
