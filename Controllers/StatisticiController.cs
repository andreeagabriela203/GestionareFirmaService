using FirmaService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirmaService.Controllers
{
    public class StatisticiController : Controller
    {
        private readonly string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;Encrypt=False;";

        public async Task<IActionResult> Index()
        {
            ViewBag.IsSearchPerformed = false;
			// Obținem angajatul lunii
			Statistici rezultat = await AngajatulLunii();
            rezultat.Cost = await CostTotal();
            PiesaUtilizataCelMaides(rezultat);
            PiesaNeutilizata(rezultat);
            GetDispSiCostMediu(rezultat);
            return View(rezultat);
        }

        public async Task<Statistici> AngajatulLunii()
        {
            string sql = @"
    SELECT a.Nume, a.Prenume, COUNT(a2.DispozitiveId) AS DispozitiveCount
    FROM Angajati a
    INNER JOIN Angajati_Servicii a2 ON a.AngajatiId = a2.AngajatiId
    INNER JOIN Dispozitive d ON a2.DispozitiveId = d.DispozitiveID
    WHERE d.Status = 'Finalizat'
    GROUP BY a.Nume, a.Prenume
    HAVING COUNT(a2.DispozitiveId) = (
        SELECT TOP 1 COUNT(a2.DispozitiveId)
        FROM Angajati_Servicii a2
        INNER JOIN Dispozitive d ON a2.DispozitiveId = d.DispozitiveID
        WHERE d.Status = 'Finalizat'
        GROUP BY a2.AngajatiId
        ORDER BY COUNT(a2.DispozitiveId) DESC
    )";

            Statistici rezultat = new Statistici();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Preia valorile
                            string nume = reader["Nume"].ToString();
                            string prenume = reader["Prenume"].ToString();

                            // Dacă DispozitiveCount este null, setăm la 0
                            int dispozitiveCount = reader["DispozitiveCount"] != DBNull.Value ? Convert.ToInt32(reader["DispozitiveCount"]) : 0;

                            // Atribuie valorile în obiectul rezultat
                            rezultat.Nume = nume;
                            rezultat.Prenume = prenume;
                            rezultat.NrDispozitive = dispozitiveCount; // Aici e valoarea numerică pe care o vrei
                        }
                    }
                }
            }

            return rezultat;
        }

        public async Task<int> CostTotal()
        {
            string sql = @"SELECT TOP 1 SUM(a2.Cost) as Suma 
                   FROM Angajati_Servicii a2 
                   INNER JOIN Dispozitive d ON d.DispozitiveID = a2.DispozitiveId
                   WHERE d.Status = 'Finalizat'";

            int cost = 0; // Inițializarea costului

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            // Verifică dacă valoarea este NULL
                            cost = reader["Suma"] != DBNull.Value ? Convert.ToInt32(reader["Suma"]) : 0;
                        }
                    }
                }
            }

            return cost;
        }

        public async Task PiesaUtilizataCelMaides(Statistici statistici)
        {
            string sql = @"  SELECT TOP 1 p.TipPiesa,p.Stoc, COUNT(a2.PieseId) AS Utilizari
        FROM Piese p
        INNER JOIN Angajati_Servicii a2 ON p.PieseId = a2.PieseId
        GROUP BY p.TipPiesa, p.Stoc
        HAVING COUNT(a2.PieseId) = (
            SELECT TOP 1 UltilizariPerPiesa
            FROM (
                SELECT COUNT(a2.PieseId) AS UltilizariPerPiesa
                FROM Angajati_Servicii a2
                INNER JOIN Piese p2 ON p2.PieseId = a2.PieseId
                GROUP BY p2.TipPiesa
            ) as piesa
        )";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); 
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync()) 
                        {
                            // Preia valorile
                            string tippiesa = reader["TipPiesa"].ToString();
                            int utilizari = reader["Utilizari"] != DBNull.Value ? Convert.ToInt32(reader["Utilizari"]) : 0;
                            int stoc = reader["Stoc"] != DBNull.Value ? Convert.ToInt32(reader["Stoc"]) : 0;
                            // Atribuie valorile în obiectul rezultat
                            statistici.Decateori = utilizari;
                            statistici.PiesaUtilizata = tippiesa;
                            statistici.Stoc = stoc;
                        }
                        else
                        {
                            // Dacă nu există date, setăm valori implicite
                            statistici.Decateori = 0;
                            statistici.PiesaUtilizata = "N/A";
                        }
                    }
                }
            }
        }

        public async Task PiesaNeutilizata(Statistici statistici)
        {
            string sql = @"
        SELECT p.TipPiesa, p.Marca, p.Model, p.Stoc, p.Cost
        FROM Piese p
        WHERE  p.PieseId NOT IN (
            SELECT DISTINCT a2.PieseId
            FROM Angajati_Servicii a2
            WHERE a2.DataFinalizare IS NOT NULL 
            AND a2.DataFinalizare > DATEADD(MONTH, -6, GETDATE())
            AND a2.PieseId IS NOT NULL
        )";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        // Inițializează liste
                        if (statistici.TipPiesa == null) statistici.TipPiesa = new List<string>();
                        if (statistici.Marca == null) statistici.Marca = new List<string>();
                        if (statistici.Model == null) statistici.Model = new List<string>();
                        if (statistici.StocPiesaNeutilizata == null) statistici.StocPiesaNeutilizata = new List<int>();
                        if (statistici.CostPiesaNeutilizata == null) statistici.CostPiesaNeutilizata = new List<int>();

                        // Adaugare valori din interogare
                        while (await reader.ReadAsync())
                        {
                            statistici.TipPiesa.Add(reader["TipPiesa"].ToString());
                            statistici.Marca.Add(reader["Marca"].ToString());
                            statistici.Model.Add(reader["Model"].ToString());
                            statistici.StocPiesaNeutilizata.Add(Convert.ToInt32(reader["Stoc"]));
                            statistici.CostPiesaNeutilizata.Add(Convert.ToInt32(reader["Cost"]));
                        }
                    }
                }
            }
        }
        //GET
        public async Task<IActionResult> Angajati(int nrLuni = 1)
        {
            if (nrLuni < 1 || nrLuni > 3)
            {
                return BadRequest("Numărul de luni trebuie să fie între 1 și 3.");
            }

            List<NuAuLucrat> angajati = await GetAngajatiNuauLucrat(nrLuni);
            return View(angajati);
        }

        // POST: Metodă pentru obținerea datelor din baza de date
        private async Task<List<NuAuLucrat>> GetAngajatiNuauLucrat(int nrLuni)
        {
            string sql = @"
        SELECT a.Nume, a.Prenume, a.Salariu
        FROM Angajati a
        WHERE a.AngajatiId NOT IN (
            SELECT DISTINCT a_s.AngajatiId
            FROM Angajati_Servicii a_s
            JOIN Dispozitive d ON a_s.DispozitiveId = d.DispozitiveID
            WHERE d.Status = 'Finalizat' AND a_s.DataFinalizare > DATEADD(MONTH, -@NrLuni, GETDATE())
        )";

            List<NuAuLucrat> rezultat = new List<NuAuLucrat>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NrLuni", nrLuni);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rezultat.Add(new NuAuLucrat
                            {
                                NumeAngajat = reader["Nume"].ToString(),
                                PrenumeAngajat = reader["Prenume"].ToString(),
                                Salariu = Convert.ToInt32(reader["Salariu"])
                            });
                        }
                    }
                }
            }

            return rezultat;
        }

        public async Task GetDispSiCostMediu(Statistici statistici)
        {
            string sql = @"
        SELECT d.TipDispozitiv, AVG(a2.Cost) AS CostMediu
        FROM Dispozitive d
        INNER JOIN Angajati_Servicii a2 ON d.DispozitiveID = a2.DispozitiveId
        GROUP BY d.TipDispozitiv;
    ";
            statistici.Disp = new List<string>();
            statistici.CostMediu = new List<decimal>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            // Preluăm datele pentru TipDispozitiv și CostMediu
                            string tipDispozitiv = reader["TipDispozitiv"].ToString();
                            decimal costMediu = Convert.ToDecimal(reader["CostMediu"]);

                            // Adăugăm în listele din obiectul Statistici
                            statistici.Disp.Add(tipDispozitiv);
                            statistici.CostMediu.Add(costMediu);
                        }
                    }
                }
            }
        }
        [HttpGet]
        public IActionResult IndexAng()
        {
            return View();
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> IndexAng(string NumeAng, string PrenumeAng)
        {
            if (string.IsNullOrEmpty(NumeAng) || string.IsNullOrEmpty(PrenumeAng))
            {
                ViewBag.Error = "Vă rugăm să completați toate câmpurile.";
                return View();
            }

            decimal venitMediu = await GetVenitMediuperAngajat(NumeAng, PrenumeAng);

            ViewBag.VenitMediu = venitMediu;
            ViewBag.NumeAng = NumeAng;
            ViewBag.PrenumeAng = PrenumeAng;
            return View();
        }

        public async Task<decimal> GetVenitMediuperAngajat(string NumeAng, string PrenumeAng)
        {
           
            ViewBag.IsSearchPerformed = true;
            string sql = @"
        SELECT a.Nume, a.Prenume, SUM(a2.Cost) AS VenitGenerat
        FROM Angajati a
        INNER JOIN Angajati_Servicii a2 ON a.AngajatiId = a2.AngajatiId
        WHERE a.Nume = @Nume AND a.Prenume = @Prenume
        GROUP BY a.Nume, a.Prenume
        ";
            decimal VenitMediu=0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    
                        command.Parameters.AddWithValue("@Nume", NumeAng);
                        command.Parameters.AddWithValue("@Prenume", PrenumeAng);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                VenitMediu = Convert.ToDecimal(reader["VenitGenerat"]);
                            }
                        }
                    
                }
            }
            return VenitMediu;
        }
    }

}


