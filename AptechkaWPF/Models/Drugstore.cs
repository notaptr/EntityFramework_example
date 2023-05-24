using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptechkaWPF
{

    public partial class Drugstore
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? AddressId { get; set; }

        public string? Telephone { get; set; }

        public string? PharmacyInn { get; set; }

        public virtual Address? Address { get; set; }

        public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    }

}