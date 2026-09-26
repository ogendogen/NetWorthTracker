namespace NetWorthTracker.Application.Common.Models;

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;

    public int SmtpPort { get; set; } = 0;

    public string SmtpUsername { get; set; } = string.Empty;

    public string SmtpPassword { get; set; } = string.Empty;
}
