using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web;
using System.Net;
using System.Security.Cryptography;


static class ModConf
{

    [DllImport("kernel32.dll")]
    static extern int GetPrivateProfileString(
  string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

	public static string ReadIni(string strIniFile, string strKey, string strItem)
	{

		StringBuilder strValue = new StringBuilder(255);
		int intSize = 0;
		intSize = GetPrivateProfileString(strKey, strItem, "", strValue, 255, strIniFile);
		return strValue.ToString();

	}

}
