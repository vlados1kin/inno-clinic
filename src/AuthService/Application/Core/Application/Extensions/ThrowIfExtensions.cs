using Application.Contracts;
using Domain.Entities.Base;

namespace Application.Extensions;

public static class ThrowIfExtensions
{
    public static T ThrowIfNotFound<T>(this T? entity, string message) where T : IEntity
    {
        if (entity is null)
            throw new NotFoundException(message);

        return entity;
    }
}