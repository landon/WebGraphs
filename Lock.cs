using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebGraphs
{
    public class ReadLock : IDisposable
    {
        ReaderWriterLockSlim _token;
        public ReadLock(ReaderWriterLockSlim token)
        {
            _token = token;
            _token.EnterReadLock();
        }
        public void Dispose()
        {
            _token.ExitReadLock();
        }
    }

    public class WriteLock : IDisposable
    {
        ReaderWriterLockSlim _token;
        public WriteLock(ReaderWriterLockSlim token)
        {
            _token = token;
            _token.EnterWriteLock();
        }
        public void Dispose()
        {
            _token.ExitWriteLock();
        }
    }
}
