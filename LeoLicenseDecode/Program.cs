using System;
using System.IO;
using System.Buffers;
using System.Xml.Serialization;
using System.Xml;
using System.Buffers.Text;
using System.Text;
namespace LeoLicenseDecode;
class Program
{
    static void Main(string[] args)
    {
        string dir = "";
        if (args.Length == 0)
            dir = ".";
        else
            dir = args[1];
        foreach (string file in Directory.EnumerateFiles(dir, "*"))
        {
            FileInfo info = new FileInfo(file);
            long size = info.Length;
            XmlDocument license = new XmlDocument();



            try
            {
                license.Load(file);
            }
            catch
            {
                break;
            }
            XmlNode root = license.DocumentElement;
            if (root != null)
            {
                string license_decoded = Encoding.UTF8.GetString(Convert.FromBase64String(root.FirstChild.InnerText));


            }
        }
    }
}