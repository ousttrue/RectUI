using RectUI.JSON;
using System;
using System.IO;
using System.Linq;
using System.Reflection;


namespace RectUIGLTFSample
{
    class State
    {
        public string OpenFile;
        public string OpenDir;

        static string ExecutablePath
        {
            get
            {
                var path = Assembly.GetAssembly(typeof(State)).CodeBase;
                return path.Substring("file:///".Length);
            }
        }

        static string StatePath
        {
            get
            {
                var exe = ExecutablePath;
                return Path.Combine(
                    Path.GetDirectoryName(exe),
                    Path.GetFileNameWithoutExtension(exe) + ".json"
                    );
            }
        }

        static State s_instance = new State();
        public static State Instance
        {
            get { return s_instance; }
        }

        public static void Save()
        {
            var f = new JsonFormatter();
            f.Serialize(s_instance);
            var path = StatePath;
            Console.WriteLine(path);
            File.WriteAllBytes(path, f.GetStoreBytes().ToArray());
        }

        public static void Restore()
        {
            try
            {
                var bytes = File.ReadAllBytes(StatePath);
                var json = bytes.ParseAsJson();
                json.Deserialize(ref s_instance);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
