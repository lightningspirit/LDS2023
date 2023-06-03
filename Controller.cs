namespace AnaliseImagens
{
    class Controller
    {
        // Dependências da classe
        private readonly IView view;
        private readonly IModel model;

        /**
         * Construtor recebe como parâmetros as dependências instanciadas
         */
        public Controller(IModel imodel, IView iview)
        {
            model = imodel;
            view = iview;

            // Método ListarComandos() do Model subscreve ao evento PrecisoDasInstrucoes da view, sem que nenhum tenha conhecimento disso
            view.OnInstructionsNeeded += model.ListarComandos;

            // Método ApresentarResultados() da View subscreve ao evento OnResultsAvailable() do Model 
            model.OnResultsAvailable += view.ApresentarResultados;
        }

        /**
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

        /**
         * Model valida e executa comando.
         * Podem ocorrer excepções que são geridas pelo método 'HandleException()' do Controller.
         * Caso não ocorram excepções, a view apresenta os resultados e o controller termina.
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

        /**
         * Faz a gestão de excepções que possam ser lançadas pelo modelo.
         * View imprime mensagem de erro e execução do programa termina.
         */
        private void HandleException(Exception excp)
        {
            view.ImprimirMensagemErro("Erro: " + excp.Message);
            Environment.Exit(ExitCodes.ERROR_OPERATION_NOT_SUCCESSFUL);
        }
    }
}


