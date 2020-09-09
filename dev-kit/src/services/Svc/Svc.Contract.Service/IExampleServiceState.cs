﻿using Core.Messages;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading;
using System.Threading.Tasks;
using Svc.Contract.Service.Models;

namespace Svc.Contract.Service
{
    public interface IExampleServiceState : IInternalHttpService
    {
        Task<string> Ping(CancellationToken token);
        Task<string> ProcessByteArray(byte[] file, CrossContext ctx, CancellationToken token);
        Task<string> ProcessArrayOfByteArray(IReadOnlyCollection<byte[]> file, CrossContext ctx, CancellationToken token);
        Task<CrossContext> ProcessComplexModel(ComplexModel file, CrossContext ctx, CancellationToken token);
        Task<ComplexModel> GetComplexModel(CrossContext ctx, CancellationToken token);
    }
}
