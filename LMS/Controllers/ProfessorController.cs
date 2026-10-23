using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo("LMSControllerTests")]

namespace LMS_CustomIdentity.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : Controller
    {
        private readonly LMSContext db;

        public ProfessorController(LMSContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat,
            string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Submissions(string subject, string num, string season, string year, string cat,
            string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname,
            string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query = from co in db.Courses
                join cl in db.Classes on co.CourseId equals cl.CourseId
                join e in db.Enrolleds on cl.ClassId equals e.ClassId
                where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                select new
                {
                    fname = e.UIdNavigation.FirstName,
                    lname = e.UIdNavigation.LastName,
                    uid = e.UId,
                    dob = e.UIdNavigation.Dob,
                    grade = e.Grade
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            // If category is null, return assignments from all categories
            if (category is null)
            {
                var query = from co in db.Courses
                    join cl in db.Classes on co.CourseId equals cl.CourseId
                    join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                    join a in db.Assignments on ac.CategoryId equals a.CategoryId
                    where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                    select new
                    {
                        aname = a.Name,
                        cname = ac.Name,
                        due = a.DueDate,
                        submissions = a.Submissions.Count()
                    };
                return Json(query.ToArray());
            }
            
            // Otherwise, return the assignments in a particular category
            else
            {
                var query = from co in db.Courses
                    join cl in db.Classes on co.CourseId equals cl.CourseId
                    join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                    join a in db.Assignments on ac.CategoryId equals a.CategoryId
                    where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                          && ac.Name == category
                    select new
                    {
                        aname = a.Name,
                        cname = ac.Name,
                        due = a.DueDate,
                        submissions = a.Submissions.Count()
                    };
                return Json(query.ToArray());
            }
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query = from co in db.Courses
                join cl in db.Classes on co.CourseId equals cl.CourseId
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                select new
                {
                    name = ac.Name,
                    weight = ac.Weight
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category,
            int catweight)
        {
            try
            {
                var query = from co in db.Courses
                    join cl in db.Classes on co.CourseId equals cl.CourseId
                    where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                    select cl.ClassId;

                AssignmentCategory ac = new AssignmentCategory();
                ac.Name = category;
                ac.Weight = (uint)catweight;
                ac.ClassId = query.First();
                db.AssignmentCategories.Add(ac);
                db.SaveChanges();
            }
            catch
            {
                return Json(new {success = false});
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category,
            string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            try
            {
                // Create an assignment
                var query = from co in db.Courses
                    join cl in db.Classes on co.CourseId equals cl.CourseId
                    join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                    where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                          && ac.Name == category
                    select ac.CategoryId;

                Assignment a = new();
                a.Name = asgname;
                a.Points = (uint)asgpoints;
                a.DueDate = asgdue;
                a.Contents = asgcontents;
                a.CategoryId = query.First();
                db.Assignments.Add(a);
                db.SaveChanges();

                // Get all students
                var queryStudents = from co in db.Courses
                    join cl in db.Classes on co.CourseId equals cl.CourseId
                    join e in db.Enrolleds on cl.ClassId equals e.ClassId
                    where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                    select e.UId;

                var students = queryStudents.ToArray();

                // Autograde for each student
                foreach (var uid in students)
                {
                    Autograde(subject, num, season, year, uid);
                }

                db.SaveChanges();
            }
            catch
            {
                return Json(new {success = false});
            }

            return Json(new {success = true});
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year,
            string category, string asgname)
        {
            var query = from co in db.Courses
                join cl in db.Classes on co.CourseId equals cl.CourseId
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                join a in db.Assignments on ac.CategoryId equals a.CategoryId
                join s in db.Submissions on a.AssignmentId equals s.AssignmentId
                where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                      && ac.Name == category && a.Name == asgname
                select new
                {
                    fname = s.UIdNavigation.FirstName,
                    lname = s.UIdNavigation.LastName,
                    uid = s.UId,
                    time = s.Time,
                    score = s.Score
                };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student whose submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category,
            string asgname, string uid, int score)
        {
            try
            {
                var query = from co in db.Courses
                    join cl in db.Classes on co.CourseId equals cl.CourseId
                    join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                    join a in db.Assignments on ac.CategoryId equals a.CategoryId
                    join s in db.Submissions on a.AssignmentId equals s.AssignmentId
                    where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                          && ac.Name == category && a.Name == asgname && s.UId == uid
                    select s;
                
                // If there is a submission, set the score
                if (query.Any())
                {
                    query.First().Score = (uint)score;
                }

                db.SaveChanges();

                // Autograde
                Autograde(subject, num, season, year, uid);

                db.SaveChanges();
            }
            catch
            {
                return Json(new {success = false});
            }

            return Json(new {success = true});
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from co in db.Courses
                join cl in db.Classes on co.CourseId equals cl.CourseId
                where cl.Professor == uid
                select new
                {
                    subject = co.Subject,
                    number = co.Number,
                    name = co.Name,
                    season = cl.Semester,
                    year = cl.Year
                };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Recalculate a student's grade (autograding)
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="num"></param>
        /// <param name="season"></param>
        /// <param name="year"></param>
        /// <param name="uid"> the student to recalculate the grade for</param>
        private void Autograde(string subject, int num, string season, int year, string uid)
        {
            // Get all assignment categories
            var query = from co in db.Courses
                join cl in db.Classes on co.CourseId equals cl.CourseId
                join ac in db.AssignmentCategories.Include(ac => ac.Assignments).ThenInclude(a => a.Submissions)
                    on cl.ClassId equals ac.ClassId
                where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                select ac;

            var categories = query.ToArray();
            
            double categoryTotal = 0;
            double categoryWeights = 0;
            
            // For each assignment category
            foreach (var ac in categories)
            {
                // Assignment category has assignments
                if (ac.Assignments.Count > 0)
                {
                    double totalPoints = 0;
                    double earnedPoints = 0;
                    
                    // Compute total points and earned points for each category
                    foreach (var a in ac.Assignments)
                    {
                        totalPoints += a.Points;
                        foreach (var s in a.Submissions)
                        {
                            if (s.UId == uid)
                            {
                                earnedPoints += s.Score;
                            }
                        }
                    }
            
                    double percentage = earnedPoints / totalPoints; // percentage of all assignments in category
                    percentage *= ac.Weight;
                    categoryWeights += ac.Weight;
                    categoryTotal += percentage;
                }
            }
            
            // Compute total percentage the student earned in the class
            double scaleFactor = 1;
            if(categoryWeights != 0)
                scaleFactor = 100 / categoryWeights;
            double classPercentage = categoryTotal * scaleFactor; // total percentage student earned in class
            
            // Get the letter grade
            string grade = GetGrade(classPercentage);
            
            // Find the enrolled student and set their grade
            var queryEnrolled = from co in db.Courses
                join cl in db.Classes on co.CourseId equals cl.CourseId
                join e in db.Enrolleds on cl.ClassId equals e.ClassId
                where co.Subject == subject && co.Number == num && cl.Semester == season && cl.Year == year
                      && e.UId == uid
                select e;
            
            var student = queryEnrolled.First();
            student.Grade = grade;
            
            db.SaveChanges();
        }

        /// <summary>
        /// Convert points to a letter grade
        /// </summary>
        /// <param name="points"></param>
        /// <returns> the letter grade </returns>
        private string GetGrade(double points)
        {
            if (points >= 93)
                return "A";
            else if (points >= 90)
                return "A-";
            else if (points >= 87)
                return "B+";
            else if (points >= 83)
                return "B";
            else if (points >= 80)
                return "B-";
            else if (points >= 77)
                return "C+";
            else if (points >= 73)
                return "C";
            else if (points >= 70)
                return "C-";
            else if (points >= 67)
                return "B+";
            else if (points >= 63)
                return "D";
            else if (points >= 60)
                return "D-";
            else
                return "E";
        }

        /*******End code to modify********/
    }
}