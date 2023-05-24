using System;
using System.Collections.Generic;

namespace AptechkaAPI
{

    public partial class Request
    {
        public int Id { get; set; }

        public int? DrugstoreId { get; set; }

        public DateTime? DateIn { get; set; }

        public DateTime? DateFinish { get; set; }

        public int? StatusId { get; set; }

        /*
        public virtual Drugstore? Drugstore { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

        public virtual Status? Status { get; set; }
        */
    }

}