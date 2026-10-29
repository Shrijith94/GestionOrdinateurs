namespace GestionOrdinateurs.API.src.dto;
public record DtoReparationReponse(
    int Id, 
    int OrdinateurId,
    string TypeReparation, 
    DateTime DateDebut, 
    string Statut
);