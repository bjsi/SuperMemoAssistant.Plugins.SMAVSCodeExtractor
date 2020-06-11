using EnvDTE;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMACodeExtracts
{
    // TODO: Add project / solution name
    public class ProjectInfo
    {
        public string file { get; set; }
        public string language { get; set; }
        public string project { get; set; }
        public ProjectInfo(Document doc)
        {
            this.file = doc.Path + doc.Name;
            this.language = doc.Language;
            this.project = doc.ProjectItem?.ContainingProject?.Name ?? string.Empty;
        }
    }

    public class Extract
    {
        [AutoIncrement]
        public long Id { get; set; }
        public DateTime timestamp { get; set; }
        public string selectedCode { get; set; }
        public string comment { get; set; }
        public double priority { get; set; }
        public bool exported { get; set; }
        public string file { get; set; }
        public string language { get; set; }
        public string project { get; set; }
        public Extract(string selectedText, string comment, double priority, ProjectInfo info)
        {
            this.timestamp = DateTime.Now;
            this.selectedCode = selectedText;
            this.comment = comment;
            this.priority = priority;
            this.file = info.file;
            this.language = info.language;
            this.project = info.project;
        }
    }
}
