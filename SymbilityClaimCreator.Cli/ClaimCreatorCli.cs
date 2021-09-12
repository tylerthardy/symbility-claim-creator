using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MockDataUtils;
using SymbilityClaimAccess;
using SymbilityClaimAccess.Models.Configuration;
using SymbilityClaimAccess.Models.Extensions;
using Task = System.Threading.Tasks.Task;
using TimeZone = SymbilityClaimAccess.TimeZone;

namespace SymbilityClaimCreator.Cli
{
    public class ClaimCreatorCli : IHostedService
    {
        private ClaimCreator _claimCreator;

        public ClaimCreatorCli(IOptions<ClaimCreatorConfiguration> options, MockAddressGenerator mockAddressGenerator)
        {
            _claimCreator = new ClaimCreator(options.Value, mockAddressGenerator);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (PromptUser("Are you ready to begin? (Y/n)") == "n")
            {
                Exit();
                return;
            }

            var method = PromptUser("Choose a method (1: create claim, 2: create & assign to IA, 3: create, assigned to IA, & assign to adjuster):");

            switch (method)
            {
                case "1":
                    await _claimCreator.CreateSourceClaim();
                    return;
                case "2":
                    var c2 = await _claimCreator.CreateSourceClaim();
                    await _claimCreator.AssignToFirstAssignee(c2);
                    return;
                case "3":
                    var c3 = await _claimCreator.CreateSourceClaim();
                    var fca1 = await _claimCreator.AssignToFirstAssignee(c3);
                    await _claimCreator.AssignToSecondAssignee(c3, fca1);
                    return;
                default:
                    Console.WriteLine("Unknown method (should a number from the list above)");
                    Exit();
                    return;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private string PromptUser(string question)
        {
            Console.Write(question + " ");
            var answer = Console.ReadLine();
            Console.WriteLine();
            return answer;
        }

        private void Exit()
        {
            Console.WriteLine("Exiting...");
            Console.ReadLine();
        }
    }
}
