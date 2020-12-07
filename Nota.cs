namespace AnaliseCentavos
{
    class Nota
    {
        public string data;
        public int qtdCentavos;
        public string descricao;

        public Nota(string data, int qtdCentavos, string descricao)
        {
            this.data = data;
            this.qtdCentavos = qtdCentavos;
            this.descricao = descricao;
        }
    }
}
