﻿// The MIT License (MIT)
//
// Copyright (c) 2015 Rasmus Mikkelsen
// https://github.com/rasmus/EventFlow
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.ReadStores;

namespace EventFlow.Subscribers
{
    public class DomainEventPublisher : IDomainEventPublisher
    {
        private readonly IDispatchToEventSubscribers _dispatchToEventSubscribers;
        private readonly IReadStoreManager _readStoreManager;

        public DomainEventPublisher(
            IDispatchToEventSubscribers dispatchToEventSubscribers,
            IReadStoreManager readStoreManager)
        {
            _dispatchToEventSubscribers = dispatchToEventSubscribers;
            _readStoreManager = readStoreManager;
        }

        public async Task PublishAsync<TAggregate, TIdentity>(
            TIdentity id,
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken cancellationToken)
            where TAggregate : IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            // ARGH, dilemma, should we pass the cancellation token to read model update or not?
            var readStoreTask = _readStoreManager.UpdateReadStoresAsync<TAggregate, TIdentity>(id, domainEvents, CancellationToken.None);

            var subscriberTask = _dispatchToEventSubscribers.DispatchAsync(domainEvents, cancellationToken);

            await Task.WhenAll(readStoreTask, subscriberTask).ConfigureAwait(false);
        }
    }
}
