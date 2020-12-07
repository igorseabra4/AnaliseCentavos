using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace AnaliseCentavos
{
    class Aluno
    {
        private int idAluno;
        private List<EntradaDeNota> notas;

        public int QtdCentavos => notas.Sum(n => n.qtdCentavos);

        public Aluno(int idAluno)
        {
            this.idAluno = idAluno; 
            notas = new List<EntradaDeNota>();
        }

        public void AdicionaEntradaDeNota(string data, int qtdCentavos, string descricao)
        {
            notas.Add(new EntradaDeNota(data, qtdCentavos, descricao));
        }

        public override string ToString()
        {
            return $"aluno: {idAluno} centavos: {QtdCentavos}";
        }
    }

    class EntradaDeNota
    {
        public string data;
        public int qtdCentavos;
        public string descricao;

        public EntradaDeNota(string data, int qtdCentavos, string descricao)
        {
            this.data = data;
            this.qtdCentavos = qtdCentavos;
            this.descricao = descricao;
        }
    }

    class AnaliseCentavos
    {
        static string GetAnonFromURL()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://lad.dsc.ufcg.edu.br/loac/uploads/OAC/anon.txt");
            request.AutomaticDecompression = DecompressionMethods.GZip;
            using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using StreamReader reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd();
        }

        static string[] SplitIntoLines(string input) => input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        static void Main(string[] args)
        {
            string lines;

            string fileName = "anon.txt";

            if (File.Exists(fileName))
            {
                Console.WriteLine("Lendo anon.txt...");
                lines = File.ReadAllText(fileName);
            }
            else
                try
                {
                    Console.WriteLine("anon.txt local não encontrado. Buscando em rede...");
                    lines = GetAnonFromURL();
                    Console.WriteLine("Sucesso.");
                }
                catch
                {
                    Console.WriteLine("Erro.");
                    Console.ReadKey();
                    return;
                }

            Console.WriteLine();

            var alunos = new Dictionary<int, Aluno> ();

            foreach (var l in SplitIntoLines(lines))
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
            Console.ReadKey();
        }
    }
}
