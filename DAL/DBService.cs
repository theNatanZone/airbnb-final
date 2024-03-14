using AirbnbProj2.Models;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace AirbnbProj2.DAL
{
    public class DBService
    {
        private readonly string connectionString;
        private readonly SqlConnection _db;

        public DBService(IConfiguration configuration)
        {
            // Program.cs contains the configuration for appsetting.json and Dependency-Injection for DBServices
            connectionString = configuration.GetConnectionString("airbnbDB");
            _db = new SqlConnection(connectionString);
        }

        public SqlConnection Connect()
        {
            if (_db.State == ConnectionState.Closed)
                _db.Open();
            return _db;
        }

        public void CloseDbConnection()
        {
            if (_db.State == ConnectionState.Open)
                _db.Close();
        }

        //---------------------------------------------------------------------------------
        // SELECT all users
        //---------------------------------------------------------------------------------
        public List<User> ReadUsers() 
        {
            SqlConnection con;
            SqlCommand cmd;
            SqlDataReader dataReader;

            try
            {
                con = Connect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to connect SQL server: {ex}");
                throw;
            }

            var users = new List<User>();
            cmd = CreateStoredProcedureCommand("spReadUsers2024", con);
            
            try
            {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to get dataReader: {ex}");
                throw;
            }

            try
            {
                while (dataReader.Read())
                {
                    var u = new User
                    {
                        FirstName = dataReader["firstName"].ToString(),
                        FamilyName = dataReader["familyName"].ToString(),
                        Email = dataReader["email"].ToString(),
                        Password = dataReader["password"].ToString(),
                        IsActive = (bool)dataReader["isActive"],
                        IsAdmin = (bool)dataReader["isAdmin"]
                    };
                    users.Add(u);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to retrieve users from dataReader: {ex}");
                throw;
            }
            

            CloseDbConnection();
            return users;
        }

        //---------------------------------------------------------------------------------
        // SELECT all flats
        //---------------------------------------------------------------------------------
        public List<Flat> ReadFlats()
        {
            SqlConnection con;
            SqlCommand cmd;
            SqlDataReader dataReader;

            try
            {
                con = Connect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to connect SQL server: {ex}");
                throw;
            }

            var flats = new List<Flat>();

            cmd = CreateStoredProcedureCommand("spReadFlats2024", con);

            try
            {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to get dataReader: {ex}");
                throw;
            }
            

            while (dataReader.Read())
            {
                var id = Convert.ToInt32(dataReader["id"]);
                var city = dataReader["city"].ToString();
                var address = dataReader["address"].ToString();
                var numberOfRooms = Convert.ToInt32(dataReader["numberOfRooms"]);
                var price = Convert.ToDouble(dataReader["price"]);

                var f = Flat.CreateFlatFromDb(id, city, address, numberOfRooms, price);
                flats.Add(f);
            }

            CloseDbConnection();
            return flats;
        }

        //---------------------------------------------------------------------------------
        // SELECT all vacations
        //---------------------------------------------------------------------------------
        public List<Vacation> ReadVacations()
        {
            SqlConnection con;
            SqlCommand cmd;
            SqlDataReader dataReader;

            try
            {
                con = Connect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to connect SQL server: {ex}");
                throw;
            }

            var vacations = new List<Vacation>();
            cmd = CreateStoredProcedureCommand("spReadVacations2024", con);

            try
            {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to get dataReader: {ex}");
                throw;
            }

            while (dataReader.Read())
            {
                var v = new Vacation
                {
                    Id = Convert.ToInt32(dataReader["id"]),
                    UserId = dataReader["userId"].ToString(),
                    FlatId = Convert.ToInt32(dataReader["flatId"]),
                    StartDate = Convert.ToDateTime(dataReader["startDate"]),
                    EndDate = Convert.ToDateTime(dataReader["endDate"])
                };
                vacations.Add(v);
            }

            CloseDbConnection();
            return vacations;
        }

        //---------------------------------------------------------------------------------
        // Creates a StoredProcedure SqlCommand 
        //---------------------------------------------------------------------------------
        public SqlCommand CreateStoredProcedureCommand(string spName, SqlConnection con)
        {
            var cmd = new SqlCommand
            {
                Connection = con,
                CommandText = spName,
                CommandTimeout = 10,
                CommandType = CommandType.StoredProcedure
            };

            return cmd;
        }

        //---------------------------------------------------------------------------------
        // INSERT new data
        //---------------------------------------------------------------------------------
        public int Insert(string spName, DbParameter[]? cmdParams)
        {
            SqlConnection con;
            SqlCommand cmd;
            int result;

            try
            {
                con = Connect();
                cmd = CreateStoredProcedureCommand(spName, con);

                if (cmdParams != null)
                {
                    cmd.Parameters.AddRange(cmdParams);
                }

                result = cmd.ExecuteNonQuery();
                return result;
            }

            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            finally
            {
                CloseDbConnection();
            }
        }

        //---------------------------------------------------------------------------------
        // UPDATE existing data
        //---------------------------------------------------------------------------------

        public int Update(string spName, DbParameter[]? cmdParams) // *****  TODO: PUT controller to update fields EXCEPT for the Email Primary key!!
        {
            SqlConnection con;
            SqlCommand cmd;
            int result;

            try
            {
                con = Connect();
                cmd = CreateStoredProcedureCommand(spName, con);

                if (cmdParams != null)
                {
                    cmd.Parameters.AddRange(cmdParams);
                }

                result = cmd.ExecuteNonQuery();
                return result;
            }

            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            finally
            {
                CloseDbConnection();
            }
        }

        //--------------------------------------------------------------------------------------------------
        // SELECT vacations and return average monthly report 
        //--------------------------------------------------------------------------------------------------

        public List<object> GetAverageReport(int month)
        {

            SqlConnection con;
            SqlCommand cmd;
            SqlDataReader dataReader;
            List<object> resultsList = new List<object>();

            try
            {
                con = Connect(); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to connect SQL server: {ex}");
                throw;
            }

            cmd = CreateStoredProcedureCommand("spGetVacationsReport2024", con);  // create the command
            cmd.Parameters.AddWithValue("@month", month);

            try
            {
                dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var result = new
                    {
                        City = dataReader["City"].ToString(),
                        AveragePricePerNight = Convert.ToDouble(dataReader["AveragePricePerNight"])
                    };
                    resultsList.Add(result);
                }

                return resultsList;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"failed to execute SQL dataReader command: {ex}");
                throw;
            }

            finally
            {
                CloseDbConnection();
            }
        }

    }
}