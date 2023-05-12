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
    internal class Retangulo : Objeto
    {
        public Retangulo(Objeto paiRef, Ponto4D ptoInfEsq, Ponto4D ptoSupDir) : base(paiRef)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 10;

            base.PontosAdicionar(new Ponto4D(ptoInfEsq.X, ptoSupDir.Y));
            base.PontosAdicionar(ptoInfEsq);
            base.PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoInfEsq.Y));
            base.PontosAdicionar(ptoSupDir);

            Atualizar();
        }

        public void Atualizar()
        {
            base.ObjetoAtualizar();
        }

        public override void ObjetoAtualizar()
        {
            if (paiRef != null)
            {
                List<Ponto4D> pontos = this.Pontos();
                while (pontos.Count > 0)
                {
                    this.PontosRemoverUltimo();
                }

                List<Ponto4D> pontosPai = paiRef.Pontos();

                for (int i = 0; i < pontosPai.Count; i++)
                {
                    base.PontosAdicionar(pontosPai[i]);
                }
            }

            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Retangulo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
#endif

    }
}
