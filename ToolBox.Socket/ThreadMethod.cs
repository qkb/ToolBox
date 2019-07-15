﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace ToolBox.Socket
{
    public partial class TcpServer
    {

        //这里的方法都是线程安全的


        /// <summary>
        /// 添加一个新的客户端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="socket"></param>
        /// <param name="thread"></param>
        private void AddSocketClient(string ip, Socket socket, Thread thread)
        {
            lockSlim.EnterWriteLock();
            try
            {
                int count = dictsocket.Count;         //目前的客户端数量
                string id = "0";

                if (count >= 1)
                {
                    for (int i = 0; i < count + 1; i++)
                    {
                        //判断是否有当前id
                        var key = dictsocket.Where(q => (q.Value.id.Equals(i.ToString()))).Select(q => q.Key);

                        //   Console.WriteLine("------"+key.Count()+"  ");
                        if (key.Count() == 1) {continue; }
                        else
                        {

                            //是否有当前id号
                            id = i.ToString();
                            ClientMode socketClient = new ClientMode(ip, thread, socket, id);
                            dictsocket.Add(ip, socketClient);
                            Console.WriteLine("添加了id号：" + id.ToString());
                            break;

                        }
                    }
                }
                else
                {

                    ClientMode socketClient = new ClientMode(ip, thread, socket, id);
                    dictsocket.Add(ip, socketClient);
                    Console.WriteLine("首次的添加了id号：" + id.ToString());

                }
            }
            catch (Exception ex)
            {

                HandelExceptionLog?.BeginInvoke("添加客户端时的错误：" + ex, null, null);
               
            }
            finally
            {
                lockSlim.ExitWriteLock();
            }


        }


        /// <summary>
        /// 移除一个客户端
        /// </summary>
        /// <param name="ip"></param>
        private void ReMoveSocketClient(string ip)
        {
            lockSlim.EnterWriteLock();
            try
            {
                if (dictsocket.ContainsKey(ip))
                {
                    string id = dictsocket[ip].id;
                    dictsocket.Remove(ip);

                    Console.WriteLine("移除了{0}号用户，ip地址为{1}", id, ip);

                    
                }

            }
            catch (Exception ex)
            {

                HandelExceptionLog?.BeginInvoke("移除一个客户端时发生的错误：" + ex, null, null);
            }
            finally
            {

                lockSlim.ExitWriteLock();
            }


        }

        /// <summary>
        /// 得到当前总人数
        /// </summary>
        /// <returns></returns>
        public int GetClientCount()
        {

            lockSlim.EnterReadLock();

            try
            {
                return dictsocket.Count;
            }
            catch (Exception ex)
            {
                HandelExceptionLog?.BeginInvoke("得到当前总人数时出错" + ex, null, null);
                return -1;
            }
            finally
            {
                lockSlim.ExitReadLock();
            }
        }

        /// <summary>
        /// 得到客户端的信息
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllClientInfo()
        {

            lockSlim.EnterReadLock();

            int count = dictsocket.Count();
            List<string> list = new List<string>();

            try
            {
                foreach (var item in dictsocket)
                {
                    list.Add(item.Value.id + " - " + item.Key);
                }

                return list;
            }
            catch (Exception ex)
            {
                
                HandelExceptionLog?.BeginInvoke("得到当前客户端信息时出错：" + ex, null, null);

                list.Add("空");
                return list;

            }
            finally
            {

                lockSlim.ExitReadLock();
            }

        }


        /// <summary>
        /// 以送信息给所有客户端
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsgToAllClient(string msg)
        {

            lockSlim.EnterReadLock();

            try
            {
                foreach (var item in dictsocket)
                {
                    if (SocketTools.IsSocketConnected(item.Value.socket))
                    {
                        item.Value.socket.Send(SocketTools.GetBytes(msg));
                    }
                    else
                    {
                        item.Value.socket.Close();
                        Console.WriteLine("IP已经退出" + item.Key);
                    }
                }
            }
            catch (Exception ex)
            {
     
                HandelExceptionLog?.BeginInvoke("发送给所有客户端时的报错：" + ex, null, null);
            }
            finally
            {
                lockSlim.ExitReadLock();
            }

        }


        /// <summary>
        /// 通过ip发送信息给客户端
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="msg"></param>
        public bool SendMsgToClientForIP(string ip,string msg) {

            lockSlim.EnterReadLock();

            bool isok = true ;

            try
            {
                if (dictsocket.ContainsKey(ip.Trim()))
                {
                     ClientMode clientMode= dictsocket[ip.Trim()];

                    if (SocketTools.IsSocketConnected(clientMode.socket))
                    {
                        clientMode.socket.Send(SocketTools.GetBytes(msg));
                        isok = true;
                    }
                    else
                    {
                        clientMode.socket.Close();
                        Console.WriteLine("IP已经退出" + clientMode.ip);
                        isok = false;
                    }

                }
             
                
            }
            catch (Exception ex)
            {

                HandelExceptionLog?.BeginInvoke("发送给客户端时的报错：" + ex, null, null);
                isok = false;
            }
            finally
            {
                lockSlim.ExitReadLock();
            }

            return isok;

        }



        /// <summary>
        /// 从号码移除一个客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ReMoveClientForId(string id)
        {

            List<string> vs = GetAllClientInfo();
            lockSlim.EnterWriteLock();
            try
            {
                for (int i = 0; i < vs.Count; i++)
                {
                    string[] str = vs[i].ToString().Trim().Split('-');

                    if (str[0].Trim().Equals(id))
                    {

                        if (dictsocket.ContainsKey(str[1].Trim()))
                        {
                            dictsocket[str[1].Trim()].socket.Close();  //关闭连接，就在线程报异常，到时自己清除

                        }
                        Console.WriteLine("删除了" + str[0] + "" + str[1]);
                        break;

                    }

                }

                return true;
            }
            catch (Exception ex)
            {
   
                HandelExceptionLog?.BeginInvoke("从号码移除一个客户端时的报错：" + ex, null, null);

                return false;
            }
            finally
            {

                lockSlim.ExitWriteLock();
            }


        }


        /// <summary>
        /// 心跳协议
        /// </summary>
        private void HearBeat()
        {
            // Console.WriteLine("进入了心跳协议");
            long timeNow = SocketTools.GetTimeStamp();
            lockSlim.EnterWriteLock();

            try
            {
                foreach (var item in dictsocket)
                {
                    long now = timeNow - item.Value.lastTickTime;
                    if (now > HearTime)
                    {
                     
                        item.Value.socket.Close();
                        //  ?.BeginInvoke(msg, null, null);
                        HandleClientClose?.BeginInvoke($"id号:{item.Value.id}，{item.Value.ip}用户没有心跳了，送他归家", null, null);

                    }
                    //else {

                    //    Console.WriteLine(timeNow);
                    //    Console.WriteLine(item.Value.lastTickTime);
                        
                    //}
                }
            }
            catch (Exception ex)
            {

                HandelExceptionLog?.BeginInvoke("心跳事件里面报错：" + ex, null, null);
            
            }
            finally
            {
                lockSlim.ExitWriteLock();
            }

        }





    }
}