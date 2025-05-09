using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym.Models;

public partial class Subscription
{
    public decimal Subscriptionid { get; set; }

    public DateTime Fromdate { get; set; }

    public DateTime Todate { get; set; }

    public decimal? Memberid { get; set; }

    public decimal? Planid { get; set; }

    public virtual User? Member { get; set; }

    public virtual Plan? Plan { get; set; }
    [NotMapped]
    public string InvoicePath { get; set; } // خاصية غير مرتبطة بقاعدة البيانات

}
