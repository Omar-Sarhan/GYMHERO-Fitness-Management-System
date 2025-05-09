using System;
using System.Collections.Generic;

namespace Gym.Models;

public partial class Card
{
    public decimal Cardid { get; set; }

    public decimal Balance { get; set; }

    public decimal Password { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
