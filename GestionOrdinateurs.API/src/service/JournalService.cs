public class JournalService
{
    public void Ecrire(string message, string dossier, string fichier)
    {
        using var writer =
            new StreamWriter(Path.Combine(dossier,fichier),
                append: true);

        writer.WriteLine(message);
    }
}