using System;
using MerchStore.Domain.Common;
using Xunit;

namespace MerchStoreTest.Domain.Common;
public class AggregateRootTest
{
    private class TestAggregate : AggregateRoot<Guid>
    {
        public TestAggregate(Guid id) : base(id) { }
        public void AddEvent(DomainEvent evt) => AddDomainEvent(evt);
    }

    private class DummyDomainEvent : DomainEvent { }

    [Fact]
    public void AddDomainEvent_AddsEventToCollection()
    {
        var agg = new TestAggregate(Guid.NewGuid());
        var evt = new DummyDomainEvent();

        agg.AddEvent(evt);

        Assert.Single(agg.DomainEvents);
        Assert.Contains(evt, agg.DomainEvents);
    }

    [Fact]
    public void AddDomainEvent_Throws_WhenNull()
    {
        var agg = new TestAggregate(Guid.NewGuid());
        Assert.Throws<ArgumentNullException>(() => agg.AddEvent(null));
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var agg = new TestAggregate(Guid.NewGuid());
        agg.AddEvent(new DummyDomainEvent());
        agg.AddEvent(new DummyDomainEvent());

        agg.ClearDomainEvents();

        Assert.Empty(agg.DomainEvents);
    }
}