using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreCrud.Api.Models;
using NetCoreCrud.Api.Repository;
using NetCoreCrud.Api.Services;
using NetCoreCrud.Api.Swagger;
using NetCoreCrud.Api.Swagger.Examples;
using Serilog;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NetCoreCrud.Api.Controllers
{
    [Route("customer")]
    public class CustomerController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICustomerService _service;
        private readonly CustomerDbContext _context;

        public CustomerController(ILogger logger, ICustomerService service,
            CustomerDbContext context)
        {
            _logger = logger;
            _context = context;
            _service = service;
        }

        [HttpGet, Route("seedSampleData")]
        public IActionResult SeedSampleData()
        {
            _service.SeedSampleData(_context);
            return Ok();
        }

        [HttpGet, Route("getAllCustomers")]
        [SwaggerOperation(OperationId = "Get all customers")]
        [SwaggerHeader("Correlation-Id", "string", Description = "GUID or equivalent that can be used to uniquely identify any request.")]
        // 200
        [ProducesResponseType(typeof(List<Customer>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GetAllCustomersExample))]
        // 404
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.NotFound)]
        [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(CommonResponseExample))]
        // 500
        [ProducesResponseType(typeof(InternalServerError), (int)HttpStatusCode.InternalServerError)]
        [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerErrorExample))]
        public async Task<IActionResult> GetAll()
        {
            var allCustomers = await _service.GetAllCustomers(_context);
            if (!allCustomers.Any())
                return NotFound(new CommonResponse((int) HttpStatusCode.NotFound, "No customers in data storee."));

            return Ok(allCustomers);
        }

        [HttpGet, Route("getCustomersByName")]
        // 200
        [ProducesResponseType(typeof(List<Customer>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GetAllCustomersExample))]
        // 404
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.NotFound)]
        [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(CommonResponseExample))]
        // 400
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.BadRequest)]
        [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(CommonResponseExample))]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            _logger.Debug("Received request with keyword {@NameKeyword}", name);

            if (string.IsNullOrEmpty(name))
            {
                _logger.Information("No input received");
                return BadRequest(new CommonResponse((int)HttpStatusCode.BadRequest, "Input missing"));
            }

            var matchingCustomers = await _service.GetCustomerByPartialName(_context, name);

            if(!matchingCustomers.Any())
            {
                _logger.Information("No customers found with the search key {@NameKeyword}", name);
                return NotFound(new CommonResponse((int) HttpStatusCode.NotFound, "Customer not found"));
            }

            _logger.Information("Data found for customers with name similar to {@NameKeyword}", name);
            return Ok(matchingCustomers);
        }

        [HttpPost, Route("addCustomer")]
        // 200
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CommonResponseExample))]
        // 400
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.BadRequest)]
        [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(CommonResponseExample))]
        public async Task<IActionResult> AddCustomer([FromBody]Customer customer)
        {
            _logger.Debug("Received request to add customer {@Name}", customer?.FirstName);
            if (customer == null)
            {
                _logger.Information("Customer data is null");
                return BadRequest(new CommonResponse((int) HttpStatusCode.BadRequest, "Input missing"));
            }
            var hasAdded = await _service.AddCustomer(_context, customer);

            return Ok(hasAdded
                ? new CommonResponse((int) HttpStatusCode.OK, $"New customer added : {customer.FirstName}")
                : new CommonResponse((int) HttpStatusCode.OK,
                    $"Record already exists. Cannot add customer with same name : {customer.FirstName}"));
        }

        [HttpPut, Route("editCustomer")]
        // 200
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CommonResponseExample))]
        public async Task<IActionResult> EditCustomer([FromBody]Customer customer)
        {
            _logger.Debug("Received request to edit customer {@Name}", customer?.FirstName);
            if (customer == null)
            {
                _logger.Information("Customer data is null");
                return BadRequest(new CommonResponse((int)HttpStatusCode.BadRequest, "Input missing"));
            }
            var response = await _service.EditCustomerByFirstName(_context, customer);
            if (response == null)
            {
                _logger.Information("Customer not found in database");
                return Ok(new CommonResponse((int)HttpStatusCode.NotFound, $"No existing record available for customer : {customer.FirstName}"));
            }

            _logger.Information("Customer details succesfully modified for : {@Name}", customer.FirstName);
            return Ok(new CommonResponse((int)HttpStatusCode.OK, $"Updated data for customer : {customer.FirstName}"));
        }

        [HttpDelete, Route("deleteCustomer")]
        // 200
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CommonResponseExample))]
        // 404
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.NotFound)]
        [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(CommonResponseExample))]
        public async Task<IActionResult> Delete([FromQuery] string name)
        {
            _logger.Debug("Received request to edit customer {@Name}", name);
            if (string.IsNullOrEmpty(name))
            {
                _logger.Information("No input received");
                return BadRequest(new CommonResponse((int)HttpStatusCode.BadRequest, "Input missing"));
            }
            var response = await _service.DeleteCustomerByFirstName(_context, name);
            if (response == null)
            {
                _logger.Information("Customer not found in database");
                return NotFound(new CommonResponse((int) HttpStatusCode.NotFound, "Customer not found"));
            }

            _logger.Information("Customer details succesfully deleted for : {@Name}", name);
            return Ok(new CommonResponse((int) HttpStatusCode.OK, $"Deleted data for customer : {name}"));
        }
    }
}
