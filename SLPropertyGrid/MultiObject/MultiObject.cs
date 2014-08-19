using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLPropertyGrid.MultiObject
{
    public class MultiObject
    {
        IEnumerable _objects;
        public object Representative { get; private set; }

        public MultiObject(IEnumerable objects)
        {
            _objects = objects;

            var properties = new Dictionary<Tuple<string, Type>, List<object>>();
            int count = 0;
            foreach (var o in objects)
            {
                var t = o.GetType();
                foreach (var pi in t.GetProperties())
                {
                    if (!Browsable(pi))
                        continue;

                    var tuple = new Tuple<string, Type>(pi.Name, pi.PropertyType);

                    List<object> list;
                    if (!properties.TryGetValue(tuple, out list))
                    {
                        list = new List<object>();
                        properties[tuple] = list;
                    }

                    list.Add(t.GetProperty(pi.Name).GetValue(o, null));
                }

                count++;
            }

            var commonProperties = properties.Where(p => p.Value.Count == count).ToList();
            Representative = PropertySettingNotificationTypeWrapperBuilder.CreateObjectWithProperties(commonProperties.Select(p => p.Key));
            ((dynamic)Representative).ValueChanged = (Action<string>)(s => { });

            var rt = Representative.GetType();
            foreach (var kvp in commonProperties)
            {
                if (kvp.Value.Distinct().Count() == 1)
                    rt.GetProperty(kvp.Key.Item1).SetValue(Representative, kvp.Value.First(), null);
            }

            ((dynamic)Representative).ValueChanged = (Action<string>)OnValueChanged;
        }

        bool Browsable(System.Reflection.PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            var possiblyFakeBrowsableAttribute = attributes.Where(a => a.GetType().Name.Contains("BrowsableAttribute")).FirstOrDefault();
            if (possiblyFakeBrowsableAttribute != null)
            {
                var p = possiblyFakeBrowsableAttribute.GetType().GetProperty("Browsable");
                if (p != null)
                    return (bool)p.GetValue(possiblyFakeBrowsableAttribute, null);
            }

            return false;
        }

        void OnValueChanged(string propertyName)
        {
            var value = Representative.GetType().GetProperty(propertyName).GetValue(Representative, null);
            foreach (var o in _objects)
                o.GetType().GetProperty(propertyName).SetValue(o, value, null);
        }
    }
}
