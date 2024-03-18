using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAO
{
    public interface IRoomDAO : IBaseDAO<RoomInformation, int> { }
    public class RoomDAO : BaseDAO<RoomInformation, int>, IRoomDAO
    {
        public RoomDAO(FUMiniHotelManagementContext context) : base(context)
        {
        }
    }
}
