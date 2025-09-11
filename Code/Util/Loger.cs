using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.IO;

namespace GHB_D1.Code.Util
{
    public class Loger
    {

        public void WriteErrLog(string pathFile, string message)
        {
            CultureInfo _cultureEnInfo = new CultureInfo("en-US");
            string strFileNameLog = DateTime.Now.ToString("yyyyMMdd", _cultureEnInfo);
            if (!Directory.Exists(pathFile))
            {
                Directory.CreateDirectory(pathFile);
            }
            string file_name = Path.Combine(pathFile, "LogErrProcess_" + strFileNameLog + ".txt");
            StreamWriter sw = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.Write);
                using (sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy", _cultureEnInfo) + " " + DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close();
            }
            catch { }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        public void WriteProcessLogFile(string pathFile, string message)
        {
            CultureInfo _cultureEnInfo = new CultureInfo("en-US");
            string strFileNameLog = DateTime.Now.ToString("yyyyMMdd", _cultureEnInfo);
            if (!Directory.Exists(pathFile))
            {
                Directory.CreateDirectory(pathFile);
            }
            string file_name = Path.Combine(pathFile, "LogProcess_" + strFileNameLog + ".txt");
            StreamWriter sw = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.Write);
                using (sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy", _cultureEnInfo) + " " + DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close();
            }
            catch { }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }

        public void WriteErrDBLog(string pathFile, string message)
        {
            CultureInfo _cultureEnInfo = new CultureInfo("en-US");
            string strFileNameLog = DateTime.Now.ToString("yyyyMMdd", _cultureEnInfo);
            if (!Directory.Exists(pathFile))
            {
                Directory.CreateDirectory(pathFile);
            }
            string file_name = Path.Combine(pathFile, "LogDBErr_" + strFileNameLog + ".txt");
            StreamWriter sw = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.Write);
                using (sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy", _cultureEnInfo) + " " + DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message);
                    sw.Flush();
                    sw.Close();
                }
                fs.Close();
            }
            catch { }
            finally
            {
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }
        }
    }
}