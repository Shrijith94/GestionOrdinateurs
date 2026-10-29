namespace GestionOrdinateurs.Shared.src.model;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using GestionOrdinateurs.Shared.src.enums;

[Table("Ordinateur")]
public class Ordinateur
{
    public int Id { get; private set; }
    public string Nom { get; set; }
    public EtatOrdinateur Etat { get; set; }

    [DefaultValue(0)]
    public int Stock { get; set; }

    [DefaultValue(500)]
    public double Prix { get; set; }

    public bool Vendu { get; set; } = false;

    public Caracteristiques Caracteristiques { get; set; }

    public List<Reparation> Reparations { get; set; } = new();

    // Contructeur vide utilisé uniquement par EF Core pour que la migration marche
    public Ordinateur()
    {
        Nom = string.Empty;
        Caracteristiques = null!;
    }

    public Ordinateur(string nom, EtatOrdinateur etat, int stock, double prix, Caracteristiques caracteristiques)
    {
        Nom = nom;
        Etat = etat;
        Stock = stock;
        Prix = prix;
        Caracteristiques = caracteristiques;
    }
}