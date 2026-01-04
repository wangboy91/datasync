using DataSync.Application.DTOs;
using Xunit;

namespace DataSync.Tests.Tests;

public class SampleTests
{
    [Fact]
    public void SourceDto_Should_Init()
    {
        var s = new SourceDto(Guid.NewGuid(), "src", "mysql", "conn", "active");
        Assert.Equal("src", s.Name);
    }
}

