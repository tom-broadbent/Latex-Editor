using AvaloniaEdit.CodeCompletion;
using DynamicData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor
{
    public static class LatexCompletionDataLoader
    {
        public static List<string> Data { get; } = new List<string>();

        public static void LoadFromDirectory(string directory)
        {
            Data.Clear();
            foreach (var path in Directory.GetFiles("Completion"))
            {
                var list = GetFromFile(path);
                Data.AddRange(list);
            }
            Data.Sort();
        }

        public static void LoadFromFile(string path)
        {
            Data.Clear();
            var list = GetFromFile(path);
            Data.AddRange(list);
            Data.Sort();
        }

        public static List<string> GetFromFile(string path)
        {
            var list = new List<string>();
            using (var streamReader = new StreamReader(path))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line[0] == '\\')
                        {
                            // remove comments
                            line = line.Split('#')[0];
                            if (line.Length > 0)
                            {
                                list.Add(line);
                            }
                        }
                    }
                }
            }

            return list;
        }
    }
}
