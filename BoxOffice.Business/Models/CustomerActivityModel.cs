using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Models
{
    public class CustomerActivityModel
    {
        public int CustomerId { get; set; }

        public string FullName { get; set; } = null!;

        public decimal SpendsOnTickets { get; set; }

        public DateTime BirthDate { get; set; }

        public ICollection<int> TiIds { get; set; } = null!;
    }
}
