using System;
using System.Collections.Generic;


namespace Gym.Models;

public partial class Plan
{
    public decimal Planid { get; set; }

    public string Planname { get; set; } = null!;

  
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? Trainerid { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual User? Trainer { get; set; }
}
