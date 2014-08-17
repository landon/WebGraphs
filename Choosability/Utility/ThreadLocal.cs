using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Choosability.Utility
{
    public class ThreadLocal<T>
    {
        readonly Func<T> _valueCreator;
        [ThreadStatic]
        static ConditionalWeakTable<object, Holder> _state;

        public class Holder { public T Val; }
        public ThreadLocal() : this(() => default(T)) { }

        public ThreadLocal(Func<T> valueCreator)
        {
            _valueCreator = valueCreator;
        }

        public T Value
        {
            get
            {
                Holder value;
                if (_state == null || _state.TryGetValue(this, out value) == false)
                {
                    var val = _valueCreator();
                    Value = val;
                    return val;
                }

                return value.Val;
            }
            set
            {
                if (_state == null)
                    _state = new ConditionalWeakTable<object, Holder>();

                var holder = _state.GetOrCreateValue(this);
                holder.Val = value;
            }
        }
    }
}
