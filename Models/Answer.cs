namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public bool IsCorrect { get; set; }
        public string AnswerContent { get; set; }
        public int UpVote { get; set; }
        public int DownVote { get; set; }

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Comment>  Comments  {get; set;}

        public Answer()
        {
            Question = new Question();
            User = new ApplicationUser();
            Comments = new HashSet<Comment>();
        }

        public Answer(int questionId, string answerContent, string userId)
        {
            this.QuestionId = questionId;
            this.AnswerContent = answerContent;
            this.UserId = userId;
            
            Comments = new HashSet<Comment>();
        }
    }
}
