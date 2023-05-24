using System;
using System.Collections.Generic;

namespace AptechkaWPF
{

    public partial class Producer
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? LicanceNumber { get; set; }

        public int? AddressId { get; set; }

        public string? Telephone { get; set; }

        public virtual Address? Address { get; set; }

        public virtual ICollection<Drug> Drugs { get; set; } = new List<Drug>();
    }

}