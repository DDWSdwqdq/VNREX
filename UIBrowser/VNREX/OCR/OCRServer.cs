using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace UIBrowser.VNREX.OCR
{

    public class OCRServer
    {
        public static readonly string ScreenSpyPath = @"\ocr-off";
        public static readonly string OCRClientExePath = @"\ocr-off\ocr";
        public static readonly string RectPath = @"\ocr-off\ocr\rect.txt";
        static Dictionary<string, Socket> clientList = new Dictionary<string, Socket>();
        static Rectangle ScrrenRect = new Rectangle();
        /// <summary>
        /// 获取桌面图片
        /// </summary>
        /// <returns></returns>
        public static System.Drawing.Bitmap GetScreenSnapshot()

        {
            System.Drawing.Rectangle rc = SystemInformation.VirtualScreen;


            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))

            {
                g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, System.Drawing.CopyPixelOperation.SourceCopy);

            }

            return bitmap;

        }

        /// <summary>
        /// 截取图像的矩形区域
        /// </summary>
        /// <param name="source">源图像对应picturebox1</param>
        /// <param name="rect">矩形区域，如上初始化的rect</param>
        /// <returns>矩形区域的图像</returns>
        public static Image AcquireRectangleImage(Image source, Rectangle rect)
        {
            if (source == null || rect.IsEmpty) return null;

            Bitmap bmSmall = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //Bitmap bmSmall = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            using (Graphics grSmall = Graphics.FromImage(bmSmall))
            {

                grSmall.DrawImage(source, new System.Drawing.Rectangle(0, 0, bmSmall.Width, bmSmall.Height), rect, GraphicsUnit.Pixel);
                grSmall.Dispose();
            }

            return bmSmall;
        }


        /// 服务器发送消息，客户端接收
        public static void SendMsg(string str)
        {
            ///遍历出字典中的所有线程
            foreach (var item in clientList)
            {
                byte[] arrMsg = Encoding.UTF8.GetBytes(str);

                ///获取键值(服务器），发送消息
                item.Value.Send(arrMsg);
            }
        }
        /// 服务器接收到客户端发送的消息
        public static void ReciveMsg(object o)
        {
            //Socket connectClient = (Socket)o; //与下面效果一样

            Socket connectClient = o as Socket;//connectClient负责客户端的通信
            IPEndPoint endPoint = connectClient.RemoteEndPoint as IPEndPoint;
            string preOCRText = "";
            while (true)
            {
                try
                {
                    ///定义服务器接收的字节大小
                    byte[] arrMsg = new byte[1024 * 1024];

                    ///接收到的信息大小(所占字节数)
                    int length = connectClient.Receive(arrMsg);

                    if (length > 0)
                    {
                        string recMsg = Encoding.UTF8.GetString(arrMsg, 0, length);
                        if (recMsg.Equals(preOCRText))
                        {
                            continue;
                        }
                        else
                        {
                            preOCRText = recMsg;
                        }
                        updateTextEvent.Invoke(recMsg);

                        VNREX.Global.TTSString = recMsg;
                        VNREX.Global.TranslateInstance.Translate(recMsg, VNREX.Global.LangSetting);
                        //获取客户端的端口号
                        //endPoint = connectClient.RemoteEndPoint as IPEndPoint;
                        //服务器显示客户端的端口号和消息
                        // listBox_log.Items.Add(DateTime.Now + "[" + endPoint.Port.ToString() + "]：" + recMsg);

                        //服务器(connectClient)发送接收到的客户端信息给客户端
                        // SendMsg("[" + endPoint.Port.ToString() + "]：" + recMsg);
                    }
                }
                catch (Exception)
                {
                    ///移除添加在字典中的服务器和客户端之间的线程
                    clientList.Remove(endPoint.ToString());

                    connectClient.Dispose();
                }
            }
        }

        public static void SendMsgProc(object o)
        {
            Socket connectClient = o as Socket;//connectClient负责客户端的通信
            IPEndPoint endPoint = connectClient.RemoteEndPoint as IPEndPoint; ;
            try
            {
                while (true)
                {
                    //得到完整屏幕的图像
                    Image catImage = GetScreenSnapshot();
                    //截取 自己选择的矩形区域 生成新的IMAGE
                    Image Image = AcquireRectangleImage(catImage, ScrrenRect);
                    //string base64Image = ImageToBase64(Image);
                    byte[] imageBuffer = ImageToBuffer(Image);
                    Image.Dispose();
                    catImage.Dispose();

                    connectClient.Send(imageBuffer);
                    Thread.Sleep(1200);
                }
            }
            catch(Exception)
            {
                ///移除添加在字典中的服务器和客户端之间的线程
                clientList.Remove(endPoint.ToString());

                connectClient.Dispose();
            }


        }
        public static string TempPor = "8699";
  
        public delegate void OCRTextEventHandler(string msg);
        private static OCRTextEventHandler updateTextEvent;
        public static event OCRTextEventHandler updateText
        {
            add
            {
                updateTextEvent += value;
            }
            remove
            {
                updateTextEvent -= value;
            }
        }
        public static string ImageToBase64(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            File.WriteAllBytes(@"F:\4\1.png", ms.ToArray());
            return Convert.ToBase64String(ms.ToArray());
        }
        public static void EncodePNG(Image image)
        {
         /*
            MemoryStream ms = new MemoryStream();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Interlace = PngInterlaceOption.On;
            encoder.Frames.Add(BitmapFrame.Create();
            encoder.Save(ms);
            return Convert.ToBase64String(ms.ToArray());*/
        }

        public static byte[] ImageToBuffer(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();

        }
        public static void AutoOCRTranslateProc(object ob)
        {
            //1.创建一个用于监听连接的Socket对象；
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            //2.用指定的端口号和服务器的Ip建立一个EndPoint对象；
            IPAddress iP = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(iP, int.Parse(TempPor));

            //3.用Socket对象的Bind()方法绑定EndPoint；
            server.Bind(endPoint);

            //4.用Socket对象的Listen()方法开始监听；

            //同一时刻内允许同时加入链接的最大数量
            server.Listen(1);

            //5.接收到客户端的连接，用Socket对象的Accept()方法创建一个新的用于和客户端进行通信的Socket对象；
            while (true)
            {
                //接受接入的一个客户端
                Socket connectClient = server.Accept();
                if (connectClient != null)
                {
                    string infor = connectClient.RemoteEndPoint.ToString();
                    clientList.Add(infor, connectClient);

                    Thread threadSendClient = new Thread(SendMsgProc);//带参的方法可以把传递的参数放到start中
                    threadSendClient.IsBackground = true;

                    //创建的新的对应的Socket和客户端Socket进行通信
                    threadSendClient.Start(connectClient);
                   // Thread.Sleep(100);

                    ///服务器将消息发送至客服端
                  

                   // SendMsg(msg);

                    //每有一个客户端接入时，需要有一个线程进行服务

                    Thread threadClient = new Thread(ReciveMsg);//带参的方法可以把传递的参数放到start中
                    threadClient.IsBackground = true;

                    //创建的新的对应的Socket和客户端Socket进行通信
                    threadClient.Start(connectClient);
                   // Thread.Sleep(100);
                }


            }
        }
        public static bool StartServer(string port)
        {
            TempPor = port;
            string screenRectPath = System.AppDomain.CurrentDomain.BaseDirectory + RectPath;
            if (!File.Exists(screenRectPath))
            {
                MessageBox.Show("rect.txt未找到，请先截取一块屏幕再执行此操作!");
                return false;
            }
            string rectStr = File.ReadAllText(screenRectPath);
            string[] rectData=rectStr.Split(',');
            ScrrenRect.X = int.Parse(rectData[0]);
            ScrrenRect.Y = int.Parse(rectData[1]);
            ScrrenRect.Width = int.Parse(rectData[2]);
            ScrrenRect.Height = int.Parse(rectData[3]);

            if (ScrrenRect.X== 0 && ScrrenRect.Y == 0)
            {
                MessageBox.Show("x,y不能都为0!");
                return false;
            }
            if (ScrrenRect.Width == 0 || ScrrenRect.Height == 0)
            {
                MessageBox.Show("宽度或高度不能为0!");
                return false;
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(AutoOCRTranslateProc));
            return true;
                     

         }

        public static void GenerateRect()
        {

            String PriviousPath = Environment.CurrentDirectory;

            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory + ScreenSpyPath;

     
            try
            {
                Process.Start("ScreenSpy.exe");
                Environment.CurrentDirectory = PriviousPath;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.CurrentDirectory = PriviousPath;
            }

        }
        public static void OpenOCRClient()
        {
            //OCRClientExePath

            String PriviousPath = Environment.CurrentDirectory;

            Environment.CurrentDirectory = System.AppDomain.CurrentDomain.BaseDirectory + OCRClientExePath;


            try
            {
                Process.Start("electron.exe");
                Environment.CurrentDirectory = PriviousPath;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.CurrentDirectory = PriviousPath;
            }
        }

    }
}
