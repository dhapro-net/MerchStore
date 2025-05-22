using System;
using MerchStore.Domain.Common;
using Xunit;
namespace MerchStoreTest.Domain.Common;
public class EntityTest
{
    private class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id) { }
        public TestEntity() : base(Guid.NewGuid()) { }
    }

    [Fact]
    public void Constructor_SetsId()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);
        Assert.Equal(id, entity.Id);
    }

    [Fact]
    public void Constructor_Throws_WhenIdIsDefault()
    {
        Assert.Throws<ArgumentException>(() => new TestEntity(default));
    }

    [Fact]
    public void Equals_ReturnsTrue_ForSameId()
    {
        var id = Guid.NewGuid();
        var e1 = new TestEntity(id);
        var e2 = new TestEntity(id);

        Assert.True(e1.Equals(e2));
        Assert.True(e1.Equals((object)e2));
        Assert.True(e1 == e2);
        Assert.False(e1 != e2);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentId()
    {
        var e1 = new TestEntity(Guid.NewGuid());
        var e2 = new TestEntity(Guid.NewGuid());

        Assert.False(e1.Equals(e2));
        Assert.False(e1.Equals((object)e2));
        Assert.False(e1 == e2);
        Assert.True(e1 != e2);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForNull()
    {
        var e1 = new TestEntity(Guid.NewGuid());
        Assert.False(e1.Equals(null));
        Assert.False(e1.Equals((object)null));
        Assert.False(e1 == null);
        Assert.True(e1 != null);
    }

    [Fact]
    public void GetHashCode_ReturnsIdHashCode()
    {
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);
        Assert.Equal(id.GetHashCode(), entity.GetHashCode());
    }
}