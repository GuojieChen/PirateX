using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PirateX.GM.App_Start
{
    public class WorkingCopy
    {
        private static string SourcePath = System.Configuration.ConfigurationManager.AppSettings["App_Data_Dir"];
        private static string DestinationPath = $"{AppDomain.CurrentDomain.BaseDirectory}App_Data";


        private static object _lockHelper = new object();


        public static void CopySourceToTarget()
        {
            lock (_lockHelper)
            {
                //Now Create all of the directories
                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

                //Copy all the files & Replaces any files with the same name+--`
                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
            }
        }
    }
}