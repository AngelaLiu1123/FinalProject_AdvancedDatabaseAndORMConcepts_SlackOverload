using Microsoft.AspNetCore.Identity;

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Reputation { get; set; }
        public virtual List<Question> Questions { get; set; }
    }
}
