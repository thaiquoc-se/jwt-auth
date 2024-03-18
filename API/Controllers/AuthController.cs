using AutoMapper;
using DataTransferObject.DTO;
using DataTransferObject.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repository;
using Repositories.Service;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IJWTTokenService _jwtTokenService;
        public AuthController(ICustomerRepository customerRepository, IMapper mapper, IConfiguration configuration, IJWTTokenService jwtTokenService)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (ModelState.IsValid)
            {
                if (login.email!.Equals(_configuration["admin:email"]) &&
                login.password!.Equals(_configuration["admin:password"]))
                {
                    var adminToken = _jwtTokenService.CreateJWTToken(_configuration["admin:email"], "Admin");
                    return Ok(new
                    {
                        message = "Successfully",
                        role ="Admin",
                        token = adminToken
                    });
                }
                var customer = await _customerRepository.Login(login.email!, login.password!);
                if (customer == null | customer!.CustomerStatus != 0)
                {
                    return NotFound();
                }
                var response = _mapper.Map<CustomerDTO>(customer);
                var userToken = _jwtTokenService.CreateJWTToken(customer.EmailAddress, "Customer");
                return Ok(new
                {
                    message = "Successfully",
                    data = response,
                    token = userToken
                });
            }

            return BadRequest();
        }
    }
}

