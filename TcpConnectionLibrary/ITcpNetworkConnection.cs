using System;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public interface ITcpNetworkConnection
    {
        Task UpdateNetworkData<T>(T obj);
        event Action<object> OnGetNetworkData;
        void UnsubscribeActions();
    }
}
