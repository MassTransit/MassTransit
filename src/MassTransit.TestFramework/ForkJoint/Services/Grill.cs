namespace MassTransit.TestFramework.ForkJoint.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.Logging;


    public class Grill :
        IGrill
    {
        readonly ILogger<Grill> _logger;
        readonly HashSet<BurgerPatty> _patties;

        public Grill(ILogger<Grill> logger)
        {
            _logger = logger;
            _patties = new HashSet<BurgerPatty>();
        }

        public async Task<BurgerPatty> CookOrUseExistingPatty(decimal weight, bool cheese)
        {
            var existing = _patties.FirstOrDefault(x => x.Cheese == cheese && x.Weight == weight);
            if (existing != null)
            {
                _logger.LogDebug("Using existing patty {Weight} {Cheese}", existing.Weight, existing.Cheese);

                _patties.Remove(existing);
                return existing;
            }

            _logger.LogDebug("Grilling patty {Weight} {Cheese}", weight, cheese);

            var patty = new BurgerPatty
            {
                Weight = weight,
                Cheese = cheese
            };

            return patty;
        }

        public void Add(BurgerPatty patty)
        {
            _patties.Add(patty);
        }
    }
}
