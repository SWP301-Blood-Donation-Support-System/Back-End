using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.DTO
{
    public class EmailRequestDTO
    {
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Body content is required")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Recipient email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Recipient { get; set; }
    }

    public class BulkEmailRequestDTO
    {
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Body content is required")]
        public string Body { get; set; }

        [Required(ErrorMessage = "At least one recipient is required")]
        [MinLength(1, ErrorMessage = "At least one recipient is required")]
        public List<string> Recipients { get; set; }
    }

    public class EmailResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> FailedRecipients { get; set; }
    }
}