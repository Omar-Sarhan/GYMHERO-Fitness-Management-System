namespace Gym.Models
{
    public class Chart
    {
            public int UsersCount { get; set; }
            public int PlanCount { get; set; }
            public int SubscriptionsCount { get; set; }
            public List<User> Users { get; set; }
            public string MonthlyData { get; set; }
            public string AnnualData { get; set; }
    }
}
