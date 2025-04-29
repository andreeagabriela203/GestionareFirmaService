using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirmaService;
using FirmaService.Models;
using Microsoft.Data.SqlClient;

namespace FirmaService.Controllers
{
    public class ServiciiController : Controller
    {
        private readonly EFCoreDbContext _context;
        private string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;;Encrypt=False;";


        public ServiciiController(EFCoreDbContext context)
        {
            _context = context;
        }

        // GET: Servicii
        public async Task<IActionResult> Index()
        {
            List<Servicii> serviciiList = new List<Servicii>();

            string sql = "SELECT * FROM Servicii"; // Interogarea SQL pentru tabela Servicii

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); // Deschidem conexiunea asincron

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) // Citim rezultatele asincron
                    {
                        while (await reader.ReadAsync()) // Citim fiecare rând
                        {
                            serviciiList.Add(new Servicii
                            {
                                ServiciiId = reader.GetInt32(reader.GetOrdinal("ServiciiId")),
                                Denumire = reader.GetString(reader.GetOrdinal("Denumire")),
                                DurataEstimata = reader.IsDBNull(reader.GetOrdinal("DurataEstimata")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("DurataEstimata")),
                                CostServiciu = reader.IsDBNull(reader.GetOrdinal("CostServiciu")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CostServiciu"))
                            });
                        }
                    }
                }
            }

            return View(serviciiList); // Returnăm lista către view
        }

        public Servicii GetServiciuById(int id)
        {
            Servicii serviciu = null;
            string sql = "SELECT * FROM Servicii WHERE ServiciiId = @ServiciiId";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                {
                    command.Parameters.AddWithValue("@ServiciiId", id);
                    sqlConnection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            serviciu = new Servicii
                            {
                                ServiciiId = reader.GetInt32(reader.GetOrdinal("ServiciiId")),
                                Denumire = reader.GetString(reader.GetOrdinal("Denumire")),
                                DurataEstimata = reader.IsDBNull(reader.GetOrdinal("DurataEstimata")) ? (long?)null : reader.GetInt64(reader.GetOrdinal("DurataEstimata")),
                                CostServiciu = reader.IsDBNull(reader.GetOrdinal("CostServiciu")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CostServiciu"))
                            };
                        }
                    }
                }
            }

            return serviciu;
        }

        // GET: Servicii/Details/5
   
        // GET: Servicii/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Servicii/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Servicii serviciu)
        {
            if (ModelState.IsValid)
            {
                // Interogarea SQL pentru Servicii
                string sql = @"INSERT INTO Servicii (Denumire, DurataEstimata, CostServiciu) 
                        VALUES (@Denumire, @DurataEstimata, @CostServiciu)";

                // Conexiunea și comanda SQL pentru Servicii
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Adăugăm parametrii
                        command.Parameters.AddWithValue("@Denumire", serviciu.Denumire);
                        command.Parameters.AddWithValue("@DurataEstimata", (object)serviciu.DurataEstimata ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CostServiciu", (object)serviciu.CostServiciu ?? DBNull.Value);

                        // Deschidem conexiunea
                        connection.Open();

                        // Executăm comanda
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(serviciu); // Dacă există erori de validare, rămânem pe aceeași pagină
        }

        // GET: Servicii/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var a = GetServiciuById(id);
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }

        // POST: Servicii/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Servicii serviciu)
        {
            if (id != serviciu.ServiciiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string sql = @"UPDATE Servicii
                       SET 
                           Denumire = @Denumire, DurataEstimata = @DurataEstimata, 
                           CostServiciu = @CostServiciu
                       WHERE ServiciiId = @ServiciiId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ServiciiId", serviciu.ServiciiId);
                        command.Parameters.AddWithValue("@Denumire", serviciu.Denumire);
                        command.Parameters.AddWithValue("@DurataEstimata", (object)serviciu.DurataEstimata ?? DBNull.Value);
                        command.Parameters.AddWithValue("@CostServiciu", (object)serviciu.CostServiciu ?? DBNull.Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(serviciu);
        }


        // GET: Servicii/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicii = await _context.Servicii
                .FirstOrDefaultAsync(m => m.ServiciiId == id);
            if (servicii == null)
            {
                return NotFound();
            }

            return View(servicii);
        }

        // POST: Servicii/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // SQL pentru ștergere
            string sql = "DELETE FROM Servicii WHERE ServiciiId = @ServiciiId";

            // Executăm comanda SQL cu parametrul
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ServiciiId", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }


        private bool ServiciiExists(int id)
        {
            return _context.Servicii.Any(e => e.ServiciiId == id);
        }
    }
}
