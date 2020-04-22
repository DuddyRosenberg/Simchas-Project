using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SimchasContributorsProject.Data
{
    public class Database
    {
        private string _connectionString;
        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Simcha> GetSimchas()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Simchas s LEFT JOIN Contributions c ON s.ID=c.SimchaID";
                connection.Open();
                var reader = command.ExecuteReader();
                var simchas = new Dictionary<int, Simcha>();
                while (reader.Read())
                {
                    var id = (int)reader["ID"];
                    var simcha = simchas.GetValueOrDefault(id);
                    if (simcha == null)
                    {
                        simcha = new Simcha
                        {
                            ID = id,
                            Name = (string)reader["Name"],
                            Date = (DateTime)reader["Date"],
                        };
                        simchas.Add(id, simcha);
                    }
                    var contributedAmount = reader["Amount"];
                    if (contributedAmount != DBNull.Value)
                    {
                        simcha.Contributions.Add((decimal)contributedAmount);
                    }
                }
                return simchas.Select(k => k.Value).ToList();
            }
        }
        public int GetTotalContributors()
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(ID) FROM Contributors";
                connection.Open();
                return (int)command.ExecuteScalar();
            }
        }
        public int AddSimcha(Simcha simcha)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO Simchas(Name,Date) VALUES(@name,@date) SELECT SCOPE_IDENTITY()";
                command.Parameters.AddWithValue("@name", simcha.Name);
                command.Parameters.AddWithValue("@date", simcha.Date);
                connection.Open();
                return (int)(decimal)command.ExecuteScalar();
            }
        }
        public List<Contributor> GetContributors()
        {
            var contributors = new Dictionary<int, Contributor>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Contributors";
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var id = (int)reader["ID"];
                    contributors.Add(id, new Contributor
                    {
                        ID = id,
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        CellNumber = (string)reader["CellNumber"],
                        AlwaysInclude = (bool)reader["AlwaysInclude"]
                    });
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Deposits";
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    contributors[(int)reader["ContributorID"]].Balance += (decimal)reader["Amount"];
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Contributions";
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    contributors[(int)reader["ContributorID"]].Balance -= (decimal)reader["Amount"];
                }
            }
            return contributors.Select(k => k.Value).ToList();
        }
        public int AddContributor(Contributor contributor)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Contributors(FirstName,LastName,CellNumber,AlwaysInclude)
                                      VALUES(@firstName,@lastName,@cellNumber,@alwaysInclude)
                                      SELECT SCOPE_IDENTITY()";
                command.Parameters.AddWithValue("@firstName", contributor.FirstName);
                command.Parameters.AddWithValue("@lastName", contributor.LastName);
                command.Parameters.AddWithValue("@cellNumber", contributor.CellNumber);
                command.Parameters.AddWithValue("@AlwaysInclude", contributor.AlwaysInclude);
                connection.Open();
                return (int)(decimal)command.ExecuteScalar();
            }
        }
        public void AddDeposit(int contributorID, decimal amount, DateTime date)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"INSERT INTO Deposits(Amount,ContributorID,Date)
                                      VALUES(@amount,@contributorID,@date)";
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@ContributorID", contributorID);
                command.Parameters.AddWithValue("@date", date);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public List<Transection> GetTransections(int id)
        {
            var transections = new List<Transection>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Deposits Where ContributorID=@id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    transections.Add(new Transection
                    {
                        Description = "Deposit",
                        Amount = (decimal)reader["Amount"],
                        Date = reader.GEtOrNull<DateTime>("Date")
                    });
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM Contributions c
                                        LEFT JOIN Simchas s ON c.simchaID=s.id
                                        Where ContributorID=@id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    transections.Add(new Transection
                    {
                        Description = $"Contribution for the {(string)reader["Name"]}",
                        Amount = -(decimal)reader["Amount"],
                        Date = reader.GEtOrNull<DateTime>("Date")
                    });
                }
            }
            return transections;
        }
        public Contributor GetContributor(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Contributors WHERE ID=@id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                return new Contributor
                {
                    ID = id,
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                };
            }
        }
        public Simcha GetSimchaWithContributors(int id)
        {
            var simcha = new Simcha();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Simchas WHERE ID=@id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                reader.Read();
                simcha.ID = id;
                simcha.Name = (string)reader["Name"];
                simcha.Date = (DateTime)reader["Date"];
                simcha.Contributors = GetContributors();
            }
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Contributions WHERE simchaID=@id";
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    simcha.Contributors.FirstOrDefault(c => c.ID == (int)reader["ContributorID"])
                        .Contribution = (decimal)reader["Amount"];
                }
            }
            return simcha;
        }
        public void DeleteAndAddContributions(int simchaID, List<Contribution> contributions)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE Contributions WHERE SimchaID=@simchaID";
                command.Parameters.AddWithValue("@simchaID", simchaID);
                connection.Open();
                command.ExecuteNonQuery();
            }
            foreach (var contribution in contributions)
            {
                if (contribution.Contribute)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO Contributions(Amount,SimchaID,ContributorID,Date)" +
                                              "VALUES(@amount,@simchaID,@contributorID,@date)";
                        command.Parameters.AddWithValue("@amount", contribution.Amount);
                        command.Parameters.AddWithValue("@simchaID", simchaID);
                        command.Parameters.AddWithValue("@contributorID", contribution.ContributorID);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void AddContribution(Contribution contribution)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Contributions(Amount,SimchaID,ContributorID,Date)" +
                                      "VALUES(@amount,@simchaID,@contributorID,@date)";
                command.Parameters.AddWithValue("@amount", contribution.Amount);
                command.Parameters.AddWithValue("@simchaID", contribution.SimchaID);
                command.Parameters.AddWithValue("@contributorID", contribution.ContributorID);
                command.Parameters.AddWithValue("@date", DateTime.Now);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}