using Clientes.Controllers;
using Clientes.Models;
using Dapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using Microsoft.Identity.Client;
using MySqlX.XDevAPI;

namespace Clientes.Interface
{
    public class ClienteRepositorio : ICliente
    {
        private readonly SQLConfiguration _configuration;

        public ClienteRepositorio(SQLConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected MySqlConnection dbConnection()
        {
            return new MySqlConnection(_configuration.ConnectionString);
        }

        public Task<IEnumerable<Cliente>> GetAllCustomers()
        {
            var db = dbConnection();
            var queryExpression = @"SELECT * FROM clientes";

            return db.QueryAsync<Cliente>(queryExpression, new { });
        }

        public async Task<bool> InsertCustomer(Cliente cliente)
        {
            var db = dbConnection();

            var queryExpression = "INSERT INTO clientes (Nombre, Apellido, Email, Telefono, PaisResidencia, FechaNacimiento, DocumentoIdentificacion) " +
                "VALUES (@nombre, @apellido, @email, @telefono, @paisResidencia, @fechaNacimiento, @documentoIdenficacion)";
            int rowsAffected = 0;

            using (MySqlCommand cmd = new MySqlCommand(queryExpression, db))
            {
                await db.OpenAsync();

                cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@email", cliente.Email);
                cmd.Parameters.AddWithValue("@telefono", cliente.Telefono);
                cmd.Parameters.AddWithValue("@paisResidencia", cliente.PaisResidencia);
                cmd.Parameters.AddWithValue("@documentoIdenficacion", cliente.DocumentoIdentificacion);
                cmd.Parameters.AddWithValue("@fechaNacimiento", cliente.FechaNacimiento);
                
                rowsAffected = await cmd.ExecuteNonQueryAsync();
            }

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateCustomer(Cliente cliente)
        {
            var queryExpression = "UPDATE clientes " +
                                   "SET Nombre = @nombre," +
                                   "Email = @email," +
                                   "Apellido = @apellido," +
                                   "Telefono = @telefono," +
                                   "PaisResidencia = @paisResidencia, " +
                                   "DocumentoIdentificacion = @documentoIdenficacion, " +
                                   "FechaNacimiento = @fechaNacimiento " +
                                   $"WHERE IdCliente = {cliente.IdCliente}";

            var db = dbConnection();

            int resultado = 0;

            using (MySqlCommand cmd = new MySqlCommand(queryExpression, db))
            {
                await db.OpenAsync();

                cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@email", cliente.Email);
                cmd.Parameters.AddWithValue("@telefono", cliente.Telefono);
                cmd.Parameters.AddWithValue("@paisResidencia", cliente.PaisResidencia);
                cmd.Parameters.AddWithValue("@documentoIdenficacion", cliente.DocumentoIdentificacion);
                cmd.Parameters.AddWithValue("@fechaNacimiento", cliente.FechaNacimiento);

                resultado = await cmd.ExecuteNonQueryAsync();
            }

            return resultado > 0;
        }

        public async Task<Cliente> ViewDetailsCustomer(int IdCliente)
        {
            var db = dbConnection();
            await db.OpenAsync();

            var queryExpression = $"SELECT * FROM clientes WHERE idCliente = {IdCliente}";

            Cliente cliente = db.QueryAsync<Cliente>(queryExpression, new { }).Result.First();
            
            queryExpression = $"SELECT idInteres FROM clientes_interes WHERE idCliente = {IdCliente}";
            List<int> result = db.QueryAsync<int>(queryExpression, new { }).Result.ToList();

            if (result != null && result.Count > 0) {
                cliente.ClienteInteres = new ClienteInteres();
                queryExpression = @"SELECT * FROM intereses";

                cliente.ClienteInteres.Intereses = db.QueryAsync<Interes>(queryExpression, new { }).Result.ToList();
                cliente.ClienteInteres.Cliente = cliente;
                cliente.ClienteInteres.SelectedOptions = result;
            }

            return cliente;
        }

        public async Task<bool> DeleteCustomer(int IdCliente)
        {
            var db = dbConnection();
            await db.OpenAsync();

            var queryExpression = $"DELETE FROM clientes WHERE idCliente = {IdCliente}";

            return await db.ExecuteAsync(queryExpression, new { Id = IdCliente }) > 0;
        }

        public async Task<ClienteInteres> ViewInterests(int IdCliente)
        {
            ClienteInteres clienteInteres = new ClienteInteres();

            var db = dbConnection();
            await db.OpenAsync();

            var queryExpression = @"SELECT * FROM intereses";

            clienteInteres.Intereses = db.QueryAsync<Interes>(queryExpression, new { }).Result.ToList();

            queryExpression = $"SELECT * FROM clientes WHERE idCliente = {IdCliente}";
            clienteInteres.Cliente = db.QueryFirstOrDefault<Cliente>(queryExpression, new { Id = IdCliente });
            
            queryExpression = $"SELECT idInteres FROM clientes_interes WHERE idCliente = {IdCliente}";
            
            clienteInteres.SelectedOptions = db.QueryAsync<int>(queryExpression, new { Id = IdCliente }).Result.ToList();

            return clienteInteres;
        }

        public async void AddNewConfigurationCustomer(ClienteInteres clienteInteres) {

            var queryExpression = "";
            var db = dbConnection();
            await db.OpenAsync();

            int idCliente = clienteInteres.Cliente.IdCliente;

            queryExpression = $"DELETE FROM clientes_interes WHERE idCliente = {idCliente}";

            await db.ExecuteAsync(queryExpression, new { Id = idCliente });

            if (clienteInteres.SelectedOptions != null && clienteInteres.SelectedOptions.Count > 0) {
                foreach (var configuration in clienteInteres.SelectedOptions)
                {
                    queryExpression = $"INSERT INTO clientes_interes (idCliente, idInteres) VALUES ({idCliente}, {configuration})";
                    await db.ExecuteAsync(queryExpression, new { Id = idCliente});
                }
            }

            queryExpression = "UPDATE clientes " +
                                $"SET CuentaTwitter = @cuentaTwitter," +
                                $"NombreEmpresa = @nombreEmpresa," +
                                $"Genero = @genero," +
                                $"UrlGenerado = @urlGenerado " +
                                $"WHERE IdCliente = {idCliente}";

            using (MySqlCommand cmd = new MySqlCommand(queryExpression, db))
            {
                cmd.Parameters.AddWithValue("@cuentaTwitter", clienteInteres.Cliente.CuentaTwitter);
                cmd.Parameters.AddWithValue("@nombreEmpresa", clienteInteres.Cliente.NombreEmpresa);
                cmd.Parameters.AddWithValue("@genero", clienteInteres.Cliente.Genero);
                cmd.Parameters.AddWithValue("@urlGenerado", clienteInteres.Cliente.UrlGenerado);
                
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
