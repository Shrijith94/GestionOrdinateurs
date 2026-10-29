public record DtoOrdinateur(
    int OrdinateurId,
    string Nom,
    string Etat,
    int Stock,
    double Prix,
    bool Vendu,
    DtoCaracteristique Caracteristiques,
    List<DtoReparation> Reparations
);

public record DtoCaracteristique(
    int RamGo,
    int StockageGo,
    string TypeOrdi,
    string Marque,
    string Processeur
);

public record DtoReparation(
    int Id,
    int OrdinateurId,
    string TypeReparation,
    DateTime DateDebut,
    string Statut
);

public record DtoOrdinateursResponse(List<DtoOrdinateur> Ordinateurs);