using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DTO
{
    public class FeedbackDTO
    {
        public string FeedbackInfo { get; set; } = string.Empty;
        // Optional: You can add additional properties if needed
        // e.g., Rating, Response, etc.
        public int RegistrationId { get; set; }
    }
}
