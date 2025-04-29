using FirmaService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FirmaService.Controllers
{

    public class FinalizatController : Controller
    {
        private string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;Encrypt=False;";

        public async Task<IActionResult> Index()
        {

            List<Models.Finalizat> rezultat = new List<Models.Finalizat>();
            await Terminate(rezultat);


            return View(rezultat);
        }
        //Functie pentru afisarea dispozitivelor reparate
        private async Task Terminate(List<Models.Finalizat> rezultat)
        {
            string sql = @"
            SELECT 
                c.Nume AS ClientNume,
                c.Prenume AS ClientPrenume,
                d.TipDispozitiv,
                d.Status,
                a.Cost AS CostFinal,
                a.ServiciiId
            FROM Dispozitive d
            INNER JOIN Clienti c ON d.ClientID = c.ClientiId
            LEFT JOIN Angajati_Servicii a ON a.DispozitiveId = d.DispozitiveID
            WHERE d.Status = 'Finalizat'
        ";



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            decimal cost = reader["CostFinal"] != DBNull.Value ? Convert.ToDecimal(reader["CostFinal"]) : 0m;

                            // Dacă costul este NULL, îl căutăm în tabelul Servicii
                            if (cost == 0m && reader["ServiciiId"] != DBNull.Value)
                            {
                                int serviciiId = Convert.ToInt32(reader["ServiciiId"]);
                                cost = await GetCostFromServicii(serviciiId, connection);
                            }

                            rezultat.Add(new Models.Finalizat
                            {
                                ClientNume = reader["ClientNume"].ToString(),
                                ClientPrenume = reader["ClientPrenume"].ToString(),
                                TipDispozitiv = reader["TipDispozitiv"].ToString(),
                                Status = reader["Status"].ToString(),
                                Cost = cost
                            });
                        }
                    }
                }
            }
        }
        // Funcție pentru a cauta costul unui serviciu
        private async Task<decimal> GetCostFromServicii(int serviciiId, SqlConnection connection)
        {
            string sql = "SELECT Cost FROM Servicii WHERE ServiciiID = @ServiciiId";

            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@ServiciiId", serviciiId);

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToDecimal(result) : 0m;
            }
        }


    }
}

