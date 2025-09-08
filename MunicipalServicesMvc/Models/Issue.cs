using System;
using System.ComponentModel.DataAnnotations;
using MunicipalServicesMvc.Services;

namespace MunicipalServicesMvc.Models
{
    public class Issue
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Location { get; set; } = "";

        [Required, StringLength(100)]
        public string Category { get; set; } = "";

        [Required, StringLength(2000)]
        public string Description { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Received";

        // Custom data structure (no List/arrays)
        public AttachmentList Attachments { get; } = new AttachmentList();
    }
}
