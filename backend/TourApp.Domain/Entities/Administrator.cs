using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourApp.Domain.Entities
{
    public class Administrator : User
    {
        protected Administrator() { }

        public Administrator(string username, string email, string passwordHash,
                           string firstName, string lastName)
            : base(username, email, passwordHash, firstName, lastName)
        {
        }
    }
}
