﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sygic.Corona.Domain;
using Sygic.Corona.Infrastructure;
using Sygic.Corona.Infrastructure.Services.DateTimeConverting;

namespace Sygic.Corona.Application.Commands
{
    public class AddExposureKeysCommandHandler : AsyncRequestHandler<AddExposureKeysCommand>
    {
        private readonly CoronaContext context;
        private readonly IDateTimeConvertService convertService;

        public AddExposureKeysCommandHandler(CoronaContext context, IDateTimeConvertService convertService)
        {
            this.context = context;
            this.convertService = convertService;
        }

        protected override async Task Handle(AddExposureKeysCommand request, CancellationToken cancellationToken)
        {
            foreach (var key in request.ExposureKeys)
            {
                var now = DateTime.UtcNow;
                var createdOn = now;
                var expiration = now.Add(request.ExpirationTime);
                var exposureKey = new ExposureKey(key.TemporaryExposureKey, key.RollingStartNumber, key.RollingDuration, createdOn, expiration);
                await context.ExposureKeys.AddAsync(exposureKey, cancellationToken);
            }

            await context.SaveEntitiesAsync(cancellationToken);
        }
    }
}
