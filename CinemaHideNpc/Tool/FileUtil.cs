using System;
using System.IO;

namespace CinemaHideNpc
{
    public static class FileUtil
    {
        static FileUtil()
        {
        }

        public static FileStream SafeOpenWrite(string filePath)
        {
            FileStream fs = null;
            try
            {
                if (File.Exists(filePath))
                {
                    fs = File.Open(filePath, FileMode.Truncate, FileAccess.Write);
                }
                else
                {
                    fs = File.Create(filePath);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
            return fs;
        }

        public static FileStream SafeOpenRead(string filePath)
        {
            FileStream fs = null;
            try
            {
                if (File.Exists(filePath))
                {
                    fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
            return fs;
        }

        public static void SafeClose(FileStream fs)
        {
            if (fs != null)
            {
                try
                {
                    fs.Close();
                    fs = null;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }

        public static int SafeCopy(Stream source, byte[] destination, byte[] buffer)
        {
            if (source == null)
            {
                Console.Error.WriteLine("source stream is null!");
                return 0;
            }

            if (destination == null)
            {
                Console.Error.WriteLine("destination buffer is null!");
                return 0;
            }

            if (destination.LongLength < source.Length)
            {
                Console.Error.WriteLine("destination buffer smaller then source stream length! src={0}, dst={1}", source.Length, destination.LongLength);
                return 0;
            }

            try
            {
                if (source.Length < int.MaxValue)
                {
                    if (buffer == null
                        || destination.Length < buffer.Length)
                    {
                        return source.Read(destination, 0, (int)source.Length);
                    }
                    else
                    {
                        bool copying = true;
                        int writedCount = 0;
                        while (copying)
                        {
                            int bytesRead = source.Read(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                Buffer.BlockCopy(buffer, 0, destination, writedCount, bytesRead);
                                writedCount += bytesRead;
                            }
                            else
                            {
                                copying = false;
                            }
                        }
                        return writedCount;
                    }
                }
                else
                {
                    Console.Error.WriteLine("huge stream is unsurpported! src={0}", source.Length);
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                return 0;
            }
        }

        public static bool SafeDelete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            return false;
        }

        public static bool SafeCopy(string src, string dst)
        {
            try
            {
                if (File.Exists(src))
                {
                    if (!File.Exists(dst) || SafeDelete(dst))
                    {
                        File.Copy(src, dst, true);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            return false;
        }

        public static bool SafeMove(string src, string dst)
        {
            try
            {
                if (File.Exists(src))
                {
                    if (!File.Exists(dst) || SafeDelete(dst))
                    {
                        File.Move(src, dst);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }

            return false;
        }

        public static DirectoryInfo CreateDirectory(string path)
        {
            var pathSplit = path.Split('/');
            string currentPath = "";
            DirectoryInfo directoryInfo = null;
            for (int i = 1; i < pathSplit.Length; i++)
            {
                currentPath = currentPath + pathSplit[i];
                if (!Directory.Exists(currentPath))
                {
                    directoryInfo = Directory.CreateDirectory(currentPath);
                }
            }
            return directoryInfo;
        }
    }
}
