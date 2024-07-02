using FluentValidation;
using SupremoWeb.Models;

namespace SupremoWeb.Shared
{
    public class ClienteModelValidator : AbstractValidator<ClienteTotalModel>
    {
        public ClienteModelValidator()
        {
            RuleFor(x => x.companyName)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.tradingName)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.street)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.houseNumber)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.city)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.neighborhood)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.state)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.postalCode)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.cpf)
                .NotEmpty().WithMessage("Campo Obrigatório")
                .Must(IsValidCPF).WithMessage("CPF inválido")
                .When(x => x.personType == "PF");

            RuleFor(x => x.cnpj)
                .NotEmpty().WithMessage("Campo Obrigatório")
                .Must(IsValidCNPJ).WithMessage("CNPJ inválido")
                .When(x => x.personType == "PJ");

            RuleFor(x => x.identificationCard)
                .NotEmpty().WithMessage("Campo Obrigatório");

            RuleFor(x => x.email)
                .EmailAddress().WithMessage("Email inválido");
        }

        private bool IsValidCPF(string cpf)
        {
            if (!string.IsNullOrEmpty(cpf))
            {
                cpf = cpf.Replace(".", "").Replace("-", "");

                if (cpf.Length != 11)
                    return false;

                var invalidCpfs = new HashSet<string>
        {
            "00000000000",
            "11111111111",
            "22222222222",
            "33333333333",
            "44444444444",
            "55555555555",
            "66666666666",
            "77777777777",
            "88888888888",
            "99999999999"
        };

                if (invalidCpfs.Contains(cpf))
                    return false;

                for (var t = 9; t < 11; t++)
                {
                    var d = 0;
                    for (var c = 0; c < t; c++)
                        d += int.Parse(cpf[c].ToString()) * ((t + 1) - c);

                    d = (d * 10) % 11;
                    if (d == 10 || d == 11)
                        d = 0;

                    if (d != int.Parse(cpf[t].ToString()))
                        return false;
                }
            }

            return true;
        }

        private bool IsValidCNPJ(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
                return false;

            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }
    }
}
