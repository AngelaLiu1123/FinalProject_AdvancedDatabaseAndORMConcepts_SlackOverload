using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models
{
    public class Question
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The length of the title shoud be more than 3 and less than 50 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(500,MinimumLength = 3, ErrorMessage = "The length of the title shoud be more than 3 and less than 500 characters.")]
        public string Body { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime DateOfCreate { get; set; }
        public DateTime? DateOfClose { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }

        public string? UserId { get; set; } //the data type of UserId should be string in ApplicationUser extends IdentityUser(table AspNetUser in Database)
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<QuestionTag> QuestionTags { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public Question(string title, string body, string userId)
        {
            this.Title = title;
            this.Body = body;
            this.UserId = userId;
            this.DateOfCreate = DateTime.Now;

            QuestionTags = new HashSet<QuestionTag>();
            Answers = new HashSet<Answer>();
            Comments = new HashSet<Comment>();
        }

        public Question()
        {
            QuestionTags = new HashSet<QuestionTag>();
            Answers = new HashSet<Answer>();
            Comments = new HashSet<Comment>();
        }
    }
}
