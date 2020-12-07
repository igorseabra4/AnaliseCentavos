using System.Collections.Generic;

namespace AnaliseCentavos
{
    class Aluno
    {
        private int idAluno;
        private List<Nota> notas;

        public int QtdCentavos => notas.Sum(n => n.qtdCentavos);

        public Aluno(int idAluno)
        {
            this.idAluno = idAluno; 
            notas = new List<Nota>();
        }

        public void AdicionaEntradaDeNota(string data, int qtdCentavos, string descricao)
        {
            notas.Add(new Nota(data, qtdCentavos, descricao));
        }

        public override string ToString()
        {
            return $"aluno: {idAluno} centavos: {QtdCentavos}";
        }
    }
}
