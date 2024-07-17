using System.Web.Mvc;
using TsRandomizer;
using TsRandomizer.Randomisation;

namespace OnlineSeedGeneratah.Controllers
{
    public class GenerateController : Controller
    {
        const uint TourmanentFlags = 0x1CBF89;

        // GET: Generate
        public ActionResult Index()
        {
	        var result = Randomizer.Generate(FillingMethod.Random, new SeedOptions(TourmanentFlags));

	        ViewData["Seed"] = result.Seed.ToString();
	        ViewData["Iterations"] = result.Itterations.ToString();
	        ViewData["TimeElapsed"] = result.Elapsed.ToString("c");

            return View();
        }

	    public ActionResult Json()
	    {
		    var result =  Randomizer.Generate(FillingMethod.Random, new SeedOptions(TourmanentFlags));

		    var seedResult = new SeedResult
		    {
			    Seed = result.Seed.ToString(),
			    Iterations = result.Itterations,
			    GenerationTime = result.Elapsed.ToString("c")
			};

		    return Json(seedResult, JsonRequestBehavior.AllowGet);
	    }

	    public class SeedResult
	    {
			public string Seed { get; set; }
			public int Iterations { get; set; }
			public string GenerationTime { get; set; }
		}
	}
}