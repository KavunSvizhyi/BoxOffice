using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Person : BaseEntity
    {
        [StringLength(30)]
        public string FirstName { get; set; } = null!;

        [StringLength(30)]
        public string LastName { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime BirthDate { get; set; }
    }
}
