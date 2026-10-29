namespace GestionOrdinateurs.API.src.dto;
public record HistoriqueReparationRecord(
    int OrdinateurId, 
    string NomOrdinateur,
    string Technicien, 
    string TypeReparation, 
    DateTime DateIntervention
);