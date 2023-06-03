using System.Reflection.Metadata;

namespace AnaliseImagens
{
    // Delegado do evento de lançar instruções
    public delegate void InstructionsHandler(ref List<string> commands);

    /**
     * O evento OnInstructionsNeeded pode ser respondido por delegados do tipo InstructionsHandler,
     * ou seja, pode ser respondido por qualquer método que tenha a mesma assignatura que o delegado definido.
     *
     * Quando o evento OnInstructionsNeeded é lançado, delegados que tenham subscrito a esse evento
     * vão receber uma lista de string por referência que podem modificar
     */
    interface IInstructionsDelegate
    {
        public event InstructionsHandler? OnInstructionsNeeded;
    }

    /**
     * Uma view que sabe apresentar instruções de uso, imprimir mensagens de erro
     * e apresentar resultados de comandos genéricos.
     */
    interface IView : IInstructionsDelegate
    {
        /**
         *  Quando este método é chamado, lança um evento OnInstructionsNeeded
         */
        public void ApresentarInstrucoes();

        /**
         * Mensagem de erro quando a operação não foi executada com sucesso e
         * término do programa com código ERROR_OPERATION_NOT_SUCCESSFUL
         */
        public void ImprimirMensagemErro(string message);

        /**
         * Este método subscreve a um evento que é lançado quando os resultados estão prontos.
         * Quando isso ocorre, recebe os resultados como argumento e imprime-os
         */
        public void ApresentarResultados(object sender, ResultsEventArgs<ICommandResult> e);
    }

    /**
     * Implementação concreta da View
     */
    class View : IView
    {
        // Evento lançado quando é necessário imprimir instruções
        public event InstructionsHandler? OnInstructionsNeeded;

        // Lista de comandos e seus Presenters
        // Como não se sabe o que é lançado, então usamos um object genérico.
        // Cada Presenter necessita de validar e fazer casting em runtime.
        private readonly Dictionary<string, Action<object>> commandPresenters;

        /**
         * O construtor instancia a lista de comandos
         */
        public View()
        {
            commandPresenters = new Dictionary<string, Action<object>>
            {
                { "analyze", PresentAnalyzeCmd }
            };
        }

        /**
         * Apresenta as instruções de uso
         */
        public void ApresentarInstrucoes()
        {
            List<string> availableCommands = new();

            if (OnInstructionsNeeded is not null)
            {
                OnInstructionsNeeded(ref availableCommands);
            }

            Console.WriteLine("Uso: AnaliseImagens <comando> <argumento>");
            Console.WriteLine("Comandos disponíveis:");
            foreach (string command in availableCommands)
            {
                Console.WriteLine($"- {command} \n Volte a tentar com o comando indicado seguido do endereço da imagem.");
            }
        }

        /**
         * Imprime uma mensagem de erro quando se dá uma excepção
         */
        public void ImprimirMensagemErro(string message)
        {
            Console.WriteLine(message);
        }

        /**
         * Handler do evento de resultados.
         * Recebe um ResultsEventArgs<ICommandResult>.
         * Tenta validar se o comando existe, caso contrário, lança uma excepção.
         */
        public void ApresentarResultados(object sender, ResultsEventArgs<ICommandResult> e)
        {
            if (commandPresenters.TryGetValue(e.Command, out Action<object>? command))
            {
                var genericExecutor = (dynamic)command;
                genericExecutor(e.Results);
            }
            else
            {
                throw new CommandNotValid(e.Command);
            }
        }


        /**
         * Presenter para o comando analyze
         */
        private void PresentAnalyzeCmd(object parameter)
        {
            if (parameter is ICommandResult<ColorPercentages> result)
            {
                ColorPercentages results = result.Value;

                Console.WriteLine("Resultados:");
                Console.WriteLine($"Red: {results.RedPercentage:F2}%");
                Console.WriteLine($"Green: {results.GreenPercentage:F2}%");
                Console.WriteLine($"Blue: {results.BluePercentage:F2}%");
            }
        }
    }
}