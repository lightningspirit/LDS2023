namespace AnaliseImagens
{

    //Definição de códigos customizados de término de programa 
    public static class ExitCodes
    {
        public const int SUCCESS = 0;
        public const int ERROR_OPERATION_NOT_SUCCESSFUL = 1;
    }

    class AnaliseImagens
    {
 
        //Iniciar o programa - ponto de entrada principal
        static void Main(string[] args)
        {
            Controller controller = new();
            controller.IniciarPrograma();

        }
    }
}
