using EmployeeTask_01_03_2024_.Controllers;
using EmployeeTask_01_03_2024_.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EmployeeTask_01_03_2024_.Repository
{
    public class EmployeeRepo
    {
        private readonly IConfiguration _configuration;

        public EmployeeRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<EmployeeModel>> GetAllEmployeesAsync()
        {
            List<EmployeeModel> employees = new List<EmployeeModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConn")))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("GetAllEmployees", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                EmployeeModel employee = new EmployeeModel
                                {
                                    EmployeeID = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    City = reader.GetString(3),
                                    Age = reader.GetInt32(4),
                                    DOJ = reader.GetDateTime(5),
                                    Department = reader.GetString(6)
                                };
                                employees.Add(employee);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return employees;
        }

        public void InsertEmployee(EmployeeModel employee, ReturnResponse returnResponse)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConn")))
            {
                connection.Open();

                using (var command = new SqlCommand("InsertEmployeeData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    DateTime da = DateTime.Now;

                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@CityId", employee.City);
                    command.Parameters.AddWithValue("@Age", employee.Age);
                    command.Parameters.AddWithValue("@DOJ", da);
                    command.Parameters.AddWithValue("@DepartmentId", employee.Department);

                    var responseCodeParam = new SqlParameter("@ResponseCode", SqlDbType.Int);
                    responseCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(responseCodeParam);

                    var responseMsgParam = new SqlParameter("@ResponseMessage", SqlDbType.VarChar, 255);
                    responseMsgParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(responseMsgParam);

                    command.ExecuteNonQuery();

                    returnResponse.ResponseCode = (int)responseCodeParam.Value;
                    returnResponse.ResponseMessage = responseMsgParam.Value.ToString();
                }
            }
        }

        public async Task<List<City>> GetCities()
        {
            List<City> cities = new List<City>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConn")))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand("[dbo].[GetCities]", connection);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        City city = new City
                        {
                            CityId = reader.GetInt32(0),
                            CityName = reader.GetString(1)
                        };
                        cities.Add(city);
                    }
                }
            }

            return cities;
        }

        public async Task<List<DepartmentModel>> GetDepartment()
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConn")))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand("[dbo].[GetDepartment]", connection);
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        DepartmentModel department = new DepartmentModel();
               
                        {
                            department.DepartmentID = reader.GetInt32(0);
                            department.DepartmentName = reader.GetString(1);
                        };
                        departments.Add(department);
                    }
                }
            }

            return departments;
        }

        public async Task<EmployeeModel> GetEmployeeByIdAsync(int employeeId)
        {
            EmployeeModel employee = null;

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConn")))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("GetEmployeeById", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            DateTime da = DateTime.Now;
                            employee = new EmployeeModel
                            {
                                EmployeeID = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                City = reader.GetString(3),
                                Age = reader.GetInt32(4),
                                DOJ = reader.GetDateTime(5),
                                Department = reader.GetString(6)
                            };
                        }
                    }
                }
            }

            return employee;
        }

        public void UpdateEmployee(EmployeeModel employee, out int responseCode, out string responseMsg)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConn")))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("[dbo].[UpdateEmployee]", connection);
                command.CommandType = CommandType.StoredProcedure;

                DateTime da = DateTime.Now;

                command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName);
                command.Parameters.AddWithValue("@City", employee.City);
                command.Parameters.AddWithValue("@Age", employee.Age);
                command.Parameters.AddWithValue("@DOJ", da);
                command.Parameters.AddWithValue("@Department", employee.Department);

                command.Parameters.Add("@ResponseCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@ResponseMsg", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();

                responseCode = Convert.ToInt32(command.Parameters["@ResponseCode"].Value);
                responseMsg = command.Parameters["@ResponseMsg"].Value.ToString();
            }
        }

        public async Task<ReturnResponse> DeleteEmployee(int employeeID)
        {
            ReturnResponse returnResponse = new ReturnResponse();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConn")))
            {
                using (SqlCommand command = new SqlCommand("DeleteEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@EmployeeID", employeeID);

                    SqlParameter responseCodeParam = new SqlParameter("@ResponseCode", SqlDbType.Int);
                    responseCodeParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(responseCodeParam);

                    SqlParameter responseMsgParam = new SqlParameter("@ResponseMsg", SqlDbType.VarChar, 255);
                    responseMsgParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(responseMsgParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    returnResponse.ResponseCode = Convert.ToInt32(command.Parameters["@ResponseCode"].Value);
                    returnResponse.ResponseMessage = command.Parameters["@ResponseMsg"].Value.ToString();
                }
            }

            return returnResponse;
        }

        public EmployeeLogin ValidateLogin(string username, string password)
        {
            var employee = new EmployeeLogin();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DbConn")))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("ValidateLogin", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = username;
                command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = password;
                command.Parameters.Add("@EmployeeId", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@AuthToken", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
                command.Parameters.Add("@ResponseCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                command.Parameters.Add("@ResponseMsg", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;

                command.ExecuteNonQuery();

                employee.EmployeeId = (int)command.Parameters["@EmployeeId"].Value;
                employee.AuthToken = command.Parameters["@AuthToken"].Value.ToString(); 
                                                                                        
            }

            return employee;
        }
    }
}
