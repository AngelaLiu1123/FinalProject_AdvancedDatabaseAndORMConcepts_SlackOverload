using FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.ViewModels
{
    public class TagVM
    {
        public string[] SelectedTags { get; set; }
        public IEnumerable<SelectListItem> AllTags { get; set; }
        public Question Question { get; set; }

        //public SelectedTag(SelectList allTags)
        //{
        //    this.SelectedTags = new string[3];
        //    this.AllTags = allTags;
        //}
    }
}
