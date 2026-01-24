using NetWorthTracker.Database.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NetWorthTracker.Database.Constants;

public static class Definitions
{
    public static readonly ImmutableList<Definition> Assets = [
        // Assets
        new Definition { Name = "Mieszkanie/Dom", Type = DefinitionType.Asset },
        new Definition { Name = "Nieruchomości", Type = DefinitionType.Asset },
        new Definition { Name = "Meble", Type = DefinitionType.Asset },
        new Definition { Name = "Sprzęt domowy", Type = DefinitionType.Asset },
        new Definition { Name = "Samochód", Type = DefinitionType.Asset },
        new Definition { Name = "Inne środki transportu", Type = DefinitionType.Asset },
        new Definition { Name = "Kryptowaluty", Type = DefinitionType.Asset },
        new Definition { Name = "Biżuteria", Type = DefinitionType.Asset },
        new Definition { Name = "Ubezpieczenie na życie", Type = DefinitionType.Asset },
        new Definition { Name = "Akcje/ETF", Type = DefinitionType.Asset },
        new Definition { Name = "Obligacje", Type = DefinitionType.Asset },
        new Definition { Name = "Rachunek bankowy", Type = DefinitionType.Asset },
        new Definition { Name = "Lokaty", Type = DefinitionType.Asset },
        new Definition { Name = "Metale szlachetne", Type = DefinitionType.Asset },
        new Definition { Name = "Gotówka", Type = DefinitionType.Asset },
        new Definition { Name = "Inne", Type = DefinitionType.Asset },
        // Debts
        new Definition { Name = "Kredyt gotówkowy", Type = DefinitionType.Debt },
        new Definition { Name = "Kredyt hipoteczny", Type = DefinitionType.Debt },
        new Definition { Name = "Karta kredytowa", Type = DefinitionType.Debt },
        new Definition { Name = "Pożyczka", Type = DefinitionType.Debt },
        new Definition { Name = "Limit w rachunku bankowym", Type = DefinitionType.Debt }
    ];
}
