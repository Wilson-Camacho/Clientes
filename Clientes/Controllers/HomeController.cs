using Clientes.Interface;
using Clientes.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics;
using System.Drawing.Printing;

namespace Clientes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICliente _cliente;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, ICliente cliente)
        {
            _logger = logger;
            _cliente = cliente;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            Task<IEnumerable<Cliente>> usuarios = _cliente.GetAllCustomers();
            int PageSize = 5;

            var startIndex = (page - 1) * PageSize;
            var items = usuarios.Result.ToList().Skip(startIndex).Take(PageSize).ToList();

            var totalPages = (int) Math.Ceiling((double) usuarios.Result.ToList().Count / PageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.Items = items;

            return View();
        }
        
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCustomer(Cliente cliente)
        {
            if (!ModelState.IsValid) return View();

            var resultado = _cliente.InsertCustomer(cliente);
            
            if (resultado.Result) {
                Index();
                return RedirectToAction("Index");
            }

            return View();
        }


        public IActionResult UpdateCustomer(int idCliente)
        {
            Task <Cliente> cliente = _cliente.ViewDetailsCustomer(idCliente);
            return View(cliente.Result);
        }

        [HttpPost]
        public IActionResult UpdateCustomer(Cliente cliente)
        {
            if (!ModelState.IsValid) return View();

            var resultado = _cliente.UpdateCustomer(cliente);

            if (resultado.Result)
            {
                Index();
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult DeleteCustomer(int idCliente)
        {
            _cliente.DeleteCustomer(idCliente);
            return RedirectToAction("Index");
        }

        public IActionResult NewSettingsCustomer(int idCliente) {
            Task<ClienteInteres> interes = _cliente.ViewInterests(idCliente);
            return View(interes.Result);
        }

        [HttpPost]
        public IActionResult NewSettingsCustomer(ClienteInteres clienteInteres)
        {
            clienteInteres.Cliente.UrlGenerado = HttpContext.Request.GetDisplayUrl() + "?idCliente=" + clienteInteres.Cliente.IdCliente;
            _cliente.AddNewConfigurationCustomer(clienteInteres);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
