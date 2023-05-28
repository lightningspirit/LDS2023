using System;

namespace AnaliseImagens
{

    /*
     * Implementação de uma classe derivada de EventsArg que possui como atributo uma variável do tipo ColorPercentages
     * Esta classe permite que eventos que usem AnalysisResultsEventArgs como parâmetro possam passar os resultados aos delegados 
     * que subscrevam o evento
     */
    public class AnalysisResultsEventArgs : EventArgs
    {
        //Atributo
        public ColorPercentages Results { get; set; }

        //Construtor
        public AnalysisResultsEventArgs(ColorPercentages results)
        {
            Results = results;
        }
    }
}