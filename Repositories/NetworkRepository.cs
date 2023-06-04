using DualBuffer.Models.Enums;

namespace DualBuffer.Repositories
{
    public class NetworkRepository : INetworkRepository
    {
        private readonly NetworkDbContext _context;
        public NetworkRepository(NetworkDbContext context) {
            _context = context;
        }

        public  Call AddAsync(Call call)
        {
            _context.calls.Add(call);

           _context.SaveChanges();

            return call;
        }



    }
}
