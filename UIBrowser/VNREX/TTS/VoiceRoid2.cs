using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Threading;
namespace TTS
{
 public	class VoiceRoid2
	{
	 public	Process proc = new Process();
		public bool ini = false;

		public static string GetMD5HashFromFile(string filename)
		{
			try
			{
				FileStream file = new FileStream(filename, FileMode.Open);
				System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] retVal = md5.ComputeHash(file);
				file.Close();

				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));


				}

				return sb.ToString();



			}
			catch (System.Exception ex)
			{

				return "er";
			}

		}
		public bool VoiceRoid2CliInit(string cliPath)
        {
			bool result = false;


			if (!System.IO.File.Exists(cliPath))
				throw new Exception("文件不在在");

		string MD5=	GetMD5HashFromFile(cliPath);
			if (!MD5.Equals("ccecdfd708ea63fc90b68202c6cf93db"))
				throw new Exception("文件校验失败请选择正确的文件");
		String PriviousPath = Environment.CurrentDirectory;
			Environment.CurrentDirectory = Path.GetDirectoryName(cliPath);
		
			proc.StartInfo.FileName = Path.GetFileName(cliPath);
			//proc.StartInfo.Arguments = @"startservice";
			proc.StartInfo.UseShellExecute = false;
			//proc.StartInfo.StandardOutputEncoding = Encoding.Unicode;
			proc.StartInfo.RedirectStandardInput = true;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.CreateNoWindow = true;
            try
            {
				result=proc.Start();
				if (result)
                {
					proc.BeginOutputReadLine();
					Thread.Sleep(500);
					ini = true;
					//proc.StandardInput.WriteLineAsync("StartService");

				}
				
				Environment.CurrentDirectory = PriviousPath;
				return result;
            }
            catch
            {
				Environment.CurrentDirectory = PriviousPath;
				result = false;
				return result;
            }
			
		}


	}
}
