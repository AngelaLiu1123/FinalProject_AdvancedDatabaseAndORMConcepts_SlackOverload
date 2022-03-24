using Microsoft.AspNetCore.Identity;

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Reputation { get; set; }
        public virtual List<Question> Questions { get; set; }
        public virtual List<Answer> Answers { get; set; }
        public virtual List<Comment> Comments { get; set; }
    }
}
