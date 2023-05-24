using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptechkaWPF
{

    public partial class Address
    {
        public int Id { get; set; }

        public string? City { get; set; }

        public string? Street { get; set; }

        public string? Home { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                return City + ", " + Street + ", " + Home;
            }
        }

        public virtual ICollection<Drugstore> Drugstores { get; set; } = new List<Drugstore>();

        public virtual ICollection<Producer> Producers { get; set; } = new List<Producer>();
    }

}