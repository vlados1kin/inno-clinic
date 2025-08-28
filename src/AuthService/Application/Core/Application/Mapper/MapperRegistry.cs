using Application.Contracts.Mapper;

namespace Application.Mapper;

public sealed class MapperRegistry
{
    private readonly Dictionary<(Type, Type), object> _mappers = [];

    public void Registry<TSource, TDestination>(IMapper<TSource, TDestination> mapper)
    {
        if (_mappers.TryGetValue((typeof(TSource), typeof(TDestination)), out _))
            throw new InvalidOperationException($"Mapper<{nameof(TSource)}, {nameof(TDestination)}> is already registered.");
        
        _mappers[(typeof(TSource), typeof(TDestination))] = mapper;
    }

    public TDestination Map<TSource, TDestination>(TSource model)
    {
        if (_mappers.TryGetValue((typeof(TSource), typeof(TDestination)), out var mapper))
            return ((IMapper<TSource, TDestination>)mapper).Map(model);

        throw new InvalidOperationException($"Mapper<{nameof(TSource)}, {nameof(TDestination)}> is not registered.");
    }
}