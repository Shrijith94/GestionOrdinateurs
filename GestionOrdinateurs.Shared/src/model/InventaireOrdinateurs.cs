using System.Dynamic;

namespace GestionOrdinateurs.Shared.src.model;
public class InventaireOrdinateur
{
    public List<Ordinateur> Ordinateurs = new();
    
    public Ordinateur this[int index]
    {
        get {return Ordinateurs[index];}
        set {Ordinateurs[index] = value;}
    }

    public Ordinateur? this[string reference]
    {
        get{return Ordinateurs.FirstOrDefault(o=> o.Nom == reference);}
        set
        {
            int position = Ordinateurs.FindIndex(o=> o.Nom == reference);
            if(position>=0 && value != null)
            {
                Ordinateurs[position] = value;
            }
        }
    }
}