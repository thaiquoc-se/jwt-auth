using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAO
{
    public interface ICustomerDAO : IBaseDAO<Customer, int> { }

    public class CustomerDAO : BaseDAO<Customer, int>, ICustomerDAO
    {
        public CustomerDAO(FUMiniHotelManagementContext context) : base(context)
        {
        }
    }
}
