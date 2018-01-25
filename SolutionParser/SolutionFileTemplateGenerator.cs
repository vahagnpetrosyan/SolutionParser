using System;
using System.Collections.Generic;
using System.IO;

namespace SolutionParser
{
    internal static class SolutionFileTemplateGenerator
    {
        public static LinkedList<string> InitFillList()
        {
            LinkedList<string> linkedList = new LinkedList<string>();
            linkedList.AddLast("Microsoft Visual Studio Solution File, Format Version 12.00");
            linkedList.AddLast("# Visual Studio 14");
            linkedList.AddLast("VisualStudioVersion = 14.0.25420.1");
            linkedList.AddLast("MinimumVisualStudioVersion = 10.0.40219.1");
            linkedList.AddLast("Global");
            linkedList.AddLast("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            linkedList.AddLast("\t\tDebug|Any CPU = Debug|Any CPU");
            linkedList.AddLast("\t\tRelease|Any CPU = Release|Any CPU");
            linkedList.AddLast("\tEndGlobalSection");
            linkedList.AddLast("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            linkedList.AddLast("\tEndGlobalSection");
            linkedList.AddLast("\tGlobalSection(SolutionProperties) = preSolution");
            linkedList.AddLast("\t\tHideSolutionNode = FALSE");
            linkedList.AddLast("\tEndGlobalSection");
            linkedList.AddLast("\tGlobalSection(NestedProjects) = preSolution");
            linkedList.AddLast("\tEndGlobalSection");
            linkedList.AddLast("EndGlobal");
            return linkedList;
        }

        public static void InitFill(StreamWriter sw)
        {
            foreach (string initFill in SolutionFileTemplateGenerator.InitFillList())
                sw.WriteLine(initFill);
        }

        public static string GetProjectRecord(string key)
        {
            return string.Format("Project(\"{0}\") = \"Sky.Suite.ViewModels\", \"Sky.Suite.ViewModels\", \"{1}\n", key, key) + " EndProject";
        }

        public static string FileExtensionRemove(string fileName)
        {
            int length = -1;
            int num;
            while ((num = fileName.IndexOf(".")) != -1)
                length = num;
            if (length != -1)
                return fileName.Substring(0, length);
            throw new ArgumentException("the argument is not valid");
        }
    }
}
