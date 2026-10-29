using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Genre : BaseEntity
    {
        [StringLength(50)]
        public string GenreName { get; set; } = null!;

        public ICollection<Performance> Performances { get; set; } = null!;
    }
}
