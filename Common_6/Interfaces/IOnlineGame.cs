using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IOnlineGame
    {
        Task<bool> SendGameRequestAsync();
    }
}
