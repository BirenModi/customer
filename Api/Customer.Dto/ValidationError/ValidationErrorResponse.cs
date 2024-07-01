using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Dto.ValidationError
{
    public class ValidationErrorResponse
    {
        public string Message { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
    }
}
