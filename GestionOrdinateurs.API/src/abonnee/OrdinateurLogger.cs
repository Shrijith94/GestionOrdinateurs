namespace GestionOrdinateurs.API.src.abonnee;
using GestionOrdinateurs.API.src.events;

/**
 * Abonné aux évènements de OrdinateurService : affiche dans la console un message pour chaque action
 * (ajout, suppression, mis à jour)
 */
public class OrdinateurLogger
{
    public void AfficherMessageAjout(object? sender, OrdinateurAjouteEventArgs e)
    {
        Console.WriteLine("[EVENT] Nouvel ordinateur ajouté : "+ e.Nom);
    }

    public void AfficherMessageSuppression(object? sender, OrdinateurSupprimeEventArgs e)
    {
        Console.WriteLine("[EVENT] Ordinateur supprimé ID :"+ e.Id);
    }

    public void AfficherMessageMiseAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        Console.WriteLine("[EVENT] Ordinateur mis à jour ID : "+e.Id);
    }
}