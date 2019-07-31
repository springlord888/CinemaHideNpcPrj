using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LitJson;

namespace CinemaHideNpc
{
    public class Persistence
    {
        public static readonly JsonPersistence json = new JsonPersistence();
        public class JsonPersistence
        {
            //public void Save(string path, object o)
            //{
            //    using (var stream = File.CreateText(path))
            //    {
            //        Serializer.json.Serialize(stream, o);
            //    }
            //}

            ////public T Load<T>(string path)
            ////{
            ////    var aa = ResourceManager2.Instance.LoadResourceSync(path);
            ////    var buffer = aa != null ? aa.rfile.Buffer : null;
            ////    if (buffer != null)
            ////    {
            ////        using (Stream stream = new MemoryStream(buffer, 0, aa.rfile.NumBytesToRead))
            ////        {
            ////            using (var sr = new StreamReader(stream))
            ////            {
            ////                T ret = Serializer.json.Deserialize<T>(sr, path);
            ////                aa.ReleaseImmediately();
            ////                return ret;
            ////            }
            ////        }
            ////    }

            ////    return default(T);
            ////}

            public string PrettyJson(string text)
            {
                text = text.Replace("[", "[\n");
                text = text.Replace("{", "{\n");
                text = text.Replace(",", ",\n");
                text = text.Replace("]", "]\n");
                return text;
            }

            public void Save(string path, object o)
            {
                File.Delete(path);
                using (var sw = File.CreateText(path))
                {
                    string text = JsonMapper.ToJson(o);
                    text = PrettyJson(text);
                    sw.Write(text);
                }
            }


            public T LoadFile<T>(string filePath)
            {
                using (var fs = FileUtil.SafeOpenRead(filePath))
                {
                    if (fs != null)
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            string json_phrase = sr.ReadToEnd();

                            JsonReader jr = new JsonReader(json_phrase);
                            T result = JsonMapper.ToObject<T>(jr);
                            return result;
                        }
                    }
                }

                return default(T);
            }
        }
    }
}
