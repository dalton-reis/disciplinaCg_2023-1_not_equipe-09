#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public Circulo(Objeto paiRef, double raio, int numeroPontos) : base(paiRef)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 5;

            Ponto4D pto = new Ponto4D();
            for (int i = 0; i < 360; i += (360 / numeroPontos))
            {
                pto = Matematica.GerarPtosCirculo(i, raio);
                PontosAdicionar(pto);
            }

            Atualizar();
        }

        public void Atualizar()
        {
            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Circulo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
#endif
    }
}
