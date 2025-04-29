using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace FirmaService.Controllers
{
    public class InLucruController : Controller
    {
        private string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;Encrypt=False;";

        public async Task<IActionResult> IndexAsync()
        {
            List<Models.InLucru> rezultat = new List<Models.InLucru>();
        
            await StocZero(rezultat);
            return View(rezultat);
        }
        public async Task StocZero(List<Models.InLucru> rezultat)
        {
            string sql = @"
        SELECT d.TipDispozitiv, d.Marca, d.Model, p.TipPiesa
        FROM Dispozitive d 
        INNER JOIN Angajati_Servicii a2 ON d.DispozitiveID = a2.DispozitiveId 
        LEFT JOIN Piese p ON p.PieseId = a2.PieseId 
        WHERE d.Status = 'In lucru' AND (p.Stoc = 0 AND a2.PieseId IS NOT NULL)
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
                            string tipDispozitiv = reader["TipDispozitiv"].ToString();
                            string marca = reader["Marca"].ToString();  // Poti să adaugi și acest câmp dacă vrei
                            string model = reader["Model"].ToString();  // Poti să adaugi și acest câmp dacă vrei
                            string tipPiesa = reader["TipPiesa"].ToString();

                            // Crearea unui obiect InLucru și adăugarea acestuia în listă
                            rezultat.Add(new Models.InLucru
                            {
                                TipDispozitiv = tipDispozitiv,
                                Marca=marca,
                                Model=model,
                                piesa_lipsa=tipPiesa
                            }) ;
                            
                        }
                    }
                }
            }
        }

    }
}

