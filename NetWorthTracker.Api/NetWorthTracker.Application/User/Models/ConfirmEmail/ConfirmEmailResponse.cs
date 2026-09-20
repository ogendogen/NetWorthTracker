using System;
using System.Collections.Generic;
using System.Text;

namespace NetWorthTracker.Application.User.Models.ConfirmEmail;

public sealed record ConfirmEmailResponse(bool IsEmailConfirmed);
