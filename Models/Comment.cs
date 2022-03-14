namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string ContentOfComment { get; set; }
        public string UserId { get; set; }
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Question Question { get; set; }
        public virtual Answer? Answer { get; set; }

        public Comment(string userId, int questionId, string contentOfComment)
        {
            this.UserId = userId;
            this.QuestionId = questionId;
            this.ContentOfComment = contentOfComment;

            User = new ApplicationUser();
            Question = new Question();
            Answer = new Answer();
        }

        public Comment(string userId, int questionId, int? answerId, string contentOfComment)
        {
            this.UserId = userId;
            this.QuestionId = questionId;
            this.AnswerId = answerId;
            this.ContentOfComment = contentOfComment;

            User = new ApplicationUser();
            Question = new Question();
            Answer = new Answer();
        }
    }

}
