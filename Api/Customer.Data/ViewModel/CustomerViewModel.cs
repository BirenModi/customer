using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Data.ViewModel
{
    public class CustomerViewModel
    {
        public Guid? CustomerId { get; set; }
        public string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public byte[] ProfileImage { get; set; }
    }
}
