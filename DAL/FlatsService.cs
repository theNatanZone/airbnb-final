using AirbnbProj2.Models;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AirbnbProj2.DAL
{
    public class FlatsService
    {
        private readonly DBService _dbService;

        public FlatsService(DBService dbService)
        {
            _dbService = dbService;
        }

        public bool InsertFlat(Flat flat)
        {
            try
            {
                var dbParameters = new DbParameter[]
                {
                    new SqlParameter("@city", flat.City),
                    new SqlParameter("@address", flat.Address),
                    new SqlParameter("@numberOfRooms", flat.NumberOfRooms),
                    new SqlParameter("@price", flat.Price)
                };

                var result = _dbService.Insert("spInsertFlat2024", dbParameters);
                return result != 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception thrown while flat insertion: {ex}");
                throw;
            }
        }

        public List<Flat> GetFlats()
        {
            return _dbService.ReadFlats();
        }

        public List<Flat> GetByCityAndMaxPrice(string city, double maxPrice)
        {
            var flats = GetFlats();
            return flats.Where(flat => flat.City == city && flat.Price <= maxPrice).ToList();
        }

    }
}
