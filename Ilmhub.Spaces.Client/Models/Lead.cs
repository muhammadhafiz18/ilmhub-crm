namespace Ilmhub.Spaces.Client.Models;

public class Lead
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public LeadStatus Status { get; set; }
    public LeadSource Source { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? Notes { get; set; }
    public Course? InterestedCourse { get; set; }
    public string? Reason { get; set; } // New property for Lost reason
}

public enum LeadStatus
{
    Yangi,
    Aloqa,
    Boglanildi,
    QaytaBoglanish,
    Tugallanmagan,
    RegistratsiyaBolgan,
    SinovDarsda,
    Kelishilindi,
    Kelishilinmadi,
    Yoqotildi
}

public enum LeadSource
{
    Telegram,
    Instagram,
    Referral
}