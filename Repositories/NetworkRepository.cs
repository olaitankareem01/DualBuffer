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

        public  List<Call> GetAllAsync()
        {
            return _context.calls.ToList();
        }

        public void DeleteAsync(int id)
        {
            var callFound = _context.calls.Find(id);
            if (callFound != null)
            {
                
                _context.calls.Remove(callFound);
                _context.SaveChanges();
            
            }


        }

        public void UpdateAsync(Call Call)
        {
            _context.calls.Update(Call);
        }
        
        public Call GetAsync(int id)
        {
           return _context.calls.Find(id);
        }


    }
}
