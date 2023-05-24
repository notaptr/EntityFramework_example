using System;
using System.Collections.Generic;

namespace AptechkaWPF
{

    public partial class Status
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Code { get; set; }

        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }

}