﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace ServiceLoaderMedpomData
{
    public class FileAndMD5
    {
        public string Name { get; set; }
        public string MD5 { get; set; }
        [XmlIgnore]
        public string Comment_status { get; set; }
        [XmlIgnore]
        public int CodeStatus { get; set; }
        public FileAndMD5()
        {
        }
        public FileAndMD5(string Name, string MD5, int Length)
        {
            this.Name = Name;
            this.MD5 = MD5;
            this.Length = Length;
        }
        public int Length { get; set; }
    }


    public class Version
    {
        public List<FileAndMD5> FileList = new List<FileAndMD5>();



        public static string GetMd5Hash(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                return result;
            }
        }
        public static bool VerifyMd5Hash(string path, string hash)
        {
            return GetMd5Hash(path) == hash;
        }

        public void SaveToFile(string path)
        {
            Stream st = File.Create(path);
            XmlSerializer ser = new XmlSerializer(typeof(List<FileAndMD5>));
            ser.Serialize(st, FileList);
            st.Close();

        }

        public void LoadFromFile(string path)
        {
            Stream st = File.OpenRead(path);
            XmlSerializer ser = new XmlSerializer(typeof(List<FileAndMD5>));
            FileList = (List<FileAndMD5>)ser.Deserialize(st);
            st.Close();

        }

    }
}
