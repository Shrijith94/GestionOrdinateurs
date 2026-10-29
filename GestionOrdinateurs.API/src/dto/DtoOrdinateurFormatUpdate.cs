namespace GestionOrdinateurs.API.src.dto;
/**
 * Record utilisée pour représenter les données nécessaires à la mise à jour (requête PUT)
 */
public record DtoOrdinateurFormatUpdate (
    string? Nom = null, 
    string? Etat = null,   
    int? Stock = null, 
    double? Prix = null, 
    int? RamGo = null , 
    int? StockageGo = null,
    bool? Vendu = null
);