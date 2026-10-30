using System.Collections.Concurrent;
using NetWorthTracker.Domain.Common.Interfaces;

namespace NetWorthTracker.IntegrationTests;

public sealed class RecordingEmailService : IEmailService
{
    public const string PostRegistrationKind = "post-registration";
    public const string PasswordResetKind = "password-reset";

    private readonly ConcurrentQueue<RecordedEmail> _sentEmails = new();

    public IReadOnlyCollection<RecordedEmail> SentEmails => _sentEmails.ToArray();

    public void SendPostRegistrationEmail(string username, string toAddress, string confirmationToken) =>
        _sentEmails.Enqueue(new RecordedEmail(PostRegistrationKind, username, toAddress, confirmationToken));

    public void SendPasswordResetEmail(string username, string toAddress, string resetToken) =>
        _sentEmails.Enqueue(new RecordedEmail(PasswordResetKind, username, toAddress, resetToken));

    public sealed record RecordedEmail(string Kind, string Username, string ToAddress, string Token);
}
