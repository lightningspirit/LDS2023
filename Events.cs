namespace AnaliseImagens
{
    /**
     * Implementação de uma classe derivada de EventsArg que possui como atributo uma variável do tipo ColorPercentages
     * Esta classe permite que eventos que usem ResultsEventArgs<TInput> como par�metro possam passar os resultados aos delegados
     * que subscrevam o evento
     */
    public class ResultsEventArgs<TInput> : EventArgs
    {
        // Comando que executou resultado
        public string Command { get; }

        // Resultado do comando
        public TInput Results { get; }

        // Construtor
        public ResultsEventArgs(string command, TInput results)
        {
            Command = command;
            Results = results;
        }
    }
}
