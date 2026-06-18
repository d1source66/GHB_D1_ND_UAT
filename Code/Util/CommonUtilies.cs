using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GHB_D1.Code.Util
{
    public class CommonUtilies
    {
        Loger _logSys = new Loger();
        public void deleteFileByReport(string reportname, string ext)
        {
            string _strPathFile = System.Web.HttpContext.Current.Server.MapPath(@"~\Logs\");
            string[] files = Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath(@"~\TempFiles"));
            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);
                bool isMatch = filename.StartsWith(reportname) && file.EndsWith(ext);
                if (isMatch && System.IO.File.Exists(file))
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        _logSys.WriteErrLog(_strPathFile, "Cannot Delete File: " + ex.Message.ToString());
                    }
                }
            }
        }
    }
}