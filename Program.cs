using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
namespace BulkDataUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            ForderChecker forderChecker = new ForderChecker(@"E:\Jenkins' File\FIT\2020 Spring Grad\Hurricane\ISAIAS\ISAIAS_Jack's08012020");
            forderChecker.CollectFiles();
            DesignsafeUploader designsafeUploader = new DesignsafeUploader(forderChecker, "project-2213334571396698601-242ac11a-0001-012/ISAIAS_Data/OldSystem/");
            designsafeUploader.ShowLocalPath = true;
            designsafeUploader.ShowCommand = true;
            designsafeUploader.ShowOutput = true;
            designsafeUploader.Uploader();
            Console.ReadLine();
        }
    }
    class ForderChecker
    {
        public string Fordername { get; set; }

        public string[] FilesPathList { get; set; }
        public int FilesAmount { get; set; }
        public ForderChecker(string Fordername)
        {
            this.Fordername = Fordername;
            ///Input <LocalFordername> should be a string of forder address with the starter of @; 
            ///Ex: @"D:\jenkins personal file\FIT\2019 Fall Grad\Hurricane\Dorian Hurricane House Jack"
        }

        public void CollectFiles()
        {
            if (Directory.Exists(this.Fordername) == true)
            {
                try
                {
                    this.FilesPathList = Directory.GetFiles(this.Fordername);
                    this.FilesAmount = FilesPathList.Length;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else { Console.WriteLine("Fordername Wrong"); }

        }
        public void ShowFilesAmount()
        {
            Console.WriteLine("We have {0} Files at {1}", this.FilesAmount, this.Fordername);
            foreach (string file in this.FilesPathList)
            {
                Console.WriteLine(file);
            }

        }
        public void ShowFilesName()
        {
            this.ShowFilesAmount();
            foreach (string file in this.FilesPathList)
            {
                Console.WriteLine(file);
            }

        }
    }
    class DesignsafeUploader
    {

        public string[] LocalFilesName { get; set; }
        public string LocalFordername { get; set; }
        public string[] LocalPathList { get; set; }
        public string CloudPath { get; set; }
        /// <param name="cloudPath">Cloud destination path.(i.e. project-6284144844314644966-242ac11c-0001-012/GUI_Test/)</param>
        public int LocalFileAmount { get; set; }
        public string Command { get; set; }
        public string TapisHeader { get; set; }
        private readonly Process process;
        public bool ShowCommand { get; set; }
        public bool ShowLocalPath { get; set; }
        public bool ShowOutput { get; set; }
        public DesignsafeUploader(ForderChecker forderChecker, string cloudPath)
        {
            process = new Process();
            this.LocalFordername = forderChecker.Fordername;
            this.LocalFileAmount = forderChecker.FilesAmount;
            this.LocalPathList = forderChecker.FilesPathList;
            this.CloudPath = cloudPath;
            this.TapisHeader = "files upload agave://";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = "tapis";

        }
        protected string BuildCommand(string localPath)
        {
            string command = this.TapisHeader + this.CloudPath + " \"" + localPath + "\"";
            return command;

        }
        public void Uploader()
        {

            foreach (string localPath in this.LocalPathList)
            {
                this.Command = this.BuildCommand(localPath);
                process.StartInfo.Arguments = this.Command;
                if (this.ShowLocalPath == true)
                {
                    Console.WriteLine("Uploading File:{0}", localPath);
                }
                if (this.ShowCommand == true)
                {
                    Console.WriteLine("<{0}>", this.Command);


                }
                process.Start();
                StreamReader reader = process.StandardOutput;
                string output = reader.ReadToEnd();
                reader.Close();
                if (this.ShowOutput == true)
                {
                    Console.WriteLine("<{0}>", output);
                }
                process.WaitForExit();
                process.Close();
                Thread.Sleep(3000);
            }
            Console.WriteLine("**************Finish***********");

        }
    }

}
