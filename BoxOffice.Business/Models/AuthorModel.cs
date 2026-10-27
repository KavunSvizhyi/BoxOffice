using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class AuthorModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public ICollection<int> PerfomanceIds { get; set; } = null!;
    }
}
