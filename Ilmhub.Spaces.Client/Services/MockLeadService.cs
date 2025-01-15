using Ilmhub.Spaces.Client.Interfaces;
using Ilmhub.Spaces.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ilmhub.Spaces.Client.Services;

public class MockLeadService : ILeadService
{
    private readonly List<Lead> leads;
    private readonly ICourseService courseService;

    public MockLeadService(ICourseService courseService)
    {
        this.courseService = courseService;
        leads = [];

        var random = new Random();
        var courses = courseService.GetCoursesAsync().Result.ToList();

        for (int i = 1; i <= 50; i++)
        {
            var status = (LeadStatus)random.Next(0, 10);
            leads.Add(new Lead
            {
                Id = i,
                Name = GetRandomUzbekName(),
                Phone = $"{random.Next(100000000, 999999999)}", // 9-digit format
                Status = status,
                Source = (LeadSource)random.Next(0, 3),
                CreatedAt = DateTime.Now.AddDays(-random.Next(1, 60)),
                ModifiedAt = DateTime.Now.AddDays(-random.Next(0, 30)),
                Notes = "Potentsial o'quvchi bilan suhbat o'tkazildi.",
                InterestedCourse = courses[random.Next(courses.Count)],
                Reason = status == LeadStatus.Lost ? GetRandomLostReason() : null
            });
        }
    }

    public ValueTask<IEnumerable<Lead>> GetLeadsAsync(CancellationToken cancellationToken = default) =>
        new(leads);

    public ValueTask<Lead?> GetLeadByIdAsync(int id, CancellationToken cancellationToken = default) =>
        new(leads.FirstOrDefault(l => l.Id == id));

    public ValueTask<Lead> CreateLeadAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        lead.Id = leads.Max(l => l.Id) + 1;
        lead.CreatedAt = DateTime.Now;
        lead.ModifiedAt = DateTime.Now;
        leads.Add(lead);
        return new ValueTask<Lead>(lead);
    }

    public ValueTask<Lead> UpdateLeadAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        var existingLead = leads.FirstOrDefault(l => l.Id == lead.Id);
        if (existingLead != null)
        {
            existingLead.Name = lead.Name;
            existingLead.Phone = lead.Phone;
            existingLead.Status = lead.Status;
            existingLead.Source = lead.Source;
            existingLead.ModifiedAt = DateTime.Now;
            existingLead.Notes = lead.Notes;
            existingLead.InterestedCourse = lead.InterestedCourse;
        }
        return new ValueTask<Lead>(existingLead ?? lead);
    }

    public ValueTask<bool> DeleteLeadAsync(int id, CancellationToken cancellationToken = default)
    {
        var lead = leads.FirstOrDefault(l => l.Id == id);
        if (lead != null)
        {
            leads.Remove(lead);
            return new ValueTask<bool>(true);
        }
        return new ValueTask<bool>(false);
    }

    private string GetRandomUzbekName()
    {
        var firstNames = new[] { "Alisher", "Gulnora", "Bobur", "Dilshod", "Nodira", "Jamshid", "Sarvar", "Oybek", "Malika", "Akmal", "Feruza", "Rustam", "Ziyoda", "Umid", "Nargiza" };
        var lastNames = new[] { "Qodirov", "Karimova", "Alimov", "Tursunov", "Azizova", "Nurmatov", "Abdullayev", "Toshpulatov", "Rahimova", "Xolmatov", "Umarova", "Saidov", "Yusupova", "Ismoilov", "Ergasheva" };

        var random = new Random();
        return $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}";
    }

    private string GetRandomLostReason()
    {
        var reasons = new[]
        {
            "Narxi qimmat",
            "Vaqti to'g'ri kelmadi",
            "Boshqa kursni tanladi",
            "Moliyaviy qiyinchiliklar",
            "Qiziqish yo'qoldi",
            "Oilaviy vaziyat",
            "Ish jadvalidagi o'zgarishlar",
            "Uzoq masofa",
            "Sog'liq muammolari",
            "Boshqa ta'lim muassasasini tanladi"
        };

        return reasons[new Random().Next(reasons.Length)];
    }
}