namespace Application.Contracts.Exceptions;

public sealed class UnauthorizedException(string message) : Exception(message);