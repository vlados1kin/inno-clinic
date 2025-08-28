namespace Application.Contracts.Exceptions;

public sealed class NotFoundException(string message) : Exception(message);