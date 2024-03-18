using BusinessObjects.Models;
using DataTransferObject.DTO;
using DataTransferObject.ResponseVM;
using Microsoft.EntityFrameworkCore;
using Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> GetAll();
        Task<Customer?> Login(string email, string password);
        Task<Customer> GetByID(int id);
        Task<ResponseVM> Add(Customer customer);
        Task<ResponseVM> Update(CustomerDTO customer, int id);
        Task<ResponseVM> Delete(int id);
    }
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseVM> Add(Customer customer)
        {
            var check = _unitOfWork.Customer.Get(cus => cus.EmailAddress == customer.EmailAddress).FirstOrDefault();
            if (check != null)
            {
                return new ResponseVM
                {
                    IsSuccess = false,
                    message = "This Email Already Taken"
                };
            }
            await _unitOfWork.Customer.Add(customer);
            var isAdd = await _unitOfWork.SaveChangesAsync();

            if (isAdd)
            {
                return new ResponseVM
                {
                    IsSuccess = true,
                    message = "Successfully"
                };
            }

            return new ResponseVM
            {
                IsSuccess = false,
                message = "Fail"
            };
        }

        public async Task<ResponseVM> Delete(int id)
        {
            var existedCustomer = await _unitOfWork.Customer.Get(cus => cus.CustomerId == id && cus.CustomerStatus == 0).FirstOrDefaultAsync();
            if (existedCustomer == null)
            {
                return new ResponseVM
                {
                    IsSuccess = true,
                    message = "Customer notfound"
                };
            }
            existedCustomer.CustomerStatus = 1;
            _unitOfWork.Customer.Update(existedCustomer);
            var isDelete = await _unitOfWork.SaveChangesAsync();
            if (isDelete)
            {
                return new ResponseVM
                {
                    IsSuccess = true,
                    message = "Successfully"
                };
            }
            return new ResponseVM
            {
                IsSuccess = false,
                message = "Fail"
            };
        }

        public IQueryable<Customer> GetAll()
        {
            return _unitOfWork.Customer.Get(c => c.CustomerStatus == 0);
        }

        public async Task<Customer> GetByID(int id)
        {
            var customer = await _unitOfWork.Customer.Get(c => c.CustomerId == id && c.CustomerStatus == 0).SingleOrDefaultAsync();
            if (customer != null)
            {
                return customer;
            }
            throw new Exception();
        }

        public async Task<Customer?> Login(string email, string password)
        {
            var check = await _unitOfWork.Customer.Get(cus => cus.EmailAddress == email && cus.Password == password).SingleOrDefaultAsync();
            return check!;
        }

        public async Task<ResponseVM> Update(CustomerDTO customer, int id)
        {
            var existedCustomer = await _unitOfWork.Customer.Get(cus => cus.CustomerId == id && cus.CustomerStatus == 0).SingleOrDefaultAsync();
            if (existedCustomer != null)
            {

                existedCustomer.CustomerFullName = customer.CustomerFullName;
                existedCustomer.EmailAddress = customer.EmailAddress;
                existedCustomer.Telephone = customer.Telephone;
                existedCustomer.CustomerBirthday = customer.CustomerBirthday;
                existedCustomer.CustomerStatus = customer.CustomerStatus;
                existedCustomer.Password = customer.Password;
                _unitOfWork.Customer.Update(existedCustomer);
                var isUpdate = await _unitOfWork.SaveChangesAsync();
                if (isUpdate)
                {
                    return new ResponseVM
                    {
                        IsSuccess = true,
                        message = "Successfully"
                    };
                }
                return new ResponseVM
                {
                    IsSuccess = false,
                    message = "Fail"
                };
            }
            return new ResponseVM
            {
                IsSuccess = true,
                message = "Customer not found"
            };
        }
    }
}
