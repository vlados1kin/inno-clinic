namespace Application.Contracts.Mapper;

public interface IMapper<in TSource, out TDestination>
{
    TDestination Map(TSource model);
}