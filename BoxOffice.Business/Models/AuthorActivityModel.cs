using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class AuthorActivityModel
    {
        public int AuthorId { get; set; }

        public string FullName { get; set; } = null!;

        public decimal IncomeSum { get; set; }

        public int BestPerfomanceId { get; set; }
    }
}
