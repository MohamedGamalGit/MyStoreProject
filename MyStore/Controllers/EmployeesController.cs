using AutoMapper;
using Commen.ViewModels;
using Commen.ViewModels.Employees;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Repositories.IGenericRepository;

namespace MyStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeesController(IGenericRepository<Employee> employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }


        [HttpPost("addEmployee")]
        public async Task<IActionResult> AddEmployee(EmployeeAddVM employeeAddVM)
        {
            try
            {
                var model = _mapper.Map<Employee>(employeeAddVM);
                await _employeeRepository.AddAsync(model);
                await _employeeRepository.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("getAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            return Ok(await _employeeRepository.GetByIdAsync(id));
        }

        [HttpPost("update")]
        public async Task<ResponseVM<EmployeeAddVM>> UpdateEmployee(EmployeeAddVM employee)
        {
            try
            {

                _employeeRepository.Update(_mapper.Map<Employee>(employee));
                await _employeeRepository.SaveChangesAsync();


                return new ResponseVM<EmployeeAddVM>()
                {
                    Code = 200,
                    Message = "Employee updated successfully",
                    Data = new List<EmployeeAddVM>() { employee }
                };
            }
            catch (Exception ex)
            {

                return new ResponseVM<EmployeeAddVM>()
                {
                    Code = 400,
                    Message = "Employee updated Error",
                    Data = new List<EmployeeAddVM>() { employee }
                };
            }
            
        }
    }
}
