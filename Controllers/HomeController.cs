using FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Data;
using FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models;
using FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Controllers
{
    [Authorize] //Only Logged in User can access this HomeController
    public class HomeController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public HomeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]  //Except for index page(User who don't log in still can access index page)
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString; 
            int pageSize = 10;//the max value is 10 records in every page
            var questions = _db.Questions.Include(q => q.User).Include(q => q.Answers);
            if (searchString != null)
            {
                var searchedQuestion = questions.Where(q => q.Title.Contains(searchString));
                return View(await PaginatedList<Question>.CreateAsync(searchedQuestion.AsNoTracking(), pageNumber ?? 1, pageSize));
            }
            
            if (sortOrder != null)
            {
                if (sortOrder == "Active")
                {
                    var mostActiveQuestions = questions.OrderByDescending(q => q.Answers.Count);
                    return View(await PaginatedList<Question>.CreateAsync(mostActiveQuestions.AsNoTracking(), pageNumber ?? 1, pageSize));
                }
            }

            var mostRecentQuestions = questions.OrderByDescending(q => q.DateOfCreate);
            return View(await PaginatedList<Question>.CreateAsync(mostRecentQuestions.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [Authorize(Roles = "Admin")] 
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string newRoleName)
        {
            try
            {
                await _roleManager.CreateAsync(new IdentityRole(newRoleName));
                await _db.SaveChangesAsync();
                ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

                if (await _roleManager.RoleExistsAsync(newRoleName))
                {
                    if (!await _userManager.IsInRoleAsync(currentUser, newRoleName))
                    {
                        await _userManager.AddToRoleAsync(currentUser, newRoleName);
                        await _db.AddRangeAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }

            return View();
        }

        [Authorize(Roles="Admin")]
        public IActionResult AddUserToRole()
        {
            ViewBag.UserNames = new SelectList(_db.Users, "Id", "UserName");
            ViewBag.RoleNames = new SelectList(_db.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userId, string roleId)
        {
            try
            {
                if (userId != null && roleId != null)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(userId);
                    IdentityRole role = await _roleManager.FindByIdAsync(roleId);

                    ViewBag.UserNames = new SelectList(_db.Users, "Id", "UserName");
                    ViewBag.RoleNames = new SelectList(_db.Roles, "Id", "Name");

                    if (user != null && role != null)
                    {
                        if (!await _userManager.IsInRoleAsync(user, role.Name))
                        {
                            await _userManager.AddToRoleAsync(user, role.Name);
                            await _db.SaveChangesAsync();
                            ViewBag.Message = "Sucessfully assigned the user in role";
                            return View();
                        }
                        else
                        {
                            ViewBag.Message = "The user already existed in the role";
                            return View();
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
            
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ShowAndDeleteQuestions()
        {
            return View(_db.Questions.Include(q => q.User));
        }

        [HttpPost]
        public IActionResult DeleteQuestion(int questionId)
        {
            //Cascade Delete(delete the data and other related data)
            //delete a question(first need to delete related comments and then answers, finally delete a question)
            Question questionToDelete = _db.Questions.First(q => q.Id == questionId);


            if (questionToDelete.Comments.Any())
            {
                _db.Comments.RemoveRange(questionToDelete.Comments);
            }

            if (questionToDelete.Answers.Any())
            {
                _db.Answers.RemoveRange(questionToDelete.Answers);
            }

            _db.Questions.Remove(questionToDelete);
            _db.SaveChanges();
            return RedirectToAction("ShowAndDeleteQuestions");
        }

        public IActionResult PostNewQuestion()
        {
            TagVM vm = new TagVM();
            vm.SelectedTags = new string[3];
            vm.AllTags = new SelectList(_db.Tags.ToList(), "Id", "Name");
            return View(vm);
        }

        [HttpPost]
        public IActionResult PostNewQuestion(string title, string body, TagVM tagVm)
        {
            string currentUserName = User.Identity.Name;
            ApplicationUser currentUser = _db.Users.First(u => u.UserName == currentUserName);
            string userId = currentUser.Id;

            Question newQuestion = new Question(title, body, userId);
            _db.Questions.Add(newQuestion);
            foreach (var tag in tagVm.SelectedTags)
            {
                QuestionTag questionTag = new QuestionTag(newQuestion.Id, int.Parse(tag));
                newQuestion.QuestionTags.Add(questionTag);
            }
            
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult QuestionDetails(int questionId, string? message)
        {
            Question question = _db.Questions.Include(q => q.Answers.OrderByDescending(a => a.IsCorrect)).ThenInclude(a => a.User)
                .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag).Include(q => q.User)
                .Include(q => q.Comments).First(q => q.Id == questionId);
            if(message != null)
            {
                ViewBag.Message = message;
            }
            return View(question);
        }

        
        public IActionResult ShowAllQuestionsByTag(int tagId)
        {
            List<QuestionTag> AllQuestionsByTag = _db.QuestionTags.Include(qt => qt.Question).Include(qt => qt.Tag).Where(qt => qt.TagId == tagId).ToList();
            return View(AllQuestionsByTag);
        }

        [HttpPost]
        public async Task<IActionResult> VoteQuestionOrAnswer(int questionId, int? answerId, string upOrDown)
        {
            try
            {
                //can't vote their own Qs/As
                //update related users's reputation who is up/down voted by others
                Question questionToVote = _db.Questions.Include(q => q.User).First(q => q.Id == questionId);
                ApplicationUser userToVote = questionToVote.User;
                ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

                if (userToVote != null)
                {
                    if (answerId != null)
                    {
                        Answer answerToVote = _db.Answers.Include(a => a.Question).Include(a => a.User).First(a => a.Id == answerId);
                        if (answerToVote.UserId == currentUser.Id)
                        {
                            return RedirectToAction("QuestionDetails",
                                new
                                {
                                    questionId = questionId,
                                    message = "Sorry! You can't vote your own Answers.",
                                });
                        }
                        ApplicationUser answeredUserToVote = answerToVote.User;
                        if (upOrDown == "up")
                        {
                            answerToVote.UpVote += 1;
                            answeredUserToVote.Reputation += 5;
                        }
                        else
                        {
                            answerToVote.UpVote -= 1;
                            answeredUserToVote.Reputation -= 5;
                        }
                    }
                    else
                    {
                        if (userToVote.Id == currentUser.Id)
                        {
                            return RedirectToAction("QuestionDetails",
                                new
                                {
                                    questionId = questionId,
                                    message = "Sorry! You can't vote your own Questions.",
                                });
                        }
                        if (upOrDown == "up")
                        {
                            questionToVote.UpVote += 1;
                            userToVote.Reputation += 5;
                        }
                        else
                        {
                            questionToVote.UpVote -= 1;
                            userToVote.Reputation -= 5;
                        }
                    }

                    _db.SaveChanges();
                    await _userManager.UpdateAsync(userToVote);
                }

                return RedirectToAction("QuestionDetails", new { questionId = questionId });
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", new {message = ex.Message});
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsCorrect(int questionId, int answerId)
        {
            //show the correct answer on the top of answer list and have some indicator
            ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            Question questionToMark = _db.Questions.Include(q => q.User).First(q => q.Id == questionId);
            ApplicationUser userOfQuestion = questionToMark.User;
            if (userOfQuestion.Id != currentUser.Id)
            {
                return RedirectToAction("QuestionDetails",
                    new
                    {
                        questionId = questionId,
                        message = "Sorry! You can't mark the answer as correct since you are not the question onwer.",
                    });
            }
            Answer correctAnswer = _db.Answers.Include(a => a.Question).ThenInclude(q => q.User).First(a => a.Id == answerId);
            correctAnswer.IsCorrect = true;
            _db.SaveChanges();
            return RedirectToAction("QuestionDetails", new { questionId = questionId });
        }

        [HttpPost]
        public async Task<IActionResult> PostAAnswer(int questionId, string answerContent)
        {
            ApplicationUser currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
            Answer newAnswer = new Answer(questionId, answerContent, currentUser.Id);
            if(newAnswer != null)
            {
                _db.Answers.Add(newAnswer);
                _db.SaveChanges();
            }
            
            return RedirectToAction("QuestionDetails", new { questionId = questionId });
        }

        [HttpPost]
        public async Task<IActionResult> AddAComment(int questionId, int? answerId, string comment)
        {
            try
            {
                if(comment != null)
                {
                    ApplicationUser userToComent = await _userManager.FindByNameAsync(User.Identity.Name);
                    Question questionToComment = _db.Questions.First(q => q.Id == questionId);

                    if (answerId != null)
                    {
                        Answer answerToComent = _db.Answers.First(a => a.Id == answerId);
                        Comment newComment = new Comment(userToComent.Id,questionId, answerId, comment);

                        newComment.Question = questionToComment;
                        newComment.User = userToComent;
                        newComment.Answer = answerToComent;

                        questionToComment.Comments.Add(newComment);
                        answerToComent.Comments.Add(newComment);
                        _db.Comments.Add(newComment);
                        _db.SaveChanges();
                    }
                    else
                    {
                        Comment newComment = new Comment(userToComent.Id,questionId, comment);
                        newComment.Question = questionToComment;
                        newComment.User = userToComent;
                        newComment.Answer = null;
                        questionToComment.Comments.Add(newComment);
                        _db.Comments.Add(newComment);
                        _db.SaveChanges();
                    }
                }
                else
                {
                    return RedirectToAction("QuestionDetails",
                                new
                                {
                                    questionId = questionId,
                                    message = "Sorry! You can't submit empty comment.",
                                });
                }
 
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
            return RedirectToAction("QuestionDetails", new
            {
                questionId = questionId,

            });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View();
        }
    }
}