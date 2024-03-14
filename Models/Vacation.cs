namespace AirbnbProj2.Models
{
    public class Vacation
    {
        private int id;
        private string userId;
        private int flatId;
        private DateTime startDate;
        private DateTime endDate;

        private static readonly List<Vacation> vacationsList = new List<Vacation>();

        public Vacation() { }

        public Vacation(int id, string userId, int flatId, DateTime startDate, DateTime endDate)
        {
            Id = id;
            UserId = userId;
            FlatId = flatId;
            StartDate = startDate;
            EndDate = endDate;
        }

        //public int Id { get => id; internal set => id = value; }
        public int Id { get => id; set => id = value; }
        public string UserId { get => userId; set => userId = value; }
        public int FlatId { get => flatId; set => flatId = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public DateTime EndDate { get => endDate; set => endDate = value; }

    }
}
