﻿using System;
using System.Collections.Generic;
using System.IO;
using static AnaliseImagens.Model;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing;

namespace AnaliseImagens
{

    //Definição da classe ColorPercentages
    public class ColorPercentages
    {
        public float RedPercentage { get; set; }
        public float GreenPercentage { get; set; }
        public float BluePercentage { get; set; }
    }
   
    class Model
    {
        //Atributos da classe
        private readonly List<string> listCmds;
        public delegate void CommandValidator(string[] args);
        public delegate ColorPercentages CommandExecutor(string[] args);
        private readonly Dictionary<string, CommandValidator> commandValidators; 
        private readonly Dictionary<string, CommandExecutor> commandExecutors;

        /*
         * O evento OnResultsAvailable pode ser respondido por delegados do tipo AnalysisResultsHandler, ou seja, pode ser 
         * respondido por qualquer método que tenha a mesma assignatura que o delegado definido. 
         * Quando o evento OnResultsAvailable é lançado, delegados que tenham subscrito a esse evento vão receber e terão acesso
         * aos resultados
         */
        public delegate void AnalysisResultsHandler(object sender, AnalysisResultsEventArgs e);
        public event AnalysisResultsHandler OnResultsAvailable;

        //Construtor
        public Model()
        {
            listCmds = new List<string> { "analyze" };

            //Os dicionários são inicializados associando a cada comando uma função que irá validar ou executar o comando
            commandValidators = new Dictionary<string, CommandValidator>
            {
                { "analyze", ValidateAnalyzeCmd }
            };

            commandExecutors = new Dictionary<string, CommandExecutor>
            {
                { "analyze", ExecuteAnalyzeCmd }
            };
        }


        /*
         * Implementação do método que lança o evento 'OnResultsAvailable'
        */
        protected virtual void RaiseResultsAvailable(ColorPercentages results)
        {
            OnResultsAvailable?.Invoke(this, new AnalysisResultsEventArgs(results));
        }


        /*
         * Retorna a lista de comandos disponíveis  
        */
        public void ListarComandos(ref List<string> commands) 
        {
            commands = listCmds;
        }


        public void ValidarComando(string command, string[] args)
        {
            if (commandValidators.TryGetValue(command, out CommandValidator validator))
            {
                validator(args);
            }
            else
            {
                throw new CommandNotValid(command);
            }
        }

        /*
         * Executa comando introduzido pelo utilizador. 
         * Se comando for executado com sucesso, o evento 'OnResultsAvailable' é lançado. Um método da View subscreve a este método
         * Se o comando não for executado com sucesso, é lançada uma excepção e o controlo retorna ao Controller que irá lidar com essa
         * excepção
         */
        public void ExecutarComando(string command, string[] args)
        {
            if (commandExecutors.TryGetValue(command, out CommandExecutor executor))
            {
                //Quando os resultados estão prontos, é lançado o evento
                ColorPercentages results = executor(args);
                RaiseResultsAvailable(results);
            }
            else
            {
                throw new OperationError(command);
            }
            
        }

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
        * Função que calcula as percentagens de cada cor e retornar resultado como objecto do tipo ColorPercentages
        */
        private ColorPercentages ExecuteAnalyzeCmd(string[] args)
        {
            Bitmap bitmap = new(args[0]);

            float totalPixels = bitmap.Height * bitmap.Width;

            Console.WriteLine("Imagem com "+totalPixels+ " Pixels");

            // Cria um filtro de cores vermelhas
            ColorFiltering filterRed = new()
            {
                Red = new IntRange(150, 255),
                Green = new IntRange(0, 100),
                Blue = new IntRange(0, 100)
            };

            // Cria um filtro de cores verdes
            ColorFiltering filterGreen = new()
            {
                Red = new IntRange(0, 80),
                Green = new IntRange(100, 200),
                Blue = new IntRange(0, 100)
            };

            // Cria um filtro de cores azuis
            ColorFiltering filterBlue = new()
            {
                Red = new IntRange(0, 80),
                Green = new IntRange(0, 150),
                Blue = new IntRange(150, 255)
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

            Console.WriteLine("Imagem com " + redPixels + " REDPixels");
            Console.WriteLine("Imagem com " + greenPixels + " GREENPixels");
            Console.WriteLine("Imagem com " + bluePixels + " BLUEPixels");


            ColorPercentages results = new()
            {
                RedPercentage = (redPixels/totalPixels)*100,
                GreenPercentage = (greenPixels/totalPixels)*100,
                BluePercentage = (bluePixels/totalPixels)*100,
            };

            return results;
        }
    }
}