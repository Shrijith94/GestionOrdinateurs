namespace GestionOrdinateurs.Shared.src.model;
using GestionOrdinateurs.Shared.src.enums;
public class Caracteristiques
{
    public int RamGo { get; set; }
    public TypeOrdinateur TypeOrdi { get; private set; }
    public int StockageGo { get; set; }
    public string Marque { get; private set; }
    public string Processeur { get; private set; }

    public Caracteristiques(int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string marque, string processeur)
    {
        RamGo = ramGo;
        StockageGo = stockageGo;
        TypeOrdi = typeOrdi;
        Marque = marque;
        Processeur = processeur;
    }
}