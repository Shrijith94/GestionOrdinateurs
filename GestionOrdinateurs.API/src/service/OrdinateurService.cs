namespace GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.Shared.src.model;
using GestionOrdinateurs.API.src.data;
using GestionOrdinateurs.Shared.src.enums;
using GestionOrdinateurs.API.src.dto;
using GestionOrdinateurs.API.src.events;
using GestionOrdinateurs.API.src.abonnee;
using Microsoft.EntityFrameworkCore;

/**
* Classe de service pour gérer les opérations liées aux ordinateurs
*/
public class OrdinateurService : IOrdinateurService
{

     private readonly ParcInformatiqueContext parcInformatiqueContext ;
     public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
     public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;

     public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;
     public readonly JournalService? _journalService;

    public OrdinateurService(ParcInformatiqueContext parcInformatiqueContextParam, JournalService? journalService = null)
    {
        parcInformatiqueContext = parcInformatiqueContextParam;
        _journalService = journalService;
    }

    /**
     * Format de réponses pour les méthodes get pour affiche pour un ordinateur, ses caractèristique et des réparations avec des DTO
     * DtoOrdinateurReparationsReponse qui regroupe DtoCaracteristiqueReponse et DtoReparationReponse. Donc chaque fois qu'on renvoie un ordinateur en réponse JSON
     * on renvoie DtoOrdinateurReparationsReponse maintenant (pour éviter les cycles entre Ordinateur et Réparation)
     * @param {int} id : l'identifiant de l'ordinateur à récupérer
     * @return {DtoOrdinateurReparationsReponse?} : retourne l'ordinateur correspondant à l'identifiant fourni, ou null si aucun ordinateur n'est trouvé
     */
    private static  DtoOrdinateurReparationsReponse ConvertirOrdinateurEnDto(Ordinateur ordinateur)
    {
        return new DtoOrdinateurReparationsReponse(
            ordinateur.Id,
            ordinateur.Nom,
            ordinateur.Etat.ToString(),
            ordinateur.Stock,
            ordinateur.Prix,
            ordinateur.Vendu,
            new DtoCaracteristiqueReponse(
                ordinateur.Caracteristiques.RamGo,
                ordinateur.Caracteristiques.StockageGo,
                ordinateur.Caracteristiques.TypeOrdi.ToString(),
                ordinateur.Caracteristiques.Marque,
                ordinateur.Caracteristiques.Processeur
            ),
            ordinateur.Reparations.Select(r => new DtoReparationReponse(
                r.Id,
                r.OrdinateurId,
                r.TypeReparation,
                r.DateDebut,
                r.Statut
            )).ToList()
        );
    }

    /**
     * Récupère un ordinateur par son identifiant
     * @param {int} id : l'identifiant de l'ordinateur à récupérer
     * @return {DtoOrdinateurReparationsReponse?} : retourne l'ordinateur correspondant à l'identifiant fourni, ou null si aucun ordinateur n'est trouvé
     */
    public async Task<DtoOrdinateurReparationsReponse?> ObtenirOrdinateurParIdAvecReparationsAsync(int id)
    {
        var ordinateur = await parcInformatiqueContext.Ordinateurs.Where(ordinateur => ordinateur.Id == id)
        .Include(o => o.Reparations)
        .Select(o => ConvertirOrdinateurEnDto(o))
        .FirstOrDefaultAsync();
        return ordinateur;
    }

    public async Task<Ordinateur?> ObtenirOrdinateurParIdAsync(int id)
    {
        return await parcInformatiqueContext.Ordinateurs.FirstOrDefaultAsync(ordinateur => ordinateur.Id == id);
    }

    public async Task<List<Ordinateur>> ObtenirTousLesOrdinateursAsync()
    {
        return await parcInformatiqueContext.Ordinateurs.ToListAsync();
    }

    /**
     * Récupère tous les ordinateurs
     * @return {List<DtoOrdinateurReparationsReponse>} : retourne une liste de tous les ordinateurs
     */
    public async Task<List<DtoOrdinateurReparationsReponse>> ObtenirTousLesOrdinateursAvecReparationsAsync()
    {
        return await parcInformatiqueContext.Ordinateurs.
        Include(o => o.Reparations).
        Select(o => ConvertirOrdinateurEnDto(o)).ToListAsync();
    }

    /**
     * Ajoute un nouvel ordinateur à la liste
     * @param {Ordinateur} ordinateur : l'ordinateur à ajouter
     */
    public async Task AjouterOrdinateurAsync(Ordinateur ordinateur)
    {
        parcInformatiqueContext.Ordinateurs.Add(ordinateur);
        await parcInformatiqueContext.SaveChangesAsync();
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs 
        { 
            Nom = ordinateur.Nom 
        });
        var message = DateTime.Now.Date + " - "+ ordinateur.Nom+" ajouté";
        var dossier = Path.Combine(Directory.GetCurrentDirectory(), "src","Logs", "Ajout_Logs", "Ordinateurs");
        _journalService?.Ecrire(message, dossier, "ajoutOrdinateur.log" );
    }

    /**
     * Supprime un ordinateur de la liste
     * @param {int} id : l'identifiant de l'ordinateur à supprimer
     */
    public async Task  SupprimerOrdinateurAsync (int id)
    {
        var ordinateurToRemove = await parcInformatiqueContext.Ordinateurs.FirstOrDefaultAsync(ordinateur => ordinateur.Id == id);
        if (ordinateurToRemove != null)
        {
            parcInformatiqueContext.Ordinateurs.Remove(ordinateurToRemove);
            await parcInformatiqueContext.SaveChangesAsync();
            OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs 
            { 
                Id = id
            });
            var message = DateTime.Now.Date + " - "+ ordinateurToRemove.Nom+" supprimé";
            var dossier = Path.Combine(Directory.GetCurrentDirectory(), "src","Logs", "Suppression_Logs", "Ordinateurs");
            _journalService?.Ecrire(message, dossier, "suppressionOrdinateur.log" );
        }
        else
        {
            throw new ArgumentException("Ordinateur pas trouvé");
        }
    }

    /**
     * Met à jour les informations d'un ordinateur existant
     * @param {int} id : l'identifiant de l'ordinateur à mettre à jour
     * @param {DtoOrdinateurFormatUpdate} updatedOrdinateur : les nouvelles informations de l'ordinateur
     */
    public async Task MettreAJourOrdinateurAsync(int id, DtoOrdinateurFormatUpdate OrdinateurPourMiseAJour)
    {
        var ordinateurToUpdate = await parcInformatiqueContext.Ordinateurs.FirstOrDefaultAsync(ordinateur => ordinateur.Id == id);
        if (ordinateurToUpdate != null)
        {
            ordinateurToUpdate.Nom = OrdinateurPourMiseAJour.Nom ?? ordinateurToUpdate.Nom;
            ordinateurToUpdate.Etat = OrdinateurPourMiseAJour.Etat switch
            {
                "EnService" => EtatOrdinateur.EnService,
                "EnPanne" => EtatOrdinateur.EnPanne,
                "EnMaintenance" => EtatOrdinateur.EnMaintenance,
                "HorsService" => EtatOrdinateur.HorsService,
                "Lent" => EtatOrdinateur.Lent,
                "Reparable" => EtatOrdinateur.Reparable,
                _ => ordinateurToUpdate.Etat
            };
            ordinateurToUpdate.Stock = OrdinateurPourMiseAJour.Stock ?? ordinateurToUpdate.Stock;
            ordinateurToUpdate.Prix = OrdinateurPourMiseAJour.Prix ?? ordinateurToUpdate.Prix;
            ordinateurToUpdate.Caracteristiques.RamGo = OrdinateurPourMiseAJour.RamGo ?? ordinateurToUpdate.Caracteristiques.RamGo;
            ordinateurToUpdate.Caracteristiques.StockageGo = OrdinateurPourMiseAJour.StockageGo ?? ordinateurToUpdate.Caracteristiques.StockageGo;
            ordinateurToUpdate.Vendu = OrdinateurPourMiseAJour.Vendu ?? ordinateurToUpdate.Vendu;
            await parcInformatiqueContext.SaveChangesAsync();
            OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs 
                { 
                    Id = id,
                });
            var message = DateTime.Now.Date + " - "+ ordinateurToUpdate.Nom+" modifié";
            var dossier = Path.Combine(Directory.GetCurrentDirectory(), "src","Logs", "Modification_Logs", "Ordinateurs");
            _journalService?.Ecrire(message, dossier, "modificationOrdinateur.log" );
        }
        else
        {
            throw new ArgumentException("Ordinateur pas trouvé");
        }
    }

    /**
     * Récupère les ordinateurs par marque
     * @param {string} marque : la marque des ordinateurs à récupérer
     * @return {List<DtoOrdinateurReparationsReponse>} : retourne une liste d'ordinateurs correspondant à la marque fournie
     */
    public async Task<List<DtoOrdinateurReparationsReponse>> ObtenirOrdinateursParMarque(string marque)
    {
        return await parcInformatiqueContext.Ordinateurs.Where(ordinateur => ordinateur.Caracteristiques.Marque == marque)
        .Include(ordinateur => ordinateur.Reparations)
        .Select(ordinateur => ConvertirOrdinateurEnDto(ordinateur)).ToListAsync();
    }


    /**
     * Récupère tous les ordinateurs avec un stock supérieur à zéro
     * @return {List<DtoOrdinateurReparationsReponse>} : retourne une liste d'ordinateurs avec un stock supérieur à zéro
     */
    public async Task<List<DtoOrdinateurReparationsReponse>> ObtenirOrdinateursStockSuperieurZeroAsync()
    {
        return await parcInformatiqueContext.Ordinateurs.Where(ordinateur => ordinateur.Stock > 0)
        .Include(ordinateur => ordinateur.Reparations)
        .Select(ordinateur => ConvertirOrdinateurEnDto(ordinateur)).ToListAsync();
    }

    /**
     * Récupère tous les ordinateurs par prix décroissant
     * @return {List<DtoOrdinateurReparationsReponse>} : retourne une liste d'ordinateurs triés par prix décroissant
     */
    public async Task<List<DtoOrdinateurReparationsReponse>> ObtenirOrdinateursParPrixDecroissantAsync()
    {
        return await parcInformatiqueContext.Ordinateurs.OrderByDescending(ordinateur => ordinateur.Prix)
        .Include(ordinateur => ordinateur.Reparations)
        .Select(ordinateur => ConvertirOrdinateurEnDto(ordinateur)).ToListAsync();
    }


    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
	    {
            {
                Etat: EtatOrdinateur.HorsService
            }
                => "Remplacement",

            {
                Etat: EtatOrdinateur.Lent,
                Caracteristiques:
                {
                    RamGo: < 8
                }
            }
                => "Ajouter RAM",

            _
                => "Aucune action"
	    };
    }

    /**
     * renvoie un tuple de statistique
     * @return {List<DtoOrdinateurReparationsReponse>} : retourne un tuple contenant le total d'ordi, le nombre ordi dispo, la valeur en prix de tous les
     * ordis réunis et le nombre de réparations
     */
    public (int TotalOrdinateur, int OrdiDisponibles, double ValeurStock, int NbReparations) CalculerStatistiques()
    {
        var totalOrdinateur = parcInformatiqueContext.Ordinateurs.Count();
        var ordiDisponibles = parcInformatiqueContext.Ordinateurs.Count(o => o.Stock > 0);
        var valeurStock = parcInformatiqueContext.Ordinateurs.Sum(o => o.Prix * o.Stock);
        var nbReparations = parcInformatiqueContext.Reparations.Count();

        return (totalOrdinateur, ordiDisponibles, valeurStock, nbReparations);
    }

    /**
     * renvoie un ordinateur par le nom grâce à l'indexation
     * pour la question 18, la solution est mis dans le service (car il contient le context et le controller ne le contient pas)
     *puis dans le controller la méthode est utilisé 
     * @return {List<DtoOrdinateurReparationsReponse>} : retourne un tuple contenant le total d'ordi, le nombre ordi dispo, la valeur en prix de tous les
     * ordis réunis et le nombre de réparations
     */
    public DtoOrdinateurReparationsReponse? ChercherParNom(string nom)
    {
        var inventaire = new InventaireOrdinateur();
        inventaire.Ordinateurs = parcInformatiqueContext.Ordinateurs.Include(ordinateur => ordinateur.Reparations).ToList();
        var ordinateur = inventaire[nom];
        if(ordinateur == null)
        {
            return null;
        }
        return ConvertirOrdinateurEnDto(ordinateur);
    }
}