using System;
using System.Collections.Generic;
using System.Text;

namespace BoxOffice.Business.Validation
{
    public class TheatreException : Exception
    {
        public TheatreException()
        {
        }

        public TheatreException(string message)
            : base(message)
        {
        }

        public TheatreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
