using System.ComponentModel.DataAnnotations;

namespace DCOClearinghouse.ViewModels
{
    public class LogInViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}