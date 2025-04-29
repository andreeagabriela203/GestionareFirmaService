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
    public class PieseController : Controller
    {
        private readonly EFCoreDbContext _context;
        private string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;;Encrypt=False;";


        public PieseController(EFCoreDbContext context)
        {
            _context = context;
        }

        // GET: Piese
        public async Task<IActionResult> Index()
        {
            List<Piese> pieseList = new List<Piese>();

            string sql = "SELECT * FROM Piese"; // Interogarea SQL pentru tabela Piese

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); // Deschidem conexiunea asincron

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync()) // Citim rezultatele asincron
                    {
                        while (await reader.ReadAsync()) // Citim fiecare rând
                        {
                            pieseList.Add(new Piese
                            {
                                PieseId = reader.GetInt32(reader.GetOrdinal("PieseId")),
                                TipPiesa = reader.IsDBNull(reader.GetOrdinal("TipPiesa")) ? null : reader.GetString(reader.GetOrdinal("TipPiesa")),
                                Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? null : reader.GetString(reader.GetOrdinal("Model")),
                                Marca = reader.GetString(reader.GetOrdinal("Marca")), // Marca nu este nullable, deci nu avem nevoie de verificare pentru DBNull
                                TipDispozitiv = reader.GetString(reader.GetOrdinal("TipDispozitiv")),
                                Stoc = reader.GetInt32(reader.GetOrdinal("Stoc")),
                                Cost = reader.GetInt32(reader.GetOrdinal("Cost"))
                            });
                        }
                    }
                }
            }

            return View(pieseList); 
        }

        public Piese GetPiesaById(int id)
        {
            Piese piesa = null;
            string sql = "SELECT * FROM Piese WHERE PieseId = @PieseId";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                {
                    command.Parameters.AddWithValue("@PieseId", id);
                    sqlConnection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            piesa = new Piese
                            {
                                PieseId = reader.GetInt32(reader.GetOrdinal("PieseId")),
                                TipPiesa = reader.IsDBNull(reader.GetOrdinal("TipPiesa")) ? null : reader.GetString(reader.GetOrdinal("TipPiesa")),
                                Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? null : reader.GetString(reader.GetOrdinal("Model")),
                                Marca = reader.GetString(reader.GetOrdinal("Marca")),
                                TipDispozitiv = reader.GetString(reader.GetOrdinal("TipDispozitiv")),
                                Stoc = reader.GetInt32(reader.GetOrdinal("Stoc")),
                                Cost = reader.GetInt32(reader.GetOrdinal("Cost"))
                            };
                        }
                    }
                }
            }

            return piesa;
        }

  
        // GET: Piese/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Piese/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Piese piesa)
        {
            if (ModelState.IsValid)
            {
                // Interogarea SQL pentru Piese
                string sql = @"INSERT INTO Piese (TipPiesa, Model, Marca, TipDispozitiv, Stoc, Cost) 
                        VALUES (@TipPiesa, @Model, @Marca, @TipDispozitiv, @Stoc, @Cost)";

                // Conexiunea și comanda SQL pentru Piese
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Adăugăm parametrii
                        command.Parameters.AddWithValue("@TipPiesa", (object)piesa.TipPiesa ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Model", (object)piesa.Model ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Marca", piesa.Marca);
                        command.Parameters.AddWithValue("@TipDispozitiv", piesa.TipDispozitiv);
                        command.Parameters.AddWithValue("@Stoc", piesa.Stoc);
                        command.Parameters.AddWithValue("@Cost", piesa.Cost);

                        // Deschidem conexiunea
                        connection.Open();

                        // Executăm comanda
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(piesa); // Dacă există erori de validare, rămânem pe aceeași pagină
        }

        // GET: Piese/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var a = GetPiesaById(id);
            if (a == null)
            {
                return NotFound();
            }
            return View(a);
        }

        // POST: Piese/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Piese piesa)
        {
            if (id != piesa.PieseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string sql = @"UPDATE Piese
                       SET 
                           TipPiesa = @TipPiesa, Model = @Model, Marca = @Marca, 
                           TipDispozitiv = @TipDispozitiv, Stoc = @Stoc, Cost = @Cost
                       WHERE PieseId = @PieseId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PieseId", piesa.PieseId);
                        command.Parameters.AddWithValue("@TipPiesa", (object)piesa.TipPiesa ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Model", (object)piesa.Model ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Marca", piesa.Marca);
                        command.Parameters.AddWithValue("@TipDispozitiv", piesa.TipDispozitiv);
                        command.Parameters.AddWithValue("@Stoc", piesa.Stoc);
                        command.Parameters.AddWithValue("@Cost", piesa.Cost);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(piesa);
        }


        // GET: Piese/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var piese = await _context.Piese
                .FirstOrDefaultAsync(m => m.PieseId == id);
            if (piese == null)
            {
                return NotFound();
            }

            return View(piese);
        }

        // POST: Piese/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // SQL pentru ștergere
            string sql = "DELETE FROM Piese WHERE PieseId = @PieseId";

            // Executăm comanda SQL cu parametrul
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PieseId", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }


        private bool PieseExists(int id)
        {
            return _context.Piese.Any(e => e.PieseId == id);
        }
    }
}
