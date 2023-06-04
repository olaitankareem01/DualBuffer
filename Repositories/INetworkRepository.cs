using DualBuffer.Models.Enums;

namespace DualBuffer.Repositories
{
    public interface INetworkRepository
    {
        public Call AddAsync(Call call);
    }
}
