using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            Enrolleds = new HashSet<Enrolled>();
        }

        public uint ClassId { get; set; }
        public string Semester { get; set; } = null!;
        public uint Year { get; set; }
        public string Location { get; set; } = null!;
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public uint CourseId { get; set; }
        public string Professor { get; set; } = null!;

        public virtual Course Course { get; set; } = null!;
        public virtual Professor ProfessorNavigation { get; set; } = null!;
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<Enrolled> Enrolleds { get; set; }
    }
}
