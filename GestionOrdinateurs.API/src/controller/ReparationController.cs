namespace GestionOrdinateurs.API.src.controller;
using GestionOrdinateurs.API.src.service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReparationController : ControllerBase
{
    private ReparationService _reparService;

    private IOrdinateurService _ordinateurService;

    private StockService _stockService;
    private readonly ILogger<ReparationController> _logger;
    
    public ReparationController(ReparationService reparationService, IOrdinateurService ordinateurService, StockService stockService, ILogger<ReparationController> logger)
    {
        _reparService = reparationService;
        _ordinateurService = ordinateurService;
        _stockService = stockService;
        _logger = logger;
    }

    /**
     * Simule la réparation de 5 ordinateurs en parallèle 
     * cette méthode est considéré comme l'ajout de réparation avec cette simulation. (la fonction se limite à 5 ordis en parallèle
     pour les tests)
     *
     * @return {IActionResult} : etourne un objet IActionResult indiquant le succès ou l'échec de la simulation
     */
    [HttpPost("simuler")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SimulerReparation()
    {
        var listeOrdinateurs = await _ordinateurService.ObtenirTousLesOrdinateursAsync();
        if (listeOrdinateurs.Count < 5)
        {
            return StatusCode(404,new { message = "Il n'y a pas assez d'ordi pour simuler (min 5)" });
        }
        await Task.WhenAll(
        _reparService.SimulerReparationAsync(listeOrdinateurs[0]),
        _reparService.SimulerReparationAsync(listeOrdinateurs[1]),
        _reparService.SimulerReparationAsync(listeOrdinateurs[2]),
        _reparService.SimulerReparationAsync(listeOrdinateurs[3]),
        _reparService.SimulerReparationAsync(listeOrdinateurs[4])
        );
        _logger.LogInformation("POST - api/reparation/simuler/ - les ordinateurs ont été simulés.");
        return StatusCode(201, new {message = "Les réparations ont été simulées pour les 5 premiers ordinateurs." });
    }

    /**
     * Récupère toutes les réparations
     * @return {IActionResult} : retourne un objet IActionResult avec la liste des réparations
     */
    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ObtenirToutesLesReparations()
    {
        var reparations = await _reparService.ObtenirToutesLesReparationsAsync();
        return StatusCode(200,reparations);
    }

    /**
     * Récupère le stock de SSD
     * @return {IActionResult} : retourne un objet IActionResult avec le nombre de SSD en stock
     */
    [HttpGet("stock-ssd")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> ObtenirStockSSD()
    {
        Parallel.For(
            0,
            10,
            i =>
            {
                Task.Delay(10).Wait();
                _stockService.ConsommerSSD();
            }
        );
         _logger.LogInformation("GET - api/reparation/stock-ssd/ - le stock de SSD : "+_stockService.StockSSD);
        return Ok(new { StockSSD = _stockService.StockSSD });
    }

    /**
     * Récupère toutes les réparations selon se format de DTO HistoriqueReparationRecord
     * @return {IActionResult} : retourne un objet IActionResult avec la liste des réparations sous se format HistoriqueReparationRecord
     */
    [HttpGet("historique")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenirHistoriqueReparations()
    {
        var historique = await _reparService.ObtenirHistoriqueReparationsAsync();
        return StatusCode(200, historique);
    }
}