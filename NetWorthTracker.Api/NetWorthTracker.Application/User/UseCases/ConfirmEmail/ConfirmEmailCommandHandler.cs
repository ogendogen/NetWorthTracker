using FluentResults;
using Microsoft.Extensions.Logging;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.User.Models.ConfirmEmail;
using NetWorthTracker.Application.User.Models.Login;
using NetWorthTracker.Application.User.UseCases.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Application.User.UseCases.ConfirmEmail;

public class ConfirmEmailCommandHandler : BaseRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>
{
    public ConfirmEmailCommandHandler(IServiceProvider? services, ILogger<BaseRequestHandler<ConfirmEmailCommand, ConfirmEmailResponse>> logger) : base(services, logger)
    {
    }

    protected override ValueTask<Result<ConfirmEmailResponse>> Handler(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
