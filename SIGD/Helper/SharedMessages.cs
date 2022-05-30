using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGD.Helper
{
    public class SharedMessages
    {
        public const string ERROR_SENDING_EMAIL = "Erro ao enviar o email com senha de primeiro acesso, por favor tente novamente.";
        public const string ERROR_SENDING_EMAIL_CHANGE_PASSWORD = "Erro ao enviar o email sobre alteração de senha.";
        public const string ERROR_PASSWORD_NOT_MATCH = "Usuário/email ou senha estão errados, por favor tente novamente.";
        public const string CHANGE_FIRST_ACCESS_PASSWORD = "Precisa mudar a senha de primeiro acesso.";
        public const string ERROR_FIRST_ACCESS_PASSWORD = "Senha de primeiro acesso errada.";
        public const string ERROR_SAVING_DATA = "Erro ao alterar senha, por favor tente novamente.";
    }
}
