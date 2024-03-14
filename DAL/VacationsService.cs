using AirbnbProj2.Models;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AirbnbProj2.DAL
{
    public class VacationsService
    {
        private readonly DBService _dbService;

        public VacationsService(DBService dbService)
        {
            _dbService = dbService;
        }

        public bool InsertVacation(Vacation vacation)
        {
            try
            {
                var dbParameters = new DbParameter[]
                {
                    new SqlParameter("@id", vacation.Id),
                    new SqlParameter("@userId", vacation.UserId),
                    new SqlParameter("@flatId", vacation.FlatId),
                    new SqlParameter("@startDate", vacation.StartDate),
                    new SqlParameter("@endDate", vacation.EndDate)
                };

                var result = _dbService.Insert("spInsertVacation2024", dbParameters);
                return result != 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception thrown while vacation insertion: {ex}");
                throw;
            }
        }

        public List<Vacation> GetVacations()
        {
            return _dbService.ReadVacations();
        }

        public List<Vacation> GetByDates(DateTime startDate, DateTime endDate)
        {
            var vacationsList = GetVacations();
            return vacationsList.Where(v => v.StartDate >= startDate && v.EndDate <= endDate)
                .ToList();
        }

        public bool IsAvailable(int flatId, DateTime startDate, DateTime endDate)
        {
            var vacationsList = GetVacations();
            return !vacationsList.Exists(v =>
                v.FlatId == flatId &&
                (startDate < v.EndDate && endDate > v.StartDate));
        }

        public Vacation? GetById(int id)
        {
            return GetVacations().FirstOrDefault(v => v.Id == id);
        }

        public void ValidateVacationModel(Vacation vacation)
        {
            if (vacation.Id <= 0)
                throw new ArgumentException("ArgumentException: Invalid vacation ID- must be greater than 0.");

            if (vacation.FlatId <= 0)
                throw new ArgumentException("ArgumentException: Invalid flat ID- must be greater than 0.");
            
            if (string.IsNullOrEmpty(vacation.UserId))
                throw new ArgumentException("ArgumentException: The field 'userId' is required.");

            if (vacation.EndDate <= vacation.StartDate)
                throw new ArgumentException("ArgumentException: End-Date should be greater than Start-Date.");
        }

        public List<Vacation> GetByUserEmail(string userEmail)
        {
            return GetVacations().Where(v => v.UserId == userEmail.ToLower()).ToList();
        }
    }
}
