using AutoMapper;
using BusinessObjects.Models;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Repository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public CustomerController(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCustomer()
        {
            var customer = await _customerRepository.GetAll().ToListAsync();
            if (customer != null)
            {
                return Ok(_mapper.Map<List<CustomerDTO>>(customer));
            }
            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerByID(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByID(id);
                return Ok(_mapper.Map<CustomerDTO>(customer));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewCustomer(CustomerDTO newCustomer)
        {
            var customer = _mapper.Map<Customer>(newCustomer);
            var response = await _customerRepository.Add(customer);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response.message);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCustomer(CustomerDTO customer, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _customerRepository.Update(customer, id);
                    if (response.IsSuccess)
                    {
                        return Ok(response.message);
                    }
                    return BadRequest(response.message);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var isDelete = await _customerRepository.Delete(id);
                if (isDelete.IsSuccess)
                {
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
