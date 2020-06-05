// <copyright file="Counters.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;

    using Allors;

    public partial class Counters
    {
        private UniquelyIdentifiableSticky<Counter> cache;

        private UniquelyIdentifiableSticky<Counter> Cache => this.cache ??= new UniquelyIdentifiableSticky<Counter>(this.Session);

        public static int NextValue(ISession session, Guid counterId)
        {
            static int NextValue(Counter counter)
            {
                counter.Value += 1;
                return counter.Value;
            }

            if (session.Database.IsShared)
            {
                using var outOfBandSession = session.Database.CreateSession();
                var outOfBandCounter = new Counters(outOfBandSession).Cache[counterId];
                if (outOfBandCounter != null)
                {
                    var value = NextValue(outOfBandCounter);
                    outOfBandSession.Commit();
                    return value;
                }
            }

            var sessionCounter = new Counters(session).Cache[counterId];
            return NextValue(sessionCounter);
        }
    }
}
