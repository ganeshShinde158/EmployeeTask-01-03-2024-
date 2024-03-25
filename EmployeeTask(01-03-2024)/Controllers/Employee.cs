using EmployeeTask_01_03_2024_.Models;
using EmployeeTask_01_03_2024_.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTask_01_03_2024_.Controllers
{
    public class Employee : Controller
    {
        private readonly EmployeeRepo _employeeRepository;

        public Employee(EmployeeRepo employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var employees = await _employeeRepository.GetAllEmployeesAsync();

                if (employees == null)
                {
                    TempData["SuccessMessage"] = "Data Get Successfully";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Data not get";
                }
                return View(employees);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View("error");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddEmployee()
        {

            try
            {
                var cities = await _employeeRepository.GetCities();

                var department = await _employeeRepository.GetDepartment();

                ViewBag.Cities = cities;
                ViewBag.Department = department;
                
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddEmployee(EmployeeModel employee)
        {
            ReturnResponse returnResponse = new ReturnResponse();

            try
            {
                _employeeRepository.InsertEmployee(employee, returnResponse);

                if (returnResponse.ResponseCode == 0)
                {
                    TempData["SuccessMessage"] = returnResponse.ResponseMessage;
                    return RedirectToAction("Index", "Employee");
                }
                else
                {
                    TempData["ErrorMessage"] = returnResponse.ResponseMessage;
                    return View(employee);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View("error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            return View();
        }

        [HttpPost]
        public IActionResult EditEmployee(EmployeeModel employee)
        {
            try
            {
                int responseCode;
                string responseMsg;

                ReturnResponse returnResponse = new ReturnResponse();

                _employeeRepository.UpdateEmployee(employee, out responseCode, out responseMsg);

                returnResponse.ResponseCode = responseCode;
                returnResponse.ResponseMessage = responseMsg;

                if (returnResponse.ResponseCode == 0)
                {
                    TempData["SuccessMessage"] = "Employee Update Successfully";
                }

                else
                {
                    TempData["ErrorMessage"] = "Error for update employee";
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteEmployeeById(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int employeeID)
        {
            try
            {
                ReturnResponse returnResponse = new ReturnResponse();
                returnResponse = await _employeeRepository.DeleteEmployee(employeeID);

                if (returnResponse.ResponseCode == 00)
                {
                    TempData["SuccessMessage"] = returnResponse.ResponseMessage;
                }
                else
                {
                    TempData["ErrorMessage"] = returnResponse.ResponseMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            EmployeeLogin employeeLogin = new EmployeeLogin();
            var employee = _employeeRepository.ValidateLogin(username, password);

            if (employee.EmployeeId != 0)
            {
                TempData["EmployeeId"] = employee.EmployeeId;
                TempData["AuthToken"] = employee.AuthToken;

                return RedirectToAction("Index", "Employee");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        [HttpGet]
        public IActionResult Logout()
        {
            return View("Login");
        }
    }
}
