using System.Threading.Tasks;

namespace companion.Services
{
    public interface ISignalRService
    {
        void Connect();
        void SendMessage(string id, string msg);
    }
}