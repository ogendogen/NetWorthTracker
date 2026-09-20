using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Domain.Common.Interfaces;

public interface IEmailService
{
    public void SendPostRegistrationEmail(string username, string toAddress, string confirmationToken);
}
