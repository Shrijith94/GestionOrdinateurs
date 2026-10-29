namespace GestionOrdinateurs.Tests;
using FluentAssertions;
using GestionOrdinateurs.API.src.data;
using GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.Shared.src.enums;
using GestionOrdinateurs.Shared.src.model;
using Microsoft.EntityFrameworkCore;

public class ReparationServiceTest
{
    private readonly ParcInformatiqueContext _context;

    private readonly ReparationService _reparationService;
        private ParcInformatiqueContext CreerContexte()
{
    var options =
        new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    return new ParcInformatiqueContext(options);
}

public ReparationServiceTest()
    {
        _context = CreerContexte();
         _reparationService = new ReparationService(_context);
    }

[Fact]
public async Task TestCreationReparationSimulation()
    {
        var caracteristique = new Caracteristiques(8, 256, TypeOrdinateur.Portable, "Test", "Ryzen");
        var ordinateur = new Ordinateur("TestOrdi", EtatOrdinateur.EnService, 5, 500.5, caracteristique );

        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();

        //ajout d'une réparation
        await _reparationService.SimulerReparationAsync(ordinateur);


        var ordinateurMiseAJour = await _context.Ordinateurs.
        Include(o=>o.Reparations).
        FirstOrDefaultAsync(o => o.Id == ordinateur.Id) ;

        //verifier que la liste de réparations n'est pas vide et que les éléments de la réparations soient bon
        var reparationsEnBase = await _context.Reparations.Where(r => r.OrdinateurId == ordinateur.Id).ToListAsync();
        reparationsEnBase.Should().NotBeEmpty();
        var reperation = reparationsEnBase.First();

        reperation.TypeReparation.Should().Be("Réparation");
        reperation.Statut.Should().Be("Terminée");
        reperation.Technicien.Should().Be("Admin");

      //Verifier que la reparation est bien dans la liste de reparations de l'ordinateur
        ordinateurMiseAJour?.Reparations.Should().NotBeEmpty();
        ordinateurMiseAJour?.Reparations.Should().Contain(r=>r.Id == reperation.Id);

        

    }
}