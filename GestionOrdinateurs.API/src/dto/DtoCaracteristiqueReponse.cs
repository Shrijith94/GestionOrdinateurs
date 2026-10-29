namespace GestionOrdinateurs.API.src.dto;
public record DtoCaracteristiqueReponse(
    int RamGo, 
    int StockageGo, 
    string TypeOrdi, 
    string Marque, 
    string Processeur
);