using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mature.Socket.Notify
{
    public class NotifyContainer
    {
        private NotifyContainer()
        {

        }
        private static readonly object lockObj = new object();
        private static NotifyContainer instance;

        public static NotifyContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new NotifyContainer();
                        }
                    }
                }
                return instance;
            }
        }

        private ConcurrentDictionary<string, List<INotifyPacket>> notify = new ConcurrentDictionary<string, List<INotifyPacket>>();

        public ConcurrentDictionary<string, List<INotifyPacket>> Notify
        {
            get { return notify; }
            set { notify = value; }
        }
        public void Register<TResponse>(string key, Action<TResponse> action)
        {
            if (notify.ContainsKey(key))
            {
                notify[key].Add(new NotifyPacket<TResponse>(action));
            }
            else
            {
                notify.TryAdd(key, new List<INotifyPacket>
                {
                    new NotifyPacket<TResponse>(action)
                });
            }
        }
        public void UnRegister<TResponse>(string key, Action<TResponse> action)
        {
            if (notify.ContainsKey(key))
            {
                INotifyPacket notifyPacket = null;
                foreach (var item in notify[key])
                {
                    if (item.Equals(action))
                    {
                        notifyPacket = item;
                        break;
                    }
                }
                if (notifyPacket != null)
                {
                    notify[key].Remove(notifyPacket);
                }
            }
        }
        public void Raise(string key)
        {
            if (notify.ContainsKey(key))
            {
                foreach (var item in notify[key])
                {
                    item.Raise(key);
                }
            }
        }
    }
}
