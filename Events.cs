using System;

namespace AnaliseImagens
{

    /*
     * Implementa��o de uma classe derivada de EventsArg que possui como atributo uma vari�vel do tipo ColorPercentages
     * Esta classe permite que eventos que usem AnalysisResultsEventArgs como par�metro possam passar os resultados aos delegados 
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