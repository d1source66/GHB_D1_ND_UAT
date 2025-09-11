using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using GHB_D1.Code.DAL;
using GHB_D1.Models;

namespace GHB_D1.Code.BAL
{
    public class ProcessLogBAL
    {

        public static void AddProcessLog(string strUsername, string strMessage)
        {
            bool result = false;
            try
            {
                result = ProcessLogDAL.AddProcessLog(strUsername, strMessage);
            }
            catch (Exception exc)
            {
                throw exc;
            }

        }

        public static void AddProcessLog2(AudiLogModel audilog)
        {
            bool result = false;
            try
            {
                result = ProcessLogDAL.AddProcessLog2(audilog);
            }
            catch (Exception exc)
            {
                throw exc;
            }

        }

        public static string GetServerMacAddress()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
        }

        public static string FormatMacAddress(string macAddress)
        {
            if (string.IsNullOrEmpty(macAddress)) return string.Empty;

            return string.Join(":", Enumerable.Range(0, macAddress.Length / 2)
                .Select(i => macAddress.Substring(i * 2, 2)))
                .ToUpper();
        }

        public static string GetClientOS(HttpRequestBase request)
        {
            string userAgent = request.UserAgent;

            if (userAgent.Contains("Windows NT 10.0")) return "Windows 10";
            if (userAgent.Contains("Windows NT 6.3")) return "Windows 8.1";
            if (userAgent.Contains("Windows NT 6.2")) return "Windows 8";
            if (userAgent.Contains("Windows NT 6.1")) return "Windows 7";
            if (userAgent.Contains("Mac OS X")) return "Mac OS";
            if (userAgent.Contains("Linux")) return "Linux";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("iPhone") || userAgent.Contains("iPad")) return "iOS";

            return "Unknown OS";
        }

        public static string GetServerOS()
        {
            return Environment.OSVersion.ToString();
        }

        public static string GetServerMachineName()
        {
            return Environment.MachineName;
        }

        public static string GetClientIpAddress(HttpRequestBase request)
        {
            return request.UserHostAddress;
        }

        public static string GetServerIpAddress()
        {
            string hostName = Dns.GetHostName();
            string serverIP = Dns.GetHostAddresses(hostName)
                                  .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                                  .ToString();
            return serverIP ?? "Unknown Server IP";
        }


    }
}