namespace GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.API.src.data;
using GestionOrdinateurs.API.src.dto;
using GestionOrdinateurs.Shared.src.model;
using Microsoft.EntityFrameworkCore;

public class ReparationService{

    private readonly ParcInformatiqueContext _context ;
    private object _reparationLock = new object();
    private readonly JournalService? _journal;

    private readonly string _dossierLog = Path.Combine(Directory.GetCurrentDirectory(), "src","Logs", "Reparations_Logs", "Ordinateurs");

    private readonly string fichierLog = "reparations.log";

    public ReparationService(ParcInformatiqueContext context, JournalService? journal=null)
    {
        _context = context;
        _journal = journal;
    }
    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        var reparation = new Reparation(ordinateur.Id, "Réparation", DateTime.Now, "En cours","Admin");
        lock(_reparationLock)
        {
            _context.Reparations.Add(reparation);
        }
        await Task.Delay(5000); // Simule le temps de réparation
            reparation.Statut = "Terminée";
        
        lock(_reparationLock){
            _context.SaveChanges();
            ordinateur.Reparations.Add(reparation);
            var message = DateTime.Now.Date + " - "+ ordinateur.Nom+" réparé";
            _journal?.Ecrire(message, _dossierLog,fichierLog);
        }
        
    }

    public async Task<List<DtoReparationReponse>> ObtenirToutesLesReparationsAsync()
    {
        return await _context.Reparations.Select(r=> new DtoReparationReponse(
            r.Id, 
            r.OrdinateurId, 
            r.TypeReparation, 
            r.DateDebut, 
            r.Statut
        )).ToListAsync();
    }

    public async Task<List<HistoriqueReparationRecord>> ObtenirHistoriqueReparationsAsync()
    {
        return await _context.Reparations
            .Include(r => r.UnOrdinateur)
            .Select(r => new HistoriqueReparationRecord(
                r.OrdinateurId,
                r.UnOrdinateur != null ? r.UnOrdinateur.Nom : "inconnu",
                r.Technicien,
                r.TypeReparation,
                r.DateDebut
            )).ToListAsync();
    }
}