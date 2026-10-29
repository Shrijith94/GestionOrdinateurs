namespace GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.API.src.dto;
using GestionOrdinateurs.Shared.src.model;
public interface IOrdinateurService
{
    public  Task<DtoOrdinateurReparationsReponse?> ObtenirOrdinateurParIdAvecReparationsAsync(int id);
    public  Task<Ordinateur?> ObtenirOrdinateurParIdAsync(int id);
    public Task<List<Ordinateur>> ObtenirTousLesOrdinateursAsync();
    public  Task<List<DtoOrdinateurReparationsReponse>> ObtenirTousLesOrdinateursAvecReparationsAsync();
    public  Task AjouterOrdinateurAsync(Ordinateur ordinateur);
    public Task SupprimerOrdinateurAsync(int id);
    public Task MettreAJourOrdinateurAsync(int id, DtoOrdinateurFormatUpdate OrdinateurMisAJour);

    public Task<List<DtoOrdinateurReparationsReponse>> ObtenirOrdinateursParMarque(string marque);
    public Task<List<DtoOrdinateurReparationsReponse>> ObtenirOrdinateursStockSuperieurZeroAsync();
    public Task<List<DtoOrdinateurReparationsReponse>> ObtenirOrdinateursParPrixDecroissantAsync();

    public string Diagnostiquer(Ordinateur ordinateur);
    public (int TotalOrdinateur, int OrdiDisponibles, double ValeurStock, int NbReparations) CalculerStatistiques();

    public DtoOrdinateurReparationsReponse? ChercherParNom(string nom);
}