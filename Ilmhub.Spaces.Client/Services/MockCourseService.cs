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
        var courseNames = new[]
        {
            "English Phonics 1", "English Phonics 2", "English Phonics 3", "English Phonics 4",
            "English Starters", "English The Spire 1", "English The Spire 2", "English The Spire 3",
            "English The Spire 4", "English The Spire 5", "English The Spire 6", "English Level 7",
            "English Level 8", "IT Scratch", "IT Lego", "IT Extended Scratch", "Lego (Spike Prime)",
            "IT Savodxonlik", "IT App inventor", "IT Robotics (extended)", "IT Extended Python", "IT AutoCAD",
            "IT C++", "IT Frontend", "IT Backend"
        };

        courses = courseNames.Select((name, index) => new Course
        {
            Id = index + 1,
            Name = name,
            Description = GetDescription(name),
            Price = GetPrice(name),
            DurationInWeeks = GetDuration(name),
            Instructor = GetInstructor(index),
            StartDate = DateTime.Now.AddDays((index + 1) * 7),
            IsActive = true
        }).ToList();
    }

    private string GetDescription(string courseName)
    {
        if (courseName.StartsWith("English"))
            return "English language course focused on comprehensive learning";
        if (courseName.StartsWith("IT"))
            return "Programming and technology course for modern development";
        return "Comprehensive technical course for skill development";
    }

    private decimal GetPrice(string courseName)
    {
        if (courseName.StartsWith("English"))
            return 1200000M;
        if (courseName.StartsWith("IT"))
            return 1800000M;
        return 1500000M;
    }

    private int GetDuration(string courseName)
    {
        if (courseName.StartsWith("English"))
            return 12;
        if (courseName.StartsWith("IT"))
            return 16;
        return 14;
    }

    private string GetInstructor(int index)
    {
        var instructors = new[]
        {
            "John Smith", "Sarah Johnson", "Michael Brown", "Emma Davis",
            "David Wilson", "Lisa Anderson", "Robert Taylor", "Jennifer Martin"
        };
        return instructors[index % instructors.Length];
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