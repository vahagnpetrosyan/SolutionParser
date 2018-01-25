using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SolutionParser
{
    public static class SolutionFIleParser
    {
        private static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();
        public static bool changeRoot = false;

        public static void GenerateSolutionFile(string slnFilePath)
        {
            if (!File.Exists(slnFilePath))
            {
                FileStream fileStream = (FileStream)null;
                SolutionFIleParser.readWriteLock.EnterWriteLock();
                try
                {
                    fileStream = File.Create(slnFilePath);
                }
                finally
                {
                    if (fileStream != null)
                        fileStream.Close();
                    SolutionFIleParser.readWriteLock.ExitWriteLock();
                }
            }
            if (new FileInfo(slnFilePath).Length != 0L)
                return;
            using (StreamWriter sw = new StreamWriter(slnFilePath))
                SolutionFileTemplateGenerator.InitFill(sw);
        }

        public static void AddProject(string slnFilePath, string projFilePath)
        {
            if (slnFilePath == null || projFilePath == null)
                throw new ArgumentNullException("One of the arguments is null");
            try
            {
                LinkedList<string> allLines1 = new LinkedList<string>();
                LinkedList<string> allLines2 = new LinkedList<string>();
                SolutionFIleParser.GenerateSolutionFile(slnFilePath);
                using (StreamReader streamReader = new StreamReader(slnFilePath))
                {
                    string str1;
                    while ((str1 = streamReader.ReadLine()) != "EndGlobal")
                        allLines1.AddLast(str1);
                    allLines1.AddLast(str1);
                    string str2;
                    while ((str2 = streamReader.ReadLine()) != null)
                        allLines2.AddLast(str2);
                }
                if (SolutionFIleParser.changeRoot)
                {
                    if (allLines2.Count == 0)
                        allLines2 = SolutionFileTemplateGenerator.InitFillList();
                    SolutionFIleParser.Adding(allLines2, projFilePath);
                }
                else
                    SolutionFIleParser.Adding(allLines1, projFilePath);
                using (StreamWriter streamWriter = new StreamWriter(slnFilePath))
                {
                    foreach (string str in allLines1)
                        streamWriter.WriteLine(str);
                    foreach (string str in allLines2)
                        streamWriter.WriteLine(str);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string[] TokenizeDir(string filePath)
        {
            string[] strArray = filePath.Split(new char[2]
            {
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar
            });
            List<string> source = new List<string>();
            int num = -1;
            for (int index = 0; index < strArray.Length; ++index)
            {
                if ("src".Equals(strArray[index]))
                {
                    num = index;
                    break;
                }
            }
            source.Add(strArray[num - 1]);
            for (int index = num + 1; index < strArray.Length; ++index)
            {
                if (index != strArray.Length - 2)
                    source.Add(strArray[index]);
            }
            return source.ToArray<string>();
        }

        private static Tuple<Tuple<string, string>, string> RetrieveProjectInfo(string projFilePath)
        {
            string str1 = (string)null;
            string str2 = (string)null;
            string str3 = (string)null;
            try
            {
                using (StreamReader streamReader = new StreamReader(projFilePath))
                {
                    string str4;
                    while ((str4 = streamReader.ReadLine()) != null)
                    {
                        if (str4.Contains("<ProjectGuid>"))
                        {
                            int num1 = str4.IndexOf(">");
                            int startIndex;
                            int num2 = str4.IndexOf("<", startIndex = num1 + 1);
                            str3 = str4.Substring(startIndex, num2 - startIndex);
                            break;
                        }
                    }
                    str1 = Path.GetFileNameWithoutExtension(projFilePath);
                }
                if (str2 == null)
                    str2 = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
                return str1 == null || str3 == null ? (Tuple<Tuple<string, string>, string>)null : new Tuple<Tuple<string, string>, string>(new Tuple<string, string>(str2, str3), str1);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return (Tuple<Tuple<string, string>, string>)null;
            }
        }

        private static void Adding(LinkedList<string> allLines, string projFilePath)
        {
            foreach (string allLine in allLines)
            {
                if (allLine.Contains(projFilePath))
                    return;
            }
            Tuple<Tuple<string, string>, string> tuple = SolutionFIleParser.RetrieveProjectInfo(projFilePath);
            string str1 = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
            string[] strArray = SolutionFIleParser.TokenizeDir(projFilePath);
            if (tuple == null)
                return;
            List<string> stringList1 = new List<string>();
            List<string> stringList2 = new List<string>();
            List<string> stringList3 = new List<string>();
            LinkedListNode<string> node1 = allLines.Find("Global");
            bool flag1 = false;
            for (int index1 = 0; index1 < strArray.Length - 1; ++index1)
            {
                bool flag2 = false;
                LinkedListNode<string> linkedListNode = allLines.First;
                for (int index2 = 0; index2 < 3; ++index2)
                    linkedListNode = linkedListNode.Next;
                for (; !linkedListNode.Value.Equals("Global"); linkedListNode = linkedListNode.Next)
                {
                    if (linkedListNode.Value.Contains("Project(\"") && linkedListNode.Value.Contains(str1))
                    {
                        int num1 = linkedListNode.Value.IndexOf("=");
                        int num2;
                        int num3 = linkedListNode.Value.IndexOf(",", num2 = num1 + 1);
                        int startIndex1 = num2 + 1;
                        if (linkedListNode.Value.Substring(startIndex1, num3 - startIndex1).Equals("\"" + strArray[index1] + "\""))
                        {
                            if (index1 == 0)
                            {
                                flag1 = true;
                                flag2 = true;
                            }
                            else
                                flag2 = flag1;
                            if (flag2)
                            {
                                int num4 = linkedListNode.Value.IndexOf("{");
                                int num5;
                                int startIndex2 = linkedListNode.Value.IndexOf("{", num5 = num4 + 1);
                                int num6 = linkedListNode.Value.IndexOf("}", startIndex2);
                                string str2 = linkedListNode.Value.Substring(startIndex2, num6 - startIndex2 + 1);
                                stringList2.Add(str2);
                            }
                        }
                    }
                }
                if (!flag2)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    string str2 = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
                    stringBuilder.Append("Project(\"");
                    stringBuilder.Append(str1);
                    stringBuilder.Append("\") = \"");
                    stringBuilder.Append(strArray[index1]);
                    stringBuilder.Append("\", \"");
                    stringBuilder.Append(strArray[index1]);
                    stringBuilder.Append("\", \"");
                    stringBuilder.Append(str2);
                    stringBuilder.Append("\"");
                    stringList3.Add(stringBuilder.ToString());
                    stringList1.Add(str2);
                }
            }
            foreach (string str2 in stringList3)
            {
                allLines.AddBefore(node1, str2);
                allLines.AddBefore(node1, "EndProject");
            }
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.Append("Project(\"");
            stringBuilder1.Append(tuple.Item1.Item1);
            stringBuilder1.Append("\") = \"");
            stringBuilder1.Append(tuple.Item2);
            stringBuilder1.Append("\", \"");
            stringBuilder1.Append(projFilePath);
            stringBuilder1.Append("\", \"");
            stringBuilder1.Append(tuple.Item1.Item2);
            stringBuilder1.Append("\"");
            allLines.AddBefore(node1, stringBuilder1.ToString());
            allLines.AddBefore(node1, "EndProject");
            LinkedList<string> linkedList1 = new LinkedList<string>();
            linkedList1.AddFirst("        " + tuple.Item1.Item2 + ".Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            linkedList1.AddFirst("        " + tuple.Item1.Item2 + ".Debug|Any CPU.Build.0 = Debug|Any CPU");
            linkedList1.AddFirst("        " + tuple.Item1.Item2 + ".Release|Any CPU.ActiveCfg = Release|Any CPU");
            linkedList1.AddFirst("        " + tuple.Item1.Item2 + ".Release|Any CPU.Build.0 = Release|Any CPU");
            LinkedListNode<string> node2 = allLines.Find("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            try
            {
                foreach (string str2 in linkedList1)
                    allLines.AddAfter(node2, str2);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            LinkedList<string> linkedList2 = new LinkedList<string>();
            List<string> stringList4 = new List<string>();
            foreach (string str2 in stringList2)
                stringList4.Add(str2);
            foreach (string str2 in stringList1)
                stringList4.Add(str2);
            for (int index = 1; index < stringList4.Count; ++index)
                linkedList2.AddFirst("\t\t" + stringList4[index] + " = " + stringList4[index - 1]);
            linkedList2.AddFirst("\t\t" + tuple.Item1.Item2 + " = " + stringList4[stringList4.Count - 1]);
            LinkedListNode<string> node3 = allLines.Find("\tGlobalSection(NestedProjects) = preSolution");
            try
            {
                foreach (string str2 in linkedList2)
                    allLines.AddAfter(node3, str2);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void ChangeRoot(bool boolean)
        {
            SolutionFIleParser.changeRoot = boolean;
        }
    }
}
