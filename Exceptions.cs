using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnaliseImagens
{
    //Definição das diferentes excepções que podem ser lançadas


    //Quando comando não é válido
    class CommandNotValid : Exception
    {
        public CommandNotValid(string command) : base($"Comando {command} não é válido.") { }
    }

    //Quando não existe nenhuma imagem no path indicado ou este não é válido
    class InvalidPath : Exception
    {
        public InvalidPath(string path) : base($"Não foi encontrada nenhuma imagem em {path}.") { }
    }

    //Quando não foi possível executar a operação com sucesso
    class OperationError : Exception
    {
        public OperationError(string operation) : base($"Não foi posível executar a operação {operation} com sucesso.") { }
    }
}
