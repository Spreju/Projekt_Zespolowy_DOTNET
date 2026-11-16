using Microsoft.EntityFrameworkCore;
using Olympus.Core.Entities;

namespace Olympus.Data.Data;

public static class OlympusDataSeeder
{
    /// <summary>Wstawia bóstwa olimpijskie, jeśli tabela Bogowie jest pusta.</summary>
    public static async Task SeedBogowieAsync(ApplicationDbContext db)
    {
        if (await db.Bogowie.AnyAsync())
        {
            return;
        }

        db.Bogowie.AddRange(
            new Bog { Imie = "Zeus", Domena = "niebo i burze" },
            new Bog { Imie = "Hera", Domena = "małżeństwo i rodzina" },
            new Bog { Imie = "Posejdon", Domena = "morza i trzęsienia ziemi" },
            new Bog { Imie = "Demeter", Domena = "urodzaj i rolnictwo" },
            new Bog { Imie = "Atena", Domena = "mądrość i wojna" },
            new Bog { Imie = "Apollo", Domena = "światło, muzyka i proroctwa" },
            new Bog { Imie = "Artemida", Domena = "łowy i księżyc" },
            new Bog { Imie = "Ares", Domena = "wojna" },
            new Bog { Imie = "Afrodyta", Domena = "miłość i piękno" },
            new Bog { Imie = "Hefajstos", Domena = "kuźnia i ogień" },
            new Bog { Imie = "Hermes", Domena = "podróże i handel" },
            new Bog { Imie = "Dionizos", Domena = "wino i radość" });

        await db.SaveChangesAsync();
    }
}
