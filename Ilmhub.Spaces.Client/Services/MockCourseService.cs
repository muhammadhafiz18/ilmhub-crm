using Ilmhub.Spaces.Client.Interfaces;
using Ilmhub.Spaces.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ilmhub.Spaces.Client.Services;

public class MockCourseService : ICourseService
{
    private readonly List<Course> courses;

    public MockCourseService()
    {
        courses = new List<Course>
        {
            new Course { Id = 1, Name = "Web Dasturlash", Description = "HTML, CSS, va JavaScript asoslari", Price = 1500000, DurationInWeeks = 12, Instructor = "Alisher Qodirov", StartDate = DateTime.Now.AddDays(7), IsActive = true },
            new Course { Id = 2, Name = "Python Asoslari", Description = "Python dasturlash tili bo'yicha boshlang'ich kurs", Price = 1200000, DurationInWeeks = 8, Instructor = "Gulnora Karimova", StartDate = DateTime.Now.AddDays(14), IsActive = true },
            new Course { Id = 3, Name = "Ma'lumotlar Bazasi", Description = "SQL va PostgreSQL bo'yicha amaliy kurs", Price = 1800000, DurationInWeeks = 10, Instructor = "Bobur Alimov", StartDate = DateTime.Now.AddDays(21), IsActive = true },
            new Course { Id = 4, Name = "Java Dasturlash", Description = "Java dasturlash tili va OOP kontseptsiyalari", Price = 2000000, DurationInWeeks = 16, Instructor = "Dilshod Tursunov", StartDate = DateTime.Now.AddDays(30), IsActive = true },
            new Course { Id = 5, Name = "Mobile Dasturlash", Description = "Android va iOS uchun mobil ilovalar yaratish", Price = 2500000, DurationInWeeks = 20, Instructor = "Nodira Azizova", StartDate = DateTime.Now.AddDays(45), IsActive = true },
            new Course { Id = 6, Name = "Frontend Frameworklar", Description = "React va Vue.js bo'yicha amaliy kurs", Price = 1800000, DurationInWeeks = 14, Instructor = "Jamshid Nurmatov", StartDate = DateTime.Now.AddDays(60), IsActive = true },
            new Course { Id = 7, Name = "Backend Dasturlash", Description = "Node.js va Express.js orqali server yaratish", Price = 2200000, DurationInWeeks = 16, Instructor = "Sarvar Abdullayev", StartDate = DateTime.Now.AddDays(75), IsActive = true },
            new Course { Id = 8, Name = "DevOps Asoslari", Description = "Docker, Kubernetes va CI/CD jarayonlari", Price = 2800000, DurationInWeeks = 18, Instructor = "Oybek Toshpulatov", StartDate = DateTime.Now.AddDays(90), IsActive = true },
            new Course { Id = 9, Name = "Kiberhavfsizlik", Description = "Tarmoq va dasturiy ta'minot xavfsizligi asoslari", Price = 3000000, DurationInWeeks = 20, Instructor = "Malika Rahimova", StartDate = DateTime.Now.AddDays(105), IsActive = true },
            new Course { Id = 10, Name = "Sun'iy Intellekt", Description = "Machine Learning va Deep Learning asoslari", Price = 3500000, DurationInWeeks = 24, Instructor = "Akmal Xolmatov", StartDate = DateTime.Now.AddDays(120), IsActive = true },
            // ... (add 10 more courses with similar Uzbek-themed information)
        };
    }

    public ValueTask<IEnumerable<Course>> GetCoursesAsync(CancellationToken cancellationToken = default) => 
        new(courses);

    public ValueTask<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default) => 
        new(courses.FirstOrDefault(c => c.Id == id));

    public ValueTask<Course> CreateCourseAsync(Course course, CancellationToken cancellationToken = default)
    {
        course.Id = courses.Max(c => c.Id) + 1;
        courses.Add(course);
        return new ValueTask<Course>(course);
    }

    public ValueTask<Course> UpdateCourseAsync(Course course, CancellationToken cancellationToken = default)
    {
        var existingCourse = courses.FirstOrDefault(c => c.Id == course.Id);
        if (existingCourse != null)
        {
            existingCourse.Name = course.Name;
            existingCourse.Description = course.Description;
            existingCourse.Price = course.Price;
            existingCourse.DurationInWeeks = course.DurationInWeeks;
            existingCourse.Instructor = course.Instructor;
            existingCourse.StartDate = course.StartDate;
            existingCourse.IsActive = course.IsActive;
        }
        return new ValueTask<Course>(existingCourse ?? course);
    }

    public ValueTask<bool> DeleteCourseAsync(int id, CancellationToken cancellationToken = default)
    {
        var course = courses.FirstOrDefault(c => c.Id == id);
        if (course != null)
        {
            courses.Remove(course);
            return new ValueTask<bool>(true);
        }
        return new ValueTask<bool>(false);
    }
}