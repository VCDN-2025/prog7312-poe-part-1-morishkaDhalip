using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MunicipalServicesMvc.Models
{
    public class IssueCreateViewModel
    {
        [Required, StringLength(200)]
        public string Location { get; set; } = "";

        [Required, StringLength(100)]
        public string Category { get; set; } = "";

        [Required, StringLength(2000)]
        public string Description { get; set; } = "";

        // Framework-provided collection from form POST (allowed)
        public IFormFileCollection? Files { get; set; }
    }
}
