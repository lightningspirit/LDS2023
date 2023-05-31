namespace AnaliseImagens
{
    /*
     * Implementa��o de uma classe derivada de EventsArg que possui como atributo uma vari�vel do tipo ColorPercentages
     * Esta classe permite que eventos que usem ResultsEventArgs<TInput> como par�metro possam passar os resultados aos delegados
     * que subscrevam o evento
     */
    public class ResultsEventArgs<TInput> : EventArgs
    {
        //Atributo
        public TInput Results { get; set; }

        //Construtor
        public ResultsEventArgs(TInput results)
        {
            Results = results;
        }
    }
}