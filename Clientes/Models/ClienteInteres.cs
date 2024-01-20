namespace Clientes.Models
{
    public class ClienteInteres
    {
        public List<int> SelectedOptions { get; set; } = new List<int>();
        public Cliente Cliente { get; set; }
        public List<Interes> Intereses { get; set; } = new List<Interes>();
    }
}
