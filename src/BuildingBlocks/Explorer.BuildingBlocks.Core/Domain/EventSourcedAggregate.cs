﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.BuildingBlocks.Core.Domain
{
    public abstract class EventSourcedAggregate : Entity
    {
        [JsonPropertyName("changes")]
        public virtual List<DomainEvent> Changes { get; set; }
        public int Version { get; protected set; }
        public EventSourcedAggregate()
        {
            Changes = new List<DomainEvent>();
        }
        public abstract void Apply(DomainEvent changes);
    }
}
