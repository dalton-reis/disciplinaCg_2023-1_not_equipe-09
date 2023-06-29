#define CG_OpenGL
#define CG_Debug
// #define CG_DirectX

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace gcgcg
{
    internal class Objeto  // TODO: deveria ser uma class abstract ..??
    {
        // Objeto
        // #3
        private char rotulo;
        // #3
        public char Rotulo { get => rotulo; set => rotulo = value; }
        protected Objeto paiRef;
        private List<Objeto> objetosLista = new List<Objeto>();
        private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
        public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
        private float primitivaTamanho = 1;
        public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
        private Shader _shaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
        public Shader shaderCor { set => _shaderObjeto = value; }

        // Vértices do objeto TODO: o objeto mundo deveria ter estes atributos abaixo?
        protected List<Ponto4D> pontosLista = new List<Ponto4D>();
        private int _vertexBufferObject;
        private int _vertexArrayObject;

        // BBox do objeto
        private BBox bBox = new BBox();
        public BBox Bbox()  // TODO: readonly
        {
            return bBox;
        }

        // Transformações do objeto
        private Transformacao4D matriz = new Transformacao4D();

        /// Matrizes temporarias que sempre sao inicializadas com matriz Identidade entao podem ser "static".
        private static Transformacao4D matrizTmpTranslacao = new Transformacao4D();
        private static Transformacao4D matrizTmpTranslacaoInversa = new Transformacao4D();
        private static Transformacao4D matrizTmpEscala = new Transformacao4D();
        private static Transformacao4D matrizTmpRotacao = new Transformacao4D();
        private static Transformacao4D matrizGlobal = new Transformacao4D();
        private char eixoRotacao = 'z';
        public void TrocaEixoRotacao(char eixo) => eixoRotacao = eixo;


        public Objeto(Objeto paiRef, ref char _rotulo, Objeto objetoFilho = null)
        {
            this.paiRef = paiRef;
            rotulo = _rotulo = Utilitario.CharProximo(_rotulo);
            if (paiRef != null)
            {
                ObjetoNovo(objetoFilho);
            }
        }

        private void ObjetoNovo(Objeto objetoFilho)
        {
            if (objetoFilho == null)
            {
                paiRef.objetosLista.Add(this);
            }
            else
            {
                paiRef.FilhoAdicionar(objetoFilho);
            }
        }

        public void ObjetoAtualizar()
        {
            float[] vertices = new float[pontosLista.Count * 3];
            int ptoLista = 0;
            for (int i = 0; i < vertices.Length; i += 3)
            {
                vertices[i] = (float)pontosLista[ptoLista].X;
                vertices[i + 1] = (float)pontosLista[ptoLista].Y;
                vertices[i + 2] = (float)pontosLista[ptoLista].Z;
                ptoLista++;
            }
            bBox.Atualizar(pontosLista);

            GL.PointSize(primitivaTamanho);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void Desenhar(Transformacao4D matrizGrafo)
        {
            // Ative o uso da textura antes de desenhar o retângulo
            texture.Use(TextureUnit.Texture0);

            // Renderize o retângulo usando a textura
            // Exemplo de código para desenhar um retângulo com textura
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
            GL.TexCoord2(1, 0); GL.Vertex2(1, 0);
            GL.TexCoord2(1, 1); GL.Vertex2(1, 1);
            GL.TexCoord2(0, 1); GL.Vertex2(0, 1);
            GL.End();

#if CG_OpenGL && !CG_DirectX
            GL.PointSize(primitivaTamanho);

            GL.BindVertexArray(_vertexArrayObject);

            if (paiRef != null)
            {
                matrizGrafo = matrizGrafo.MultiplicarMatriz(matriz);
                _shaderObjeto.SetMatrix4("transform", matrizGrafo.ObterDadosOpenTK());
                _shaderObjeto.Use();
                GL.DrawArrays(primitivaTipo, 0, pontosLista.Count);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
            }
            for (var i = 0; i < objetosLista.Count; i++)
            {
                objetosLista[i].Desenhar(matrizGrafo);
            }
        }

        #region Objeto: CRUD

        public void FilhoAdicionar(Objeto filho)
        {
            this.objetosLista.Add(filho);
        }

        // #3
        public void RemoverObjeto(Objeto objeto)
        {
            if (this.objetosLista.Contains(objeto))
            {
                Objeto proximoObjeto = GrafocenaBuscaProximo(objeto);
                char rotuloAtual = objeto.Rotulo;

                while (proximoObjeto != null && proximoObjeto.rotulo != 'A')
                {
                    Objeto proximoProximoObjeto = GrafocenaBuscaProximo(proximoObjeto);

                    char rotuloAntigo = proximoObjeto.Rotulo;
                    proximoObjeto.Rotulo = rotuloAtual;
                    rotuloAtual = rotuloAntigo;

                    proximoObjeto = proximoProximoObjeto;
                }

                this.objetosLista.Remove(objeto);
                Console.WriteLine("Objeto removido!");
            }
            else
            {
                Console.WriteLine("Não é possível remover o objeto, pois não está na lista.");
            }
        }

        public Ponto4D PontosId(int id)
        {
            return pontosLista[id];
        }

        public void PontosAdicionar(Ponto4D pto)
        {
            pontosLista.Add(pto);
            ObjetoAtualizar();
        }

        public void PontosRemover(Ponto4D pto)
        {
            pontosLista.Remove(pto);
            ObjetoAtualizar();
        }

        // #4
        public int PontosMaisPertoXY(Ponto4D pto)
        {
            int pontoMaisPerto = 0;
            float distancia = 999999999;

            for (int i = 0; i < pontosLista.Count; i++)
            {
                Ponto4D ponto = pontosLista[i];
                float calculo = ((float)Math.Sqrt(Math.Pow(ponto.X - pto.X, 2) + Math.Pow(ponto.Y - pto.Y, 2)));

                if (calculo < distancia)
                {
                    pontoMaisPerto = i;
                    distancia = calculo;
                }
            }

            return pontoMaisPerto;
        }

        // #9
        public bool PontoDentroBBox(Ponto4D ponto)
        {
            // Verificar se o ponto está dentro da bounding box do objeto
            if (ponto.X >= bBox.obterMenorX && ponto.X <= bBox.obterMaiorX &&
                ponto.Y >= bBox.obterMenorY && ponto.Y <= bBox.obterMaiorY &&
                ponto.Z >= bBox.obterMenorZ && ponto.Z <= bBox.obterMaiorZ)
            {
                return true;
            }

            return false;
        }
        public bool PontoDentroObjeto(Ponto4D ponto)
        {
            int intersectCount = 0;
            Ponto4D pontoInicial = pontosLista[pontosLista.Count - 1];

            for (int i = 0; i < pontosLista.Count; i++)
            {
                Ponto4D pontoFinal = pontosLista[i];

                if (((pontoInicial.Y <= ponto.Y) && (ponto.Y < pontoFinal.Y)) ||
                    ((pontoFinal.Y <= ponto.Y) && (ponto.Y < pontoInicial.Y)))
                {
                    if (ponto.X < (pontoFinal.X - pontoInicial.X) * (ponto.Y - pontoInicial.Y) / (pontoFinal.Y - pontoInicial.Y) + pontoInicial.X)
                    {
                        intersectCount++;
                    }
                }

                pontoInicial = pontoFinal;
            }

            return intersectCount % 2 == 1;
        }


        public Objeto BuscarObjetoPontoDentro(Ponto4D ponto)
        {
            for (var i = 0; i < objetosLista.Count; i++)
            {
                if (objetosLista[i].PontoDentroBBox(ponto))
                {
                    if (objetosLista[i].PontoDentroObjeto(ponto))
                    {
                        return objetosLista[i];
                    }
                }
            }
            return null;
        }
        public void PontosAlterar(Ponto4D pto, int posicao)
        {
            pontosLista[posicao] = pto;
            ObjetoAtualizar();
        }

        #endregion

        #region Objeto: Grafo de Cena

        public Objeto GrafocenaBusca(char _rotulo)
        {
            if (rotulo == _rotulo)
            {
                return this;
            }
            foreach (var objeto in objetosLista)
            {
                var obj = objeto.GrafocenaBusca(_rotulo);
                if (obj != null)
                {
                    return obj;
                }
            }
            return null;
        }

        public Objeto GrafocenaBuscaProximo(Objeto objetoAtual)
        {
            objetoAtual = GrafocenaBusca(Utilitario.CharProximo(objetoAtual.rotulo));
            if (objetoAtual != null)
            {
                return objetoAtual;
            }
            else
            {
                //return GrafocenaBusca(Utilitario.CharProximo('@'));
                return objetosLista[0];
            }
        }

        public void GrafocenaImprimir(String idt)
        {
            System.Console.WriteLine(idt + rotulo);
            foreach (var objeto in objetosLista)
            {
                objeto.GrafocenaImprimir(idt + "  ");
            }
        }

        #endregion

        #region Objeto: Transformações Geométricas

        public void MatrizImprimir()
        {
            System.Console.WriteLine(matriz);
        }
        public void MatrizAtribuirIdentidade()
        {
            matriz.AtribuirIdentidade();
            ObjetoAtualizar();
        }
        public void MatrizTranslacaoXYZ(double tx, double ty, double tz)
        {
            Transformacao4D matrizTranslate = new Transformacao4D();
            matrizTranslate.AtribuirTranslacao(tx, ty, tz);
            matriz = matrizTranslate.MultiplicarMatriz(matriz);
            ObjetoAtualizar();
        }
        public void MatrizEscalaXYZ(double Sx, double Sy, double Sz)
        {
            Transformacao4D matrizScale = new Transformacao4D();
            matrizScale.AtribuirEscala(Sx, Sy, Sz);
            matriz = matrizScale.MultiplicarMatriz(matriz);
            ObjetoAtualizar();
        }

        public void MatrizEscalaXYZBBox(double Sx, double Sy, double Sz)
        {
            matrizGlobal.AtribuirIdentidade();
            Ponto4D pontoPivo = bBox.obterCentro;

            matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
            matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

            matrizTmpEscala.AtribuirEscala(Sx, Sy, Sz);
            matrizGlobal = matrizTmpEscala.MultiplicarMatriz(matrizGlobal);

            matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
            matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

            matriz = matriz.MultiplicarMatriz(matrizGlobal);

            ObjetoAtualizar();
        }
        public void MatrizRotacaoEixo(double angulo)
        {
            switch (eixoRotacao)  // TODO: ainda não uso no exemplo
            {
                case 'x':
                    matrizTmpRotacao.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
                    break;
                case 'y':
                    matrizTmpRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
                    break;
                case 'z':
                    matrizTmpRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
                    break;
                default:
                    Console.WriteLine("opção de eixoRotacao: ERRADA!");
                    break;
            }
            ObjetoAtualizar();
        }
        public void MatrizRotacao(double angulo)
        {
            MatrizRotacaoEixo(angulo);
            matriz = matrizTmpRotacao.MultiplicarMatriz(matriz);
            ObjetoAtualizar();
        }
        public void MatrizRotacaoZBBox(double angulo)
        {
            matrizGlobal.AtribuirIdentidade();
            Ponto4D pontoPivo = bBox.obterCentro;

            matrizTmpTranslacao.AtribuirTranslacao(-pontoPivo.X, -pontoPivo.Y, -pontoPivo.Z); // Inverter sinal
            matrizGlobal = matrizTmpTranslacao.MultiplicarMatriz(matrizGlobal);

            MatrizRotacaoEixo(angulo);
            matrizGlobal = matrizTmpRotacao.MultiplicarMatriz(matrizGlobal);

            matrizTmpTranslacaoInversa.AtribuirTranslacao(pontoPivo.X, pontoPivo.Y, pontoPivo.Z);
            matrizGlobal = matrizTmpTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

            matriz = matriz.MultiplicarMatriz(matrizGlobal);

            ObjetoAtualizar();
        }

        #endregion

        public void OnUnload()
        {
            foreach (var objeto in objetosLista)
            {
                objeto.OnUnload();
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shaderObjeto.Handle);
        }

#if CG_Debug
        protected string ImprimeToString()
        {
            string retorno;
            retorno = "__ Objeto: " + rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[ " +
                string.Format("{0,10}", pontosLista[i].X) + " | " +
                string.Format("{0,10}", pontosLista[i].Y) + " | " +
                string.Format("{0,10}", pontosLista[i].Z) + " | " +
                string.Format("{0,10}", pontosLista[i].W) + " ]" + "\n";
            }
            retorno += bBox.ToString();
            return (retorno);
        }
#endif

    }
}

/*
+--------------------------------------+
|                Objeto                |
+--------------------------------------+
| - rotulo: char                        |
| + Rotulo: char { get; set; }          |
| - paiRef: Objeto                      |
| - objetosLista: List<Objeto>          |
| + PrimitivaTipo: PrimitiveType { get; set; }|
| + PrimitivaTamanho: float { get; set; }|
| + shaderCor: Shader                   |
| - pontosLista: List<Ponto4D>          |
| - _vertexBufferObject: int            |
| - _vertexArrayObject: int             |
| - bBox: BBox                          |
| - matriz: Transformacao4D             |
| + TrocaEixoRotacao(eixo: char)        |
| + Objeto(paiRef: Objeto, _rotulo: ref char, objetoFilho: Objeto)|
| - ObjetoNovo(objetoFilho: Objeto)     |
| + ObjetoAtualizar()                   |
| + Desenhar(matrizGrafo: Transformacao4D)|
| + FilhoAdicionar(filho: Objeto)       |
| + RemoverObjeto(objeto: Objeto)       |
| + PontosId(id: int): Ponto4D          |
| + PontosAdicionar(pto: Ponto4D)       |
| + PontosRemover(pto: Ponto4D)         |
| + PontosMaisPertoXY(pto: Ponto4D): int|
| + PontoDentroBBox(ponto: Ponto4D): bool|
| + PontoDentroObjeto(ponto: Ponto4D): bool|
| + BuscarObjetoPontoDentro(ponto: Ponto4D): Objeto|
| + PontosAlterar(pto: Ponto4D, posicao: int)|
| + GrafocenaBusca(_rotulo: char): Objeto|
| + GrafocenaBuscaProximo(objetoAtual: Objeto): Objeto|
| + GrafocenaImprimir(idt: String)      |
| + MatrizImprimir()                    |
| + MatrizAtribuirIdentidade()          |
| + MatrizTranslacaoXYZ(tx: double, ty: double, tz: double)|
| + MatrizEscalaXYZ(Sx: double, Sy: double, Sz: double)|
| + MatrizEscalaXYZBBox(Sx: double, Sy: double, Sz: double)|
| + MatrizRotacaoEixo(angulo: double)  |
| + MatrizRotacao(angulo: double)       |
| + MatrizRotacaoZBBox(angulo: double)  |
| + OnUnload()                          |
+--------------------------------------+


*/