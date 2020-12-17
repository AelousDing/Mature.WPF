using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Command
{
    public interface ICommand<TKey>
    {
        TKey Key { get; }
        string Name { get; }
    }
    public interface ICommand<TKey, TPackageInfo> : ICommand<TKey> where TPackageInfo : IkeyedPackageInfo<TKey> 
    {
        void Execute(ISessionWrapper session,TPackageInfo packageInfo);
    }
    public interface IAsyncCommand<TKey, TPackageInfo> : ICommand<TKey> where TPackageInfo : IkeyedPackageInfo<TKey> 
    {
        Task ExecuteAsync(ISessionWrapper session, TPackageInfo packageInfo);
    }
}
