namespace GestionOrdinateurs.API.src.service;
public class StockService
{
    public int StockSSD { get; set; } = 10;

    public object StockLock = new object();

    public void ConsommerSSD()
    {
        lock(StockLock) 
        { 
            StockSSD--; 
        } 
    }
}