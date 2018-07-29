using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Hw6
{
    public class LockGroup
    {
        private readonly object[] _locks;

        public LockGroup(int count)
        {
            _locks = new object[count];
            for (var i = 0; i < count; ++i)
            {
                _locks[i] = new object();                
            }
        }

        public void Accquire(IEnumerable<int> ids)
        {
            var sorted = ids.ToList();
            sorted.Sort();
            foreach (var id in sorted)
            {
                Monitor.Enter(_locks[id]);
            }
        }

        public void Release(IEnumerable<int> ids)
        {
            // Order here is irrelevant
            foreach (var id in ids)
            {
                Monitor.Exit(_locks[id]);
            }
        }
    }
}