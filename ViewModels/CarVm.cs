using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.ViewModels
{
    public class CarVm
    {
        public string[] SelectedCars { get; set; }
        public IEnumerable<SelectListItem> AllCars { get; set; }
    }
}
