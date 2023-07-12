using DualBuffer.Models.Enums;

namespace DualBuffer.Repositories
{
    public interface INetworkRepository
    {
        public Call AddAsync(Call call);

        public List<Call> GetAllAsync();

        public void DeleteAsync(int id);

        public void UpdateAsync(Call Call);

        public Call GetAsync(int id);
    }


}
