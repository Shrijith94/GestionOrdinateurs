namespace GestionOrdinateurs.API.src.controller;
using GestionOrdinateurs.Shared.src.model;
using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.API.src.service;
using GestionOrdinateurs.API.src.dto;
using GestionOrdinateurs.Shared.src.enums;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    private IOrdinateurService _ordinateurService;
    private readonly ILogger<OrdinateursController> _logger;

    private readonly IConfiguration _configuration; 

    public OrdinateursController(IOrdinateurService ordinateurService, ILogger<OrdinateursController> logger, IConfiguration configuration)
    {
        _ordinateurService = ordinateurService;
        _logger = logger;
        _configuration = configuration;
    }
    /**
     * Récupère tous les ordinateurs et les renvoie sous forme de liste d'objets DtoOrdinateurReponse.
     *
     * @route GET /api/ordinateurs
     * @return {IActionResult} : retourne un objet IActionResult contenant la liste de tous les ordinateurs
     */
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ObtenirTousLesOrdinateurs()
    {
        _logger.LogInformation("GET - api/ordinateurs/ - Récupération de tous les ordinateurs");
        return StatusCode(200, new {ordinateurs = await _ordinateurService.ObtenirTousLesOrdinateursAvecReparationsAsync() });
    }


    /**
     * Récupère un ordinateur en fonction de son identifiant et le renvoie sous forme d'objet DtoOrdinateurReparationsReponse
     * (format de réponse de l'API).
     *
     * @route GET /api/ordinateurs/{id}
     * @param {int} id : identifiant de l'ordinateur à récupérer
     * @return {IActionResult} : retourne un objet IActionResult contenant l'ordinateur correspondant à l'identifiant fourni
     */
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> ObtenirOrdinateur(int id)
    {
        var OrdinateurExistant = await _ordinateurService.ObtenirOrdinateurParIdAvecReparationsAsync(id);
        if (OrdinateurExistant == null)
        {
            _logger.LogWarning("GET - api.ordinateurs/"+ id +" - Ordinateur non trouvé");
            return StatusCode(404, new { message = "Ordinateur non trouvé" });
        }
        _logger.LogInformation("GET - api/ordinateurs/"+ id +" - Récupération de l'ordinateur avec l'id " + id);
        return StatusCode(200, new { ordinateur = OrdinateurExistant });
    }

    /** 
     * Ajoute un nouvel ordinateur en fonction des informations fournies dans l'objet DtoOrdinateurRequete 
     * Le types de l'ordinateur (voir l'enum TypeOrdinateur) doivent être renseigné et aussi l'état de l'ordi (voir l'enum EtatOrdinateur)
     *
     * @route POST /api/ordinateurs 
     * @param {DtoOrdinateurRequete} ordinateur : objet DtoOrdinateurRequete contenant les informations de l'ordinateur à ajouter
     * @return {IActionResult} : retourne un objet IActionResult indiquant le succès ou l'échec de l'ajout de l'ordinateur
     */
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize (Roles = "Admin")]
    public async Task<IActionResult> AjouterOrdinateur([FromBody] DtoOrdinateurRequete ordinateur)
    {   
        Caracteristiques caracteristiques = new Caracteristiques(ordinateur.RamGo, ordinateur.StockageGo ?? 0, Enum.Parse<TypeOrdinateur>(ordinateur.TypeOrdi) , ordinateur.Marque, ordinateur.Processeur);
        Ordinateur newOrdinateur = new Ordinateur(ordinateur.Nom, Enum.Parse<EtatOrdinateur>(ordinateur.Etat), ordinateur.Stock ?? 0, ordinateur.Prix ?? 500, caracteristiques);
        newOrdinateur.Caracteristiques = caracteristiques;
        await _ordinateurService.AjouterOrdinateurAsync(newOrdinateur);
        _logger.LogInformation("POST - api/ordinateurs/ - Ajout d'un ordinateur");
        return StatusCode(201, new { message = "Ordinateur ajouté avec succès" });
    }

    /** 
     * Met à jour un ordinateur en fonction de son identifiant et des informations fournies dans l'objet DtoOrdinateurFormatUpdate 
     * (format de mise à jour de l'API).
     *
     * @route PUT /api/ordinateurs/{id}
     * @param {int} id : identifiant de l'ordinateur à mettre à jour
     * @param {DtoOrdinateurFormatUpdate} ordinateur : objet DtoOrdinateurFormatUpdate contenant les nouvelles informations de l'ordinateur
     * @return {IActionResult} : retourne un objet IActionResult indiquant le succès ou l'échec de la mise à jour de l'ordinateur
     */
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize (Roles = "Admin")]
    public async Task<IActionResult> MettreÀJourOrdinateur(int id, [FromBody] DtoOrdinateurFormatUpdate ordinateur)
    {
        var OrdinateurExistant = await _ordinateurService.ObtenirOrdinateurParIdAsync(id);
        if (OrdinateurExistant == null)
        {
           _logger.LogWarning("PUT - api.ordinateurs/"+ id +" - Ordinateur non trouvé");
            return StatusCode(404, new { message = "Ordinateur non trouvé" });
        }
        await _ordinateurService.MettreAJourOrdinateurAsync(id, ordinateur);
         _logger.LogInformation("PUT - api/ordinateurs/"+id+" - modification de l'ordinateur "+ id);
        return StatusCode(200, new { message = "Ordinateur mis à jour avec succès" });
    }

    /** 
     * Supprime un ordinateur en fonction de son identifiant.
     *
     * @route DELETE /api/ordinateurs/{id}
     * @param {int} id : identifiant de l'ordinateur à supprimer
     * @return {IActionResult} : retourne un objet IActionResult indiquant le succès ou l'échec de la suppression de l'ordinateur
     */
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize (Roles = "Admin")]
    public async Task<IActionResult> SupprimerOrdinateur(int id)
    {
        var OrdinateurExistant = await _ordinateurService.ObtenirOrdinateurParIdAsync(id);
        if (OrdinateurExistant == null)
        {
             _logger.LogWarning("DELETE - api.ordinateurs/"+ id +" - Ordinateur non trouvé");
            return StatusCode(404, new { message = "Ordinateur non trouvé" });
        }
        await _ordinateurService.SupprimerOrdinateurAsync(id);
        _logger.LogInformation("DELETE - api/ordinateurs/"+id+" - suppression de l'ordinateur "+ id);
        return StatusCode(204, new { message = "Ordinateur supprimé avec succès" });
    }

    /**
     * Récupère tous les ordinateurs d'une marque spécifique.
     *
     * @route GET /api/ordinateurs/marque/{marque}
     * @param {string} marque : marque des ordinateurs à récupérer
     * @return {IActionResult} : retourne un objet IActionResult contenant la liste des ordinateurs trouvés
     */
    [HttpGet("marque/{marque}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> ObtenirOrdinateursParMarque(string marque)
    {
        var ordinateurs = await _ordinateurService.ObtenirOrdinateursParMarque(marque);
        if (ordinateurs.Count == 0)
        {
            _logger.LogWarning("GET - api.ordinateurs/marque/"+ marque +" - Marque non trouvé");
            return StatusCode(404, new { message = "Aucun ordinateur trouvé pour cette marque" });
        }
        _logger.LogInformation("GET - api/ordinateurs/marque/"+marque+" - Récupération de l'ordinateur avec la marque "+ marque);
        return StatusCode(200, new { ordinateurs = ordinateurs });
    }

    /**
     * Récupère les infos de l'application avec un IConfiguration.
     *
     * @route GET /api/ordinateurs/info
     * @return {IActionResult} : retourne un objet IActionResult contenant les infos de l'application
     */
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public IActionResult ObtenirInfo()
    {
        var NomApplication = _configuration["Application:Name"];
        var VersionApplication = _configuration["Application:Version"];
        _logger.LogInformation("GET - api/ordinateurs/info");
        return StatusCode(200, new { info = new { Application = NomApplication, version = VersionApplication } });
    }


    /**
     * Récupère tous les ordinateurs avec un stock supérieur à zéro.
     *
     * @route GET /api/ordinateurs/stock
     * @return {IActionResult} : retourne un objet IActionResult contenant la liste des ordinateurs trouvés avec des stocks supérieur à zéro
     */
    [HttpGet("stock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ObtenirOrdinateursStockSuperieurZero()
    {
        _logger.LogInformation("GET - api/ordinateurs/stock - Récupération de tous les ordinateurs avec un stock supérieur à zéro");
        return StatusCode(200, new {ordinateurs = await _ordinateurService.ObtenirOrdinateursStockSuperieurZeroAsync() });
    }

/**
     * Récupère tous les ordinateurs par prix décroissant.
     *
     * @route GET /api/ordinateurs/prix
     * @return {IActionResult} : retourne un objet IActionResult contenant la liste des ordinateurs trouvés par prix décroissant
     */
    [HttpGet("prix")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ObtenirOrdinateursParPrixDecroissant()
    {
        _logger.LogInformation("GET - api/ordinateurs/prix - Récupération de tous les ordinateurs par prix décroissant");
        return StatusCode(200, new {ordinateurs = await _ordinateurService.ObtenirOrdinateursParPrixDecroissantAsync() });
    }

/**
     * Récupère les diagnostics d'un ordinateurs
     *
     * @route GET api/ordinateurs/{id}/diagnostic
     * @return {IActionResult} : retourne un objet IActionResult contenant le risque de l'odinateur
     */
    [HttpGet("{id}/diagnostic")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> DiagnostiquerOrdinateur(int id)
    {
        var ordinateur = await _ordinateurService.ObtenirOrdinateurParIdAsync(id);
        if (ordinateur == null)
        {
            _logger.LogWarning("GET - api.ordinateurs/"+ id +"/diagnostic - Ordinateur non trouvé");
            return StatusCode(404, new { message = "Ordinateur non trouvé" });
        }
        var diagnostic = _ordinateurService.Diagnostiquer(ordinateur);
        _logger.LogInformation("GET - api/ordinateurs/"+id+"/diagnostic - Diagnostic de l'ordinateur "+ id);
        return StatusCode(200, new { diagnostic = diagnostic });
    }

/**
     * Récupère des statisquess (en tuples) globale de l'application
     *
     * @route GET /api/ordinateurs/statistique
     * @return {IActionResult} : retourne un objet IActionResult contenant le tuple mappé avec un DTo DtoStatistique
     */
     [HttpGet("statistique")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public  IActionResult ObtenirStatistiques()
    {
        (int totalOrdinateur, int ordiDisponibles, double valeurStock, int nbReparations) statistique =  _ordinateurService.CalculerStatistiques();
           _logger.LogInformation("GET - api/ordinateur/statistique/ - Récupération des statistiques");
        return StatusCode(200, new {statistiques = new DtoStatistique(statistique.totalOrdinateur,statistique.ordiDisponibles, statistique.valeurStock, statistique.nbReparations)});
    }

    /**
     * Récupère les inventaires d'un ordi par son nomg (utilisation de l'indexation)
     *
     * @route GET /api/ordinateurs/inventaire/{nom}
     * @return {IActionResult} : retourne un objet IActionResult contenant un ordinateur récupéré à partir du nom (indexation)
     */
    [HttpGet("inventaire/{nom}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public IActionResult Chercher(string nom)
    {
         var OrdinateurExistant = _ordinateurService.ChercherParNom(nom);
        if (OrdinateurExistant == null)
        {
            _logger.LogWarning("GET - api/ordinateurs/inventaire/"+ nom +" - Ordinateur non trouvé");
            return StatusCode(404, new { message = "Ordinateur non trouvé" });
        }
        _logger.LogInformation("GET - api/ordinateurs/inventaire/"+ nom +" - Récupération de l'ordinateur avec le nom " + nom);
        return StatusCode(200, new { ordinateur = OrdinateurExistant });
        
    }

}