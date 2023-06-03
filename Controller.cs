namespace AnaliseImagens
{
    class Controller
    {
        //Atributos da classe 
        private readonly View view;
        private readonly Model model; 

        //Construtor sem parâmetros
        public Controller()
        {
            view = new View(); 
            model = new Model();
            
            //Método ListarComandos() do Model subscreve ao evento PrecisoDasInstrucoes da view, sem que nenhum tenha conhecimento disso
            view.OnInstructionsNeeded += model.ListarComandos;

            //Método ApresentarResultados() da View subscreve ao evento OnResultsAvailable() do Model 
            model.OnResultsAvailable += view.ApresentarResultados;
        }

        /*
         * View imprime as instruções com os comandos disponíveis. 
         * View imprime um prompt a indicar ao utilizador para introduzir o comando.
         * Controller faz a leitura do input do utilizador e passa o comando para o método auxiliar interno à classe 'LerComando()'
        */
        public void IniciarPrograma(string[] args)
        {
            if (args.Length > 0) {
                // args[0] contem o comando e os outros argumentos são passados no segundo parâmetro
                LerComando(args[0], args.Skip(1).ToArray());
            } else {
                view.ApresentarInstrucoes();
            }
        }

        /*
         *   *Model executa comando. 
         *   Podem ocorrer excepções que são geridas pelo método 'HandleException()' do Controller. Caso não ocorram excepções,
         *   a view apresenta os resultados. 
        */
        private void LerComando(string command, string[] args)
        {
            try
            {
                // Valida o comando e os argumentos
                model.ValidarComando(command, args);
                // Executa comando passando os argumentos
                model.ExecutarComando(command, args);
                // Sai do programa com sucesso
                Environment.Exit(ExitCodes.SUCCESS);
            }
            catch (Exception excp)
            {
                HandleException(excp);
            }
        }

        /*
        * Faz a gestão de excepções que possam ser lançadas pelo modelo. 
        * Para as excepções NoCommandFound, CommandNotValid ou InvalidPath:
        *  - View imprime um prompt com uma mensagem de erro específica de cada excepção e solicita novamente ao utilizador para inserir o comando
        *  - Controller faz a leitura do comando inserido
        *  
        * Para a excepção OperationError:
        *  - View imprime mensagem de erro e execução do programa termina
        */

        private static void HandleException(Exception excp)
        {
            View.ImprimirMensagemErro("Erro: " + excp.Message);
            Environment.Exit(ExitCodes.ERROR_OPERATION_NOT_SUCCESSFUL);
        }
    }
}


