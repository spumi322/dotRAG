using dotRAG.API.Models;

namespace dotRAG.API.Application;

internal interface ISettingsService
{
    SettingsDto GetEffective();
    Task<SaveSettingsResult> SaveAsync(SettingsDto incoming, CancellationToken ct = default);
    Task ResetToDefaultsAsync(CancellationToken ct = default);
}
