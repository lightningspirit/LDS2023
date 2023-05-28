using AnaliseImagens;
using System.Data;
using System.IO;

namespace AnaliseImagens
{
    class Controller
    {
        //Atributos da classe 
        private readonly View view;
        private readonly Model model; 

        //Construtor sem parâmetros
        public Controller() {
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
        public void IniciarPrograma()
        {
      
            view.ApresentarInstrucoes();
            View.ImprimirPromptInserirInput("");
            string command;
            do
            {
              command = Console.ReadLine();

                //conhunto de imagens para teste dos resultados
                //command = "C:\\Users\\luisn\\source\\repos\\lsantos1970\\LDS2023_C1\\Imagens_teste\\teste.png";
                //command = "C:\\Users\\luisn\\source\\repos\\lsantos1970\\LDS2023_C1\\Imagens_teste\\teste_red.png";
                //command = "C:\\Users\\luisn\\source\\repos\\lsantos1970\\LDS2023_C1\\Imagens_teste\\teste_green.png";
                //command = "C:\\Users\\luisn\\source\\repos\\lsantos1970\\LDS2023_C1\\Imagens_teste\\teste_blue.png";
                
                LerComando(command);

            } while (!command.Equals("E"));
        }

        /*
         *   Se comando for o comando de saída, View imprime mensagem de despedida e programa termina
         *   Caso contrário, Model executa comando. 
         *   Podem ocorrer excepções que são geridas pelo método 'HandleException()' do Controller. Caso não ocorram excepções,
         *   a view apresenta os resultados. 
        */
        private void LerComando(string command)
        {

            if (command.Equals("E"))
            {
                View.ImprimirMensagemDespedida();
                return;
            }

            try
            {
                

                string path = command, cmd = "analyze";
                //model.ValidarComando(command);//, ref cmd, ref path);

                model.ExecutarComando(cmd, path);
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

    private void HandleException(Exception excp)
        {
            if (excp is CommandNotValid || excp is InvalidPath)
            {
                View.ImprimirPromptInserirInput(excp.Message + "\n");
                string command = Console.ReadLine();
                LerComando(command);
            }
            else if (excp is OperationError)
            {
                View.ImprimirMensagemErro(excp.Message);
            }


        }


     
    }
}


