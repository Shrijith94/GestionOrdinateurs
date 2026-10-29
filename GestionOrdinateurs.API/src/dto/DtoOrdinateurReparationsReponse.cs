namespace GestionOrdinateurs.API.src.dto;
public record DtoOrdinateurReparationsReponse(
    int OrdinateurId, 
    string Nom,
    string Etat,
    int Stock,
    double Prix,
    bool Vendu,
    DtoCaracteristiqueReponse Caracteristiques,
    List<DtoReparationReponse> Reparations
);