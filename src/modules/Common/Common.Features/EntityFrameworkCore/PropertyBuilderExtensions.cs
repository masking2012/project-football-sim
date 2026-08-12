using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace ProjectFootballSim.Common.Features.EntityFrameworkCore;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<ICollection<T>?> UseJsonCollection<T>(
        this PropertyBuilder<ICollection<T>?> builder)
    {
        var converter = new ValueConverter<ICollection<T>?, string?>(
            v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => v == null ? null : JsonSerializer.Deserialize<ICollection<T>>(v, (JsonSerializerOptions?)null));

        var comparer = new ValueComparer<ICollection<T>?>(
            (a, b) => (a ?? Array.Empty<T>()).SequenceEqual(b ?? Array.Empty<T>()),
            a => a == null ? 0 : a.Aggregate(0, (h, v) => HashCode.Combine(h, v)),
            a => a == null ? null : a.ToList());

        builder.HasConversion(converter);
        builder.Metadata.SetValueComparer(comparer);

        return builder;
    }
}
