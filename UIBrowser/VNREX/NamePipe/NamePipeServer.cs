using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UIBrowser.VNREX.NamePipe
{
  public   class NamePipeServer
    {
        public Boolean serverIsOpen = false;
        /// <summary>
        /// 重要 在轮询线程中将不停向此队列提取数据向客户端发送过去
        /// </summary>
        public ConcurrentQueue<string> safeCollectin = new ConcurrentQueue<string>();
        NamedPipeServerStream pipeServer;
        private string temp;
        public int clinetNumber = 0;
        /// <summary>
        /// 初始化命名管道
        /// </summary>
        public NamePipeServer()
        {



        }
        /// <summary>
        /// 开启服务器轮询向客户端写入数据
        /// </summary>
        public void startServer()
        {
            pipeServer =
                new NamedPipeServerStream("testpipe", PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            serverIsOpen = true;
            ThreadPool.QueueUserWorkItem(delegate
            {
                pipeServer.BeginWaitForConnection((o) =>
                {
                    NamedPipeServerStream pServer = (NamedPipeServerStream)o.AsyncState;
                    pServer.EndWaitForConnection(o);
                    clinetNumber++;
                    StreamWriter sr = new StreamWriter(pServer);
                    sr.AutoFlush = true;
                    while (true)
                    {

                        var result = safeCollectin.TryDequeue(out temp);
                        if (result)
                        {
                            sr.WriteLine(temp);
                        }
                        Thread.Sleep(50);
                    }

                }, pipeServer);
            });

        }
        public void closeServer()
        {
            pipeServer.Close();
            serverIsOpen = false;
        }
    }
}
