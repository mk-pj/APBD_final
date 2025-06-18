namespace income_verifier.Middlewares;

public class NotFoundException(string msg) : Exception(msg);