using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sygic.Corona.Infrastructure.Services.Authorization;
using MediatR;
using Sygic.Corona.Application.Validations;
using Sygic.Corona.Domain.Common;
using System.Linq;
using System.Threading;
using Sygic.Corona.Contracts.Requests;
using Sygic.Corona.Application.Commands;

namespace Sygic.Corona.QuarantineApi
{
    public class UpdateProfile
    {
        private readonly ISignVerification verification;
        private readonly IMediator mediator;
        private readonly ValidationProcessor validation;

        public UpdateProfile(ISignVerification verification, IMediator mediator, ValidationProcessor validation)
        {
            this.verification = verification ?? throw new ArgumentNullException(nameof(verification));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.validation = validation ?? throw new ArgumentNullException(nameof(validation));
        }

        [FunctionName("UpdateProfile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "profile")] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                string signedAttestation = req.Headers["X-SignedSafetyNet"].ToString();
                string[] signatureHeaderParameters = req.Headers["X-Signature"].ToString().Split(':');
                if (signatureHeaderParameters.Length != 2)
                {
                    return new BadRequestResult();
                }

                var publicKey = signatureHeaderParameters.First();
                var isVerified = verification.Verify(requestBody, publicKey, signatureHeaderParameters.Last());

                if (!isVerified)
                {
                    return new UnauthorizedResult();
                }

                var data = JsonConvert.DeserializeObject<VerifyProfileRequest>(requestBody);

                var command = new VerifyProfileCommand(data.DeviceId, data.ProfileId, data.CovidPass, data.Nonce, publicKey, signedAttestation);
                await mediator.Send(command, cancellationToken);

                return new OkResult();
            }
            catch (DomainException ex)
            {
                var errors = validation.ProcessErrors(ex);
                return new BadRequestObjectResult(errors);
            }
        }
    }
}
