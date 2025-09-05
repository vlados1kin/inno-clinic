namespace Application.Contracts;

public sealed class NotFoundException(string message) : Exception(message);

public sealed class UnauthorizedException(string message) : Exception(message);