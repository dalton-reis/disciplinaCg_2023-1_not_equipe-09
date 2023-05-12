#define CG_Debug

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace gcgcg
{
    internal class Spline : Objeto
    {
        private double numeroArestas;

        public Spline(Objeto paiRef, double numeroArestas) : base(paiRef)
        {
            PrimitivaTipo = PrimitiveType.LineStrip;
            PrimitivaTamanho = 1;

            this.numeroArestas = numeroArestas;

            List<Ponto4D> pontos = paiRef.Pontos();

            for (double i = 0; i <= this.numeroArestas; i++)
            {
                double interpolacao = (1 / this.numeroArestas) * i;

                base.PontosAdicionar(CalcularPontoInterpolacao(pontos, interpolacao));
            }

            Atualizar();
        }

        private Ponto4D CalcularPontoInterpolacao(List<Ponto4D> pontos, double interpolacao)
        {
            if (interpolacao < 0 || interpolacao > 1)
                throw new ArgumentException("valor precisa estar entre 0 e 1", nameof(interpolacao));

            if (pontos.Count == 0)
                throw new ArgumentException("não contém pontos", nameof(pontos));

            if (pontos.Count == 1)
                return pontos[0];

            // first to last - 1
            var p1 = CalcularPontoInterpolacao(pontos.GetRange(0, pontos.Count - 1), interpolacao);
            // second to last
            var p2 = CalcularPontoInterpolacao(pontos.GetRange(1, pontos.Count - 1), interpolacao);
            var nt = 1 - interpolacao;

            return new Ponto4D(nt * p1.X + interpolacao * p2.X, nt * p1.Y + interpolacao * p2.Y);
        }

        public void AlterarNumeroArestas(double numeroArestas)
        {
            if (this.numeroArestas > 1 || numeroArestas > 0)
                this.numeroArestas += numeroArestas;
        }
        public void Atualizar()
        {
            base.ObjetoAtualizar();
        }

        public override void ObjetoAtualizar()
        {   
            List<Ponto4D> pontos = this.Pontos();
            while (pontos.Count > 0)
            {
                this.PontosRemoverUltimo();
            }

            List<Ponto4D> pontosPai = paiRef.Pontos();

            for (double i = 0; i <= this.numeroArestas; i++)
            {
                double interpolacao = (1 / this.numeroArestas) * i;

                base.PontosAdicionar(CalcularPontoInterpolacao(pontosPai, interpolacao));
            }

            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Spline _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
#endif
    }
}
