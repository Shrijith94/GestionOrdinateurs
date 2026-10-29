namespace GestionOrdinateurs.Tests;
using GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.Shared.src.enums;
using GestionOrdinateurs.Shared.src.model;
using GestionOrdinateurs.API.src.data;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

public class OrdinateurServiceTest
{
    private readonly OrdinateurService _ordinateurService;
    private readonly ParcInformatiqueContext _context;

    private ParcInformatiqueContext CreerContexte()
{
    var options =
        new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    return new ParcInformatiqueContext(options);
}

    public OrdinateurServiceTest()
    {
        _context = CreerContexte();
         _ordinateurService = new OrdinateurService(_context);
    }

    [Fact]
    public async Task TestAjouterOrdinateur()
    {
        Caracteristiques caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable, "TestBrand", "TestCPU");
        List<Reparation> reparations = new List<Reparation>();
        reparations.Add(new Reparation(1, "Réparation", DateTime.Now, "En cours","Admin"));
        reparations.Add(new Reparation(1, "Réparation", DateTime.Now, "Terminée","Admin"));
        var ordinateur = new Ordinateur("TestPC",  EtatOrdinateur.EnPanne, 5, 1000,caracteristiques );
        ordinateur.Reparations = reparations;

        await _ordinateurService.AjouterOrdinateurAsync(ordinateur);
        var result = await _ordinateurService.ObtenirOrdinateurParIdAsync(ordinateur.Id);

        result.Should().BeEquivalentTo(ordinateur);
    }

    [Fact]
    public async Task TestSupprimerOrdinateur()
    {
        Caracteristiques caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable, "TestBrand", "TestCPU");
        List<Reparation> reparations = new List<Reparation>();
        reparations.Add(new Reparation(1, "Réparation", DateTime.Now, "En cours","Admin"));
        reparations.Add(new Reparation(1, "Réparation", DateTime.Now, "Terminée","Admin"));
        var ordinateur = new Ordinateur("TestPC",  EtatOrdinateur.EnPanne, 5, 1000,caracteristiques );
        ordinateur.Reparations = reparations;
        await _ordinateurService.AjouterOrdinateurAsync(ordinateur);

        await _ordinateurService.SupprimerOrdinateurAsync(ordinateur.Id);
        var result = await _ordinateurService.ObtenirOrdinateurParIdAsync(ordinateur.Id);

        result.Should().BeNull();
    }
}