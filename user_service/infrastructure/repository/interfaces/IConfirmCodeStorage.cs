namespace user_service.infrastructure.repository.interfaces;

public interface IConfirmCodeStorage
{
    protected string PREFIX { get; }
    public Task<string> GenerateAndSaveCode(string email, TimeSpan? lifeTime = null);
    public Task<bool> ConfirmEmail(string email, string code);
}
