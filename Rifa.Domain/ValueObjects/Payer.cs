using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rifa.Domain.ValueObjects
{
    public class Payer
    {
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string DocumentType { get; }
        public string DocumentNumber { get; }

        public Payer(string email, string firstName, string lastName, string documentType, string documentNumber)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            DocumentType = documentType;
            DocumentNumber = documentNumber;
        }
    }
}
