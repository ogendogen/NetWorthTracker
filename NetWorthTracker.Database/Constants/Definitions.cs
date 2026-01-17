using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NetWorthTracker.Database.Constants;

public static class Definitions
{
    public static readonly ImmutableList<AssetDefinition> Assets = [
        new AssetDefinition { Name = "Mieszkanie/Dom" },
        new AssetDefinition { Name = "Nieruchomości" },
        new AssetDefinition { Name = "Meble" },
        new AssetDefinition { Name = "Sprzęt domowy" },
        new AssetDefinition { Name = "Samochód" },
        new AssetDefinition { Name = "Inne środki transportu" },
        new AssetDefinition { Name = "Kryptowaluty" },
        new AssetDefinition { Name = "Biżuteria" },
        new AssetDefinition { Name = "Ubezpieczenie na życie" },
        new AssetDefinition { Name = "Akcje/ETF" },
        new AssetDefinition { Name = "Obligacje" },
        new AssetDefinition { Name = "Rachunek bankowy" },
        new AssetDefinition { Name = "Lokaty" },
        new AssetDefinition { Name = "Metale szlachetne" },
        new AssetDefinition { Name = "Gotówka" },
        new AssetDefinition { Name = "Inne" }
    ];

    public static readonly ImmutableList<DebtDefinition> Debts = [
        new DebtDefinition { Name = "Kredyt hipoteczny" },
        new DebtDefinition { Name = "Kredyt hipoteczny" },
        new DebtDefinition { Name = "Karta kredytowa" },
        new DebtDefinition { Name = "Pożyczka" },
        new DebtDefinition { Name = "Limit w rachunku bankowym" }
    ];
}
