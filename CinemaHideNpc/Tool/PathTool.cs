using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CinemaHideNpc
{
    class PathTool
    {
        public static string Combine(string p1, string p2, bool normalizePath = true)
        {
            string path = Path.Combine(p1, p2);
            if (normalizePath)
            {
                return NormalizePath(path);
            }
            else
            {
                return path;
            }
        }

        public static string NormalizePath(string path)
        {
            return path.Replace("\\\\", "/").Replace('\\', '/');
        }
    }
}
