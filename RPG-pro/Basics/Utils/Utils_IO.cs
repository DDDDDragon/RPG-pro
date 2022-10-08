using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_pro.Basics.Utils
{
    public class Utils_IO
    {
        public static void FindFiles(List<string> files, string rootpath, Predicate<string> predicate)
        {
            string[] subs = Directory.GetFileSystemEntries(rootpath);
            foreach (string sub in subs)
            {
                if(File.Exists(sub))
                {
                    if(predicate(sub))
                    {
                        files.Add(sub);
                    }
                }
                else
                {
                    FindFiles(files, sub, predicate);
                }
            }
        }
        public static bool FindFile(string rootpath, Predicate<string> predicate, out string file)
        {
            string[] subs = Directory.GetFileSystemEntries(rootpath);
            foreach (string sub in subs)
            {
                if (File.Exists(sub))
                {
                    if (predicate(sub))
                    {
                        file = sub;
                        return true;
                    }
                }
                else
                {
                    if(FindFile(sub, predicate, out file))
                    {
                        return true;
                    }
                }
            }
            file = string.Empty;
            return false;
        }
    }
}
