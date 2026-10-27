using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BoxOffice.Data.Entities
{
    public class Person : BaseEntity
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime BirthDate { get; set; }
    }
}
