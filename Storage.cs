using Graphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WebGraphs
{
    public static class Storage
    {
        public static void Save(Graph g, string name)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                using (var file = store.CreateFile(name))
                using (var sw = new StreamWriter(file))
                    sw.Write(g.Serialize());
            }
            catch { }
        }

        public static void Delete(string name)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    if (store.FileExists(name))
                        store.DeleteFile(name);
                }
                catch { }
            }
        }

        public static Graph Load(string name)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var file = store.OpenFile(name, FileMode.Open))
            using (var sr = new StreamReader(file))
                return Graph.Deserialize(sr.ReadToEnd());
        }

        public static IEnumerable<string> GetFileNames(string searchPattern = null)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (searchPattern == null)
                        return store.GetFileNames();
                    return store.GetFileNames(searchPattern);
                }
            }
            catch { }

            return new List<string>();
        }
    }
}
