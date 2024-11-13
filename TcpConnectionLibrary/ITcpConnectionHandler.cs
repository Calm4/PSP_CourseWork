using System;
using System.Threading.Tasks;

namespace TcpConnectionLibrary
{
    public interface ITcpConnectionHandler
    {
        Task UpdateData<T>(T obj);
        event Action<object> OnGetData;
        void ClearAllListeners();
    }
}
