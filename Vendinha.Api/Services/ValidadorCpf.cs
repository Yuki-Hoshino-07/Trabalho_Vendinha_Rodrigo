namespace Vendinha.Api.Services;

public static class ValidadorCpf
{
    public static string ApenasNumeros(string cpf) => new(cpf.Where(char.IsDigit).ToArray());

    public static bool EhValido(string cpf)
    {
        var numeros = ApenasNumeros(cpf);

        if (numeros.Length != 11 || numeros.Distinct().Count() == 1)
            return false;

        var primeiroDigito = CalcularDigito(numeros[..9], 10);
        var segundoDigito = CalcularDigito(numeros[..10], 11);

        return numeros[9] - '0' == primeiroDigito && numeros[10] - '0' == segundoDigito;
    }

    private static int CalcularDigito(string parte, int pesoInicial)
    {
        var soma = 0;
        for (var i = 0; i < parte.Length; i++)
            soma += (parte[i] - '0') * (pesoInicial - i);

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
