using Microsoft.AspNetCore.Mvc.Rendering;

namespace SupremoWeb.Shared
{
    public class CamposGerais
    {
        public List<SelectListItem> RetornaEstadosBrasileiro()
        {
            var ret = new List<SelectListItem>
            {
                new SelectListItem { Text = "Acre", Value = "AC" },
                new SelectListItem { Text = "Alagoas", Value = "AL" },
                new SelectListItem { Text = "Amapá", Value = "AP" },
                new SelectListItem { Text = "Amazonas", Value = "AM" },
                new SelectListItem { Text = "Bahia", Value = "BA" },
                new SelectListItem { Text = "Ceará", Value = "CE" },
                new SelectListItem { Text = "Distrito Federal", Value = "DF" },
                new SelectListItem { Text = "Espírito Santo", Value = "ES" },
                new SelectListItem { Text = "Goiás", Value = "GO" },
                new SelectListItem { Text = "Maranhão", Value = "MA" },
                new SelectListItem { Text = "Mato Grosso", Value = "MT" },
                new SelectListItem { Text = "Mato Grosso do Sul", Value = "MS" },
                new SelectListItem { Text = "Minas Gerais", Value = "MG" },
                new SelectListItem { Text = "Pará", Value = "PA" },
                new SelectListItem { Text = "Paraíba", Value = "PB" },
                new SelectListItem { Text = "Paraná", Value = "PR" },
                new SelectListItem { Text = "Pernambuco", Value = "PE" },
                new SelectListItem { Text = "Piauí", Value = "PI" },
                new SelectListItem { Text = "Rio de Janeiro", Value = "RJ" },
                new SelectListItem { Text = "Rio Grande do Norte", Value = "RN" },
                new SelectListItem { Text = "Rio Grande do Sul", Value = "RS" },
                new SelectListItem { Text = "Rondônia", Value = "RO" },
                new SelectListItem { Text = "Roraima", Value = "RR" },
                new SelectListItem { Text = "Santa Catarina", Value = "SC" },
                new SelectListItem { Text = "São Paulo", Value = "SP" },
                new SelectListItem { Text = "Sergipe", Value = "SE" },
                new SelectListItem { Text = "Tocantins", Value = "TO" }
            };

            return ret;
        }

        public List<SelectListItem> RetornaAtuacao()
        {
            var ret = new List<SelectListItem>
            {
                new SelectListItem { Text = "Revendedor", Value = "revendedor" },
                new SelectListItem { Text = "Consumidor", Value = "consumidor" }
            };

            return ret;
        }

        public List<SelectListItem> RetornaTipoPessoa()
        {
            var ret = new List<SelectListItem>
            {
                new SelectListItem { Text = "Pessoa Física", Value = "INDIVIDUAL" },
                new SelectListItem { Text = "Pessoa Jurídica", Value = "LEGAL_ENTITY" },
                new SelectListItem { Text = "Produtor Rural", Value = "RURAL_PRODUCER" }
            };

            return ret;
        }

        public List<SelectListItem> RetornaPersonStatus()
        {
            var ret = new List<SelectListItem>
            {
                new SelectListItem { Text = "Liberado para Venda", Value = "APPROVED" },
                new SelectListItem { Text = "Bloquear Crédito Excedido", Value = "BLOCK_CREDIT_BEYOND_LIMIT" },
                new SelectListItem { Text = "Bloqueado para Venda", Value = "BLOCKED_SALES" },
                new SelectListItem { Text = "Inativo", Value = "INACTIVE" },
                new SelectListItem { Text = "Liberado venda abaixo do permitido", Value = "APPROVED_SALES_BELOW_PERMITTED" },
                new SelectListItem { Text = "Baixado", Value = "COMPANY_CLOSED" },
                new SelectListItem { Text = "Aguardando Análise", Value = "IN_REVIEW" },
                new SelectListItem { Text = "Solicitar Documentação", Value = "DOCUMENTS_REQUIRED" },
                new SelectListItem { Text = "Atualizar Cadastro", Value = "UPDATE_INFORMATIONS_REQUIRED" }
            };

            return ret;

        }

        public List<SelectListItem> RetornaPais()
        {
            var ret = new List<SelectListItem>
            {
                new SelectListItem { Text = "Brasil", Value = "BR" },
                new SelectListItem { Text = "USA", Value = "US" }
            };

            return ret;
        }
    }
}
