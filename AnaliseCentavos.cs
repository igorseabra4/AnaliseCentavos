using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace AnaliseCentavos
{
    class AnaliseCentavos
    {
        static void Main(string[] args)
        {
            CalculateAndPrint(ParseData(GetLines(args.Length > 1 ? args[1] : "anon.txt")));
            Console.ReadKey();
        }

        static string[] GetLines(string fileName)
        {
            string lines;

            if (File.Exists(fileName))
            {
                Console.WriteLine("Lendo " + fileName);
                lines = File.ReadAllText(fileName);
            }
            else
            {
                Console.WriteLine(fileName + " local não encontrado. Buscando em rede...");
                lines = GetAnonFromURL();
                Console.WriteLine("Sucesso.");
            }

            Console.WriteLine();

            return lines.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        }

        static string GetAnonFromURL()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://lad.dsc.ufcg.edu.br/loac/uploads/OAC/anon.txt");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        static Dictionary<int, Aluno> ParseData(string[] lines)
        {
            Dictionary<int, Aluno> alunos = new Dictionary<int, Aluno>();

            foreach (var l in lines)
            {
                if (string.IsNullOrEmpty(l))
                    continue;

                var split = l.Split();
                int comecaDescricao = split[0].Length + split[1].Length + split[2].Length + 3;

                int idAluno = Convert.ToInt32(split[0]);

                if (!alunos.ContainsKey(idAluno))
                    alunos[idAluno] = new Aluno(idAluno);

                alunos[idAluno].AdicionaEntradaDeNota(split[1], Convert.ToInt32(split[2]), l.Substring(comecaDescricao));
            }

            return alunos;
        }

        static void CalculateAndPrint(Dictionary<int, Aluno> alunos)
        {
            Aluno maiorAluno = null;
            int somaTotal = 0;
            int qtdAprovados = 0;
            int qtdFinal = 0;
            int qtdReprovados = 0;

            foreach (var aluno in alunos.Values)
            {
                somaTotal += aluno.QtdCentavos;

                if (maiorAluno == null || maiorAluno.QtdCentavos < aluno.QtdCentavos)
                    maiorAluno = aluno;

                if (aluno.QtdCentavos >= 700)
                    qtdAprovados++;
                if (aluno.QtdCentavos >= 500 && aluno.QtdCentavos < 700)
                    qtdFinal++;
                if (aluno.QtdCentavos < 500)
                    qtdReprovados++;

                Console.WriteLine(aluno);
            }

            Console.WriteLine();

            float media = 1.0f * somaTotal / alunos.Count;

            Console.WriteLine("maior qtd centavos: " + maiorAluno);
            Console.WriteLine("media centavos turma toda: " + media);
            Console.WriteLine("qtd aprovados : " + qtdAprovados);
            Console.WriteLine("qtd final     : " + qtdFinal);
            Console.WriteLine("qtd reprovados: " + qtdReprovados);
        }
    }
}
