using DataAccessLayer;
using DataAccessLayer.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        ICustomerDAO Customer { get; }
        IRoomDAO Room { get; }
        Task<bool> SaveChangesAsync();
    }
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FUMiniHotelManagementContext _context;
        public UnitOfWork()
        {
            _context = new FUMiniHotelManagementContext();
            Customer = new CustomerDAO(_context);
            Room = new RoomDAO(_context);
        }
        public ICustomerDAO Customer { get; private set; }

        public IRoomDAO Room { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
