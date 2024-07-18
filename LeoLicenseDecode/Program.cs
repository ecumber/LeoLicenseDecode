using System;
using System.IO;
using System.Buffers;
using System.Xml.Serialization;
using System.Xml;
using System.Buffers.Text;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace LeoLicenseDecode;
class Program
{
    static void Main(string[] args)
    {
        string dir = "";
        if (args.Length == 0)
            dir = ".";
        else
            dir = args[0];
        foreach (string file in Directory.EnumerateFiles(dir, "*"))
        {
            List<string> packages = new List<string>();
            FileInfo info = new FileInfo(file);
            long size = info.Length;
            XmlDocument license = new XmlDocument();
            XmlDocument signedlicense = new XmlDocument();
            XmlNamespaceManager licensensmgr = new XmlNamespaceManager(license.NameTable);
            licensensmgr.AddNamespace(String.Empty, "http://schemas.microsoft.com/xboxlive/security/clas/LicResp/v1");
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
                string license_decoded = Encoding.UTF8.GetString(Convert.FromBase64String(license.GetElementsByTagName("SignedLicense").Item(0).InnerText));
                signedlicense.LoadXml(license_decoded);
                root = signedlicense.DocumentElement;
                if (root != null)
                {
                    string custompolicies = Encoding.UTF8.GetString(Convert.FromBase64String(signedlicense.GetElementsByTagName("CustomPolicies").Item(0).InnerText));
                    
                    JObject jsonobject = JObject.Parse(custompolicies);
                    JToken bodi = jsonobject["packages"];
                    JArray aodi = JArray.Parse(bodi.ToString());

                    int i = 0;
                    foreach (JToken todi in aodi)
                    {
                        i++;
                        string productid = todi.SelectToken("productId").ToString();
                        string msstorelink = String.Format("https://www.microsoft.com/store/productId/{0}", productid);
                        Console.WriteLine(String.Format(
                            "License: {0}\n" +
                            "\tPackage Index: {1}\n" +
                            "\t\tProduct ID: {2}\n" +
                            "\t\tMS Store Link: {3}\n",
                            file,
                            i,
                            productid,
                            msstorelink
                            ));
                    }


                }
            }
        }
    }
}