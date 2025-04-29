using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirmaService;
using FirmaService.Models;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.SqlClient;

namespace FirmaService.Controllers
{
    public class ClientiController : Controller
    {
        private readonly EFCoreDbContext _context;
        private string connectionString = "Server=LAPTOP-JIGFAA7Q;Database=FirmaService;Integrated Security=True;;Encrypt=False;";
        public ClientiController(EFCoreDbContext context)
        {
           _context = context;
        }

        // GET: Afisare Clienti in pagina
        public async Task<IActionResult> Index()
        {
            List<Clienti> clientiList = new List<Clienti>();

            string sql = "SELECT * FROM Clienti";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clientiList.Add(new Clienti
                            {
                                ClientiId = reader.GetInt32(reader.GetOrdinal("ClientiId")),
                                Nume = reader.GetString(reader.GetOrdinal("Nume")),
                                Prenume = reader.GetString(reader.GetOrdinal("Prenume")),
                                NrTelefon = reader.IsDBNull(reader.GetOrdinal("NrTelefon")) ? null : reader.GetInt32(reader.GetOrdinal("NrTelefon")),
                                AdresaEmail = reader.IsDBNull(reader.GetOrdinal("AdresaEmail")) ? null : reader.GetString(reader.GetOrdinal("AdresaEmail")),
                          });
                        }
                    }
                }
            }

            return View(clientiList);
        }
        public Clienti GetClientById(int id)
        {
            Clienti client = null;
            string sql = "SELECT * FROM Clienti WHERE ClientiID = @ClientiId";

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, sqlConnection))
                {
                    command.Parameters.AddWithValue("@ClientiId", id);
                    sqlConnection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            client = new Clienti
                            {
                                ClientiId = reader.GetInt32(reader.GetOrdinal("ClientiId")),
                                Nume = reader.GetString(reader.GetOrdinal("Nume")),
                                Prenume = reader.GetString(reader.GetOrdinal("Prenume")),
                                NrTelefon = reader.IsDBNull(reader.GetOrdinal("NrTelefon")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("NrTelefon")),
                                AdresaEmail = reader.IsDBNull(reader.GetOrdinal("AdresaEmail")) ? null : reader.GetString(reader.GetOrdinal("AdresaEmail"))
                            };
                        }
                    }
                }
            }

            return client;
        }

        // GET: Clienti/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clienti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Clienti client)
        {
            if (ModelState.IsValid)
            {
                // Interogarea SQL
                string sql = @"INSERT INTO Clienti (Nume, Prenume, NrTelefon, AdresaEmail) 
                       VALUES (@Nume, @Prenume, @NrTelefon, @AdresaEmail)";

                // Conexiunea și comanda SQL
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Adăugăm parametrii
                        command.Parameters.AddWithValue("@Nume", client.Nume);
                        command.Parameters.AddWithValue("@Prenume", client.Prenume);
                        command.Parameters.AddWithValue("@NrTelefon", (object)client.NrTelefon ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AdresaEmail", (object)client.AdresaEmail ?? DBNull.Value);

                        // Deschidem conexiunea
                        connection.Open();

                        // Executăm comanda
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            // Dacă există erori de validare, rămânem pe aceeași pagină
            return View(client);
        }

        // GET: Clienti/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var client = GetClientById(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }


        // POST: Clienti/Edit

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Clienti client)
        {
            if (id != client.ClientiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string sql = @"UPDATE Clienti
                       SET 
                           Nume = @Nume, Prenume = @Prenume, NrTelefon = @NrTelefon, 
                           AdresaEmail = @AdresaEmail
                       WHERE ClientiId = @ClientiId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ClientiId", client.ClientiId);
                        command.Parameters.AddWithValue("@Nume", client.Nume);
                        command.Parameters.AddWithValue("@Prenume", client.Prenume);
                        command.Parameters.AddWithValue("@NrTelefon", (object)client.NrTelefon ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AdresaEmail", (object)client.AdresaEmail ?? DBNull.Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(client);
        }




        // GET: Clienti/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClientById(id);
            return View(client);
        }

        // POST: Clienti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // SQL pentru ștergere
            string sql = "DELETE FROM Clienti WHERE ClientiId = @ClientiId";

            // Executăm comanda SQL cu parametrul
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ClientiId", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> GetClientIdByName(string nume, string prenume)
        {
            if (string.IsNullOrEmpty(nume) || string.IsNullOrEmpty(prenume))
            {
                return Json(null);
            }

            string sql = @"SELECT ClientiId FROM Clienti WHERE Nume = @Nume AND Prenume = @Prenume";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nume", nume);
                    command.Parameters.AddWithValue("@Prenume", prenume);

                    var result = await command.ExecuteScalarAsync();

                    if (result != null)
                    {
                        return Json(new { ClientID = result });
                    }
                }
            }

            return Json(null);
        }
        private bool ClientiExists(int id)
        {
            return _context.Clienti.Any(e => e.ClientiId == id);
        }
    }
}
