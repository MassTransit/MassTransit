namespace MassTransit.TestFramework.ForkJoint.Activities
{
    using System.Threading.Tasks;
    using MassTransit.Courier;
    using Microsoft.Extensions.Logging;
    using Services;


    public class GrillBurgerActivity :
        IActivity<GrillBurgerArguments, GrillBurgerLog>
    {
        readonly IGrill _grill;
        readonly ILogger<GrillBurgerActivity> _logger;

        public GrillBurgerActivity(ILogger<GrillBurgerActivity> logger, IGrill grill)
        {
            _logger = logger;
            _grill = grill;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<GrillBurgerArguments> context)
        {
            var patty = await _grill.CookOrUseExistingPatty(context.Arguments.Weight, context.Arguments.Cheese);

            return context.CompletedWithVariables<GrillBurgerLog>(new {patty}, new {patty});
        }

        public Task<CompensationResult> Compensate(CompensateContext<GrillBurgerLog> context)
        {
            var patty = context.Log.Patty;

            _logger.LogDebug("Putting Burger back in inventory: {Weight} {Cheese}", patty.Weight, patty.Cheese);

            _grill.Add(patty);

            return Task.FromResult(context.Compensated());
        }
    }
}
