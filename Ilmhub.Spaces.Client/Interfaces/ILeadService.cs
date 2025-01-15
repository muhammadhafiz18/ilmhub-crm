using Ilmhub.Spaces.Client.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ilmhub.Spaces.Client.Interfaces;

public interface ILeadService
{
    ValueTask<IEnumerable<Lead>> GetLeadsAsync(CancellationToken cancellationToken = default);
    ValueTask<Lead?> GetLeadByIdAsync(int id, CancellationToken cancellationToken = default);
    ValueTask<Lead> CreateLeadAsync(Lead lead, CancellationToken cancellationToken = default);
    ValueTask<Lead> UpdateLeadAsync(Lead lead, CancellationToken cancellationToken = default);
    ValueTask<bool> DeleteLeadAsync(int id, CancellationToken cancellationToken = default);
}