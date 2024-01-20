using Clientes.Models;

namespace Clientes.Interface
{
    public interface ICliente
    {
        Task<IEnumerable<Cliente>> GetAllCustomers();
        Task<bool> InsertCustomer(Cliente cliente);
        Task<bool> UpdateCustomer(Cliente cliente);

        Task<bool> DeleteCustomer(int idCliente);
        Task<Cliente> ViewDetailsCustomer(int IdCliente);

        Task<ClienteInteres> ViewInterests(int idCliente);

        void AddNewConfigurationCustomer(ClienteInteres clienteInteres);

    }
}
