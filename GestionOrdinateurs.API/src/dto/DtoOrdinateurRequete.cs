namespace GestionOrdinateurs.API.src.dto;
/**
 * record utilisée pour représenter les données de requête (pour les requêtes POST)
 */
public record DtoOrdinateurRequete(
    string Nom, 
    string Marque, 
    string Processeur, 
    string Etat, 
    string TypeOrdi,
    int RamGo, 
    int? Stock = null, 
    double? Prix = null, 
    int? StockageGo = null);