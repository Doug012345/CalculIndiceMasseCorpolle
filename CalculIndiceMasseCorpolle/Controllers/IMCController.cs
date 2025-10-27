using CalculIndiceMasseCorpolle.Models;
using CalculIndiceMasseCorpolle.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalculIndiceMasseCorpolle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IMCController : ControllerBase
    {
        private readonly IIMCService _imcService;
        private readonly ApplicationDbContext _context;

        public IMCController(IIMCService imcService, ApplicationDbContext context)
        {
            _imcService = imcService;
            _context = context;
        }

        // 🔥 NOUVEL ENDPOINT - Racine du contrôleur
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(new
            {
                message = "🎯 API Calcul IMC - Bienvenue!",
                description = "API pour calculer l'Indice de Masse Corporelle",
                endpoints = new[]
                {
                    new { method = "POST", path = "/api/imc/calculer", description = "Calculer un IMC" },
                    new { method = "GET", path = "/api/imc/historique", description = "Voir l'historique complet" },
                    new { method = "GET", path = "/api/imc/historique/{nom}", description = "Historique par nom" },
                    new { method = "GET", path = "/api/imc/statistiques", description = "Statistiques des calculs" },
                    new { method = "GET", path = "/api/imc/test", description = "Test du contrôleur" }
                },
                timestamp = DateTime.Now
            });
        }

        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok(new
            {
                message = "✅ Contrôleur IMC fonctionne!",
                status = "Operational",
                timestamp = DateTime.Now
            });
        }

        //[HttpPost("calculer")]
        //public async Task<ActionResult<IMCResponse>> CalculerIMC([FromBody] IMCRequest request)
        //{
        //    try
        //    {
        //        // Validation des données
        //        if (string.IsNullOrWhiteSpace(request.Nom))
        //            return BadRequest("Le nom est obligatoire");

        //        if (request.Poids <= 0 || request.Taille <= 0)
        //            return BadRequest("Le poids et la taille doivent être supérieurs à 0");

        //        // Calcul de l'IMC
        //        var result = _imcService.CalculerImc(request);

        //        // Sauvegarde en base de données
        //        var calculIMC = new CalculIMC
        //        {
        //            Nom = result.Nom,
        //            Poids = result.Poids,
        //            Taille = result.Taille,
        //            IMC = result.IMC,
        //            Categorie = result.Categorie,
        //            DateCalcul = result.DateCalcul
        //        };

        //        _context.CalculsIMC.Add(calculIMC);
        //        await _context.SaveChangesAsync();

        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
        //    }
        //}

        [HttpPost("calculer")]
        public async Task<ActionResult<IMCResponse>> CalculerIMC([FromBody] IMCRequest request)
        {
            try
            {
                Console.WriteLine($"🔍 Requête reçue: Nom={request.Nom}, Poids={request.Poids}, Taille={request.Taille}");

                // Validation des données
                if (string.IsNullOrWhiteSpace(request.Nom))
                    return BadRequest("Le nom est obligatoire");

                if (request.Poids <= 0 || request.Taille <= 0)
                    return BadRequest("Le poids et la taille doivent être supérieurs à 0");

                // Calcul de l'IMC
                var result = _imcService.CalculerImc(request);
                Console.WriteLine($"🔍 IMC calculé: {result.IMC}, Catégorie: {result.Categorie}");

                // Sauvegarde en base de données
                var calculIMC = new CalculIMC
                {
                    Nom = result.Nom,
                    Poids = result.Poids,
                    Taille = result.Taille,
                    IMC = result.IMC,
                    Categorie = result.Categorie,
                    DateCalcul = result.DateCalcul
                };

                _context.CalculsIMC.Add(calculIMC);
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Données sauvegardées en base");

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Erreur interne du serveur: {ex.Message}");
            }
        }

        [HttpGet("historique")]
        public async Task<ActionResult<IEnumerable<CalculIMC>>> GetHistorique()
        {
            try
            {
                var historique = await _context.CalculsIMC
                    .OrderByDescending(x => x.DateCalcul)
                    .ToListAsync();

                return Ok(historique);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération de l'historique: {ex.Message}");
            }
        }

        [HttpGet("historique/{nom}")]
        public async Task<ActionResult<IEnumerable<CalculIMC>>> GetHistoriqueParNom(string nom)
        {
            try
            {
                var historique = await _context.CalculsIMC
                    .Where(x => x.Nom.ToLower() == nom.ToLower())
                    .OrderByDescending(x => x.DateCalcul)
                    .ToListAsync();

                return Ok(historique);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération de l'historique: {ex.Message}");
            }
        }

        [HttpGet("statistiques")]
        public async Task<ActionResult<object>> GetStatistiques()
        {
            try
            {
                var totalCalculs = await _context.CalculsIMC.CountAsync();

                var statistiques = new
                {
                    TotalCalculs = totalCalculs,
                    Categories = await _context.CalculsIMC
                        .GroupBy(x => x.Categorie)
                        .Select(g => new
                        {
                            Categorie = g.Key,
                            Count = g.Count(),
                            Pourcentage = totalCalculs > 0 ? Math.Round((g.Count() * 100.0) / totalCalculs, 2) : 0
                        })
                        .ToListAsync(),
                    DernierCalcul = await _context.CalculsIMC
                        .OrderByDescending(x => x.DateCalcul)
                        .FirstOrDefaultAsync()
                };

                return Ok(statistiques);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur lors de la récupération des statistiques: {ex.Message}");
            }
        }
    }
}