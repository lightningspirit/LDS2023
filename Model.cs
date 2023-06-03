using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing;

namespace AnaliseImagens
{
    // Delegados dos eventos
    public delegate void AnalysisResultsHandler(object sender, ResultsEventArgs<ICommandResult> e);
    public delegate void CommandValidator(string[] args);
    public delegate ICommandResult<T> CommandExecutor<T>(string[] args);

    /**
     * Usado para retornar resultados de um comando para a View
     */
    public interface ICommandResult {}
    public interface ICommandResult<TValue> : ICommandResult
    {
        TValue Value { get; }
    }

    /**
     * O evento OnResultsAvailable pode ser respondido por delegados do tipo AnalysisResultsHandler,
     * ou seja, pode ser respondido por qualquer método que tenha a mesma assignatura que o delegado definido. 
     * Quando o evento OnResultsAvailable é lançado, delegados que tenham subscrito a esse evento
     * vão receber e terão acesso aos resultados.
     */
    interface IResultsDelegate
    {
        public event AnalysisResultsHandler? OnResultsAvailable;
    }

    /**
     * Um contentor de comandos que sabe listar, validar e executar um comando
     */
    interface IModel : IResultsDelegate
    {
        public void ListarComandos(ref List<string> commands);
        public void ValidarComando(string command, string[] args);
        public void ExecutarComando(string command, string[] args);
    }

    /**
     * Implementação do resultado de um comando
     */
    class CommandResult<TValue> : ICommandResult<TValue>
    {
        private readonly TValue value;
        public TValue Value => value;

        public CommandResult(TValue input)
        {
            value = input;
        }
    }

    /**
     * Resultado usado no comando analyze
     */
    public class ColorPercentages
    {
        public float RedPercentage { get; set; }
        public float GreenPercentage { get; set; }
        public float BluePercentage { get; set; }
    }

    /**
     * A implementação concreta do IModel
     */
    class Model : IModel
    {
        // Lista de comandos da classe
        private readonly List<string> listCmds;
        private readonly Dictionary<string, CommandValidator> commandValidators;
        private readonly Dictionary<string, Delegate> commandExecutors;

        // Evento lançado quando um resultado de um comando está disponível
        public event AnalysisResultsHandler? OnResultsAvailable;

        /**
         * Construtor instancia os atributos
         */
        public Model()
        {
            listCmds = new List<string> { "analyze" };

            // Os dicionários são inicializados associando a cada comando
            // uma função que irá validar ou executar o comando
            commandValidators = new Dictionary<string, CommandValidator>
            {
                { "analyze", ValidateAnalyzeCmd }
            };

            commandExecutors = new Dictionary<string, Delegate>
            {
                { "analyze", ExecuteAnalyzeCmd }
            };
        }


        /**
         * Implementação do método que lança o evento 'OnResultsAvailable'
         */
        protected virtual void RaiseResultsAvailable(string command, ICommandResult results)
        {
            OnResultsAvailable?.Invoke(this, new ResultsEventArgs<ICommandResult>(command, results));
        }


        /**
         * Retorna a lista de comandos disponíveis  
         */
        public void ListarComandos(ref List<string> commands) 
        {
            commands = listCmds;
        }

        /**
         * Invoca o validador do comando.
         * Exceciona se o comando não for válido, voltando o controlo para o Controller.
         */
        public void ValidarComando(string command, string[] args)
        {
            if (commandValidators.TryGetValue(command, out CommandValidator? validator))
            {
                validator(args);
            }
            else
            {
                throw new CommandNotValid(command);
            }
        }

        /**
         * Executa comando introduzido pelo utilizador. 
         * Se comando for executado com sucesso, o evento 'OnResultsAvailable' é lançado.
         *
         * Um método da View subscreve este método.
         *
         * Se o comando não for executado com sucesso, é lançada uma excepção e o controlo
         * retorna ao Controller que irá lidar com essa excepção
         */
        public void ExecutarComando(string command, string[] args)
        {
            if (commandExecutors.TryGetValue(command, out Delegate? executor))
            {
                // Usa retorno dinâmico que é avaliado em runtime
                // Isto é necessário para que se possa manter vários
                // comandos que retornam resultados diferentes.
                var genericExecutor = (dynamic)executor;
                dynamic results = genericExecutor(args);

                // Quando os resultados estão prontos, é lançado o evento
                RaiseResultsAvailable(command, results);
            }
            else
            {
                throw new OperationError(command);
            }
        }

        /**
         * Valida o comando analyze.
         * Caso não haja argumentos ou o ficheiro providênciado
         * não exista, então lança exceção.
         */
        private void ValidateAnalyzeCmd (string[] args)
        {
            if (args.Length == 0) {
                throw new EmptyCommandArguments();
            }

            if (!File.Exists(args[0])) {
                throw new InvalidPath(args[0]);
            }
        }


        /**
         * Função que calcula as percentagens de cada cor e retornar
         * resultado como objecto do tipo ColorPercentages
         */
        private ICommandResult<ColorPercentages> ExecuteAnalyzeCmd(string[] args)
        {
            Bitmap bitmap = new(args[0]);

            float totalPixels = bitmap.Height * bitmap.Width;

            // Cria um filtro de cores vermelhas
            ColorFiltering filterRed = new()
            {
                Red = new IntRange(100, 255),
                Green = new IntRange(0, 100),
                Blue = new IntRange(0, 100)
            };

            // Cria um filtro de cores verdes
            ColorFiltering filterGreen = new()
            {
                Red = new IntRange(0, 100),
                Green = new IntRange(100, 255),
                Blue = new IntRange(0, 100)
            };

            // Cria um filtro de cores azuis
            ColorFiltering filterBlue = new()
            {
                Red = new IntRange(0, 100),
                Green = new IntRange(0, 100),
                Blue = new IntRange(100, 255)
            };

            // Aplica o filtro Red na imagem
            Bitmap bitmapRed = filterRed.Apply((Bitmap)bitmap);

            // Aplica o filtro Blue na imagem
            Bitmap bitmapBlue = filterBlue.Apply((Bitmap)bitmap);

            // Aplica o filtro Green na imagem
            Bitmap bitmapGreen = filterGreen.Apply((Bitmap)bitmap);

            // Cria um objeto ImageStatistics
            ImageStatistics bitmapRedPixeis = new(bitmapRed);
            ImageStatistics bitmapBluePixeis = new(bitmapBlue);
            ImageStatistics bitmapGreenPixeis = new(bitmapGreen);

            // Obtém as estatísticas da imagem

            float redPixels = bitmapRedPixeis.PixelsCountWithoutBlack;
            float greenPixels = bitmapGreenPixeis.PixelsCountWithoutBlack;
            float bluePixels = bitmapBluePixeis.PixelsCountWithoutBlack;

            ColorPercentages results = new()
            {
                RedPercentage = (redPixels / totalPixels) * 100,
                GreenPercentage = (greenPixels / totalPixels) * 100,
                BluePercentage = (bluePixels / totalPixels) * 100,
            };

            return new CommandResult<ColorPercentages>(results);
        }
    }
}