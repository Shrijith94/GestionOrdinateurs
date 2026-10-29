namespace GestionOrdinateurs.API.src.dto;
public record DtoStatistique (
    int TotalOrdinateur, 
    int OrdiDisponibles, 
    double ValeurStock, 
    int NbReparations
);