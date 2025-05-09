using System;
using System.Collections.Generic;

namespace Gym.Models;

public partial class User
{
    public decimal Userid { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public decimal? Roleid { get; set; }

    public decimal? Profileid { get; set; }

    public decimal? Cardid { get; set; }

    public virtual Card? Card { get; set; }

    public virtual ICollection<Plan> Plans { get; set; } = new List<Plan>();

    public virtual Profile? Profile { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
}
