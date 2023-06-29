#define CG_Gizmo  // debugar gráfico.
#define CG_OpenGL // render OpenGL.
// #define CG_DirectX // render DirectX.
#define CG_Privado // código do professor.

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

//FIXME: padrão Singleton

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        Objeto mundo;
        private char rotuloNovo = '?';
        private Objeto objetoSelecionado = null;

        private readonly float[] _sruEixos =
        {
      -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f, /* Z+ */
    };

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private Shader _shaderBranca;
        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Shader _shaderCiano;
        private Shader _shaderMagenta;
        private Shader _shaderAmarela;

        // #2
        private List<Ponto4D> vertices = new List<Ponto4D>();
        private bool isDrawingPolygon = false;

        // #4
        private Poligono poligonoSelecionado;
        private float distanciaMinima;
        private int verticeSelecionadoIndex;
        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo = new Objeto(null, ref rotuloNovo);
        }
        private void Diretivas()
        {
#if DEBUG
      Console.WriteLine("Debug version");
#endif
#if RELEASE
    Console.WriteLine("Release version");
#endif
#if CG_Gizmo
            Console.WriteLine("#define CG_Gizmo  // debugar gráfico.");
#endif
#if CG_OpenGL
            Console.WriteLine("#define CG_OpenGL // render OpenGL.");
#endif
#if CG_DirectX
      Console.WriteLine("#define CG_DirectX // render DirectX.");
#endif
#if CG_Privado
            Console.WriteLine("#define CG_Privado // código do professor.");
#endif
            Console.WriteLine("__________________________________ \n");
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            Diretivas();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            #region Cores
            _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
            #endregion

            #region Eixos: SRU  
            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            #endregion

            #region Objeto: polígono qualquer  
            List<Ponto4D> pontosPoligonoBandeira = new List<Ponto4D>();
            pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.25));
            pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.25));
            pontosPoligonoBandeira.Add(new Ponto4D(0.75, 0.75));
            pontosPoligonoBandeira.Add(new Ponto4D(0.50, 0.50));
            pontosPoligonoBandeira.Add(new Ponto4D(0.25, 0.75));
            objetoSelecionado = new Poligono(mundo, ref rotuloNovo, pontosPoligonoBandeira);
            #endregion
            #region declara um objeto filho ao polígono
            List<Ponto4D> pontosPoligonoTriangulo = new List<Ponto4D>();
            pontosPoligonoTriangulo.Add(new Ponto4D(0.50, 0.50));
            pontosPoligonoTriangulo.Add(new Ponto4D(0.75, 0.75));
            pontosPoligonoTriangulo.Add(new Ponto4D(0.25, 0.75));
            objetoSelecionado = new Poligono(objetoSelecionado, ref rotuloNovo, pontosPoligonoTriangulo);
            #endregion
            #region declara um objeto neto ao polígono
            objetoSelecionado = new Circulo(objetoSelecionado, ref rotuloNovo, 0.05, new Ponto4D(0.50, 0.50));
            objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
            #endregion

            #region Objeto: retângulo  
            objetoSelecionado = new Retangulo(mundo, ref rotuloNovo, new Ponto4D(-0.25, 0.25), new Ponto4D(-0.75, 0.75));
            objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
            #endregion

            // #region Objeto: segmento de reta  
            // objetoSelecionado = new SegReta(mundo, ref rotuloNovo, new Ponto4D(-0.5, -0.5), new Ponto4D());
            // #endregion

            // #region Objeto: ponto  
            // objetoSelecionado = new Ponto(mundo, ref rotuloNovo, new Ponto4D(-0.25, -0.25));
            // objetoSelecionado.PrimitivaTipo = PrimitiveType.Points;
            // objetoSelecionado.PrimitivaTamanho = 5;
            // #endregion

#if CG_Privado
            // #region Objeto: circulo  
            // objetoSelecionado = new Circulo(mundo, ref rotuloNovo, 0.2, new Ponto4D());
            // objetoSelecionado.shaderCor = _shaderAmarela
            // #endregion

            // #region Objeto: SrPalito  
            // objetoSelecionado = new SrPalito(mundo, ref rotuloNovo);
            // #endregion

            // #region Objeto: Spline
            // objetoSelecionado = new Spline(mundo, ref rotuloNovo);
            // #endregion
#endif

        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

#if CG_Gizmo
            Sru3D();
#endif
            mundo.Desenhar(new Transformacao4D());
            SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
            #region Teclado
            var input = KeyboardState;
            // #2
            if (input.IsKeyPressed(Keys.Enter))
            {
                if (isDrawingPolygon)
                {
                    isDrawingPolygon = false;

                    List<Ponto4D> verticesClonados = new List<Ponto4D>();
                    foreach (Ponto4D item in vertices)
                    {
                        Ponto4D itemClonado = item; // Copia o valor do elemento
                        verticesClonados.Add(itemClonado); // Adiciona o elemento clonado na nova lista
                    }

                    objetoSelecionado = new Poligono(mundo, ref rotuloNovo, verticesClonados);

                    vertices.Clear();

                    Console.WriteLine("Novo polígono desenhado!");
                }
            }
            // #3
            if (input.IsKeyPressed(Keys.D))
            {
                System.Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");

                int janelaLargura = Size.X;
                int janelaAltura = Size.Y;
                Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
                Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

                objetoSelecionado.PontosRemover(objetoSelecionado.PontosId(objetoSelecionado.PontosMaisPertoXY(sruPonto)));
            }
            // #5
            if (input.IsKeyPressed(Keys.E))
            {
                Objeto proximoObjeto = mundo.GrafocenaBuscaProximo(objetoSelecionado);

                mundo.RemoverObjeto(objetoSelecionado);

                objetoSelecionado = proximoObjeto;
                objetoSelecionado.shaderCor = _shaderAmarela;
            }
            // #9
            if (input.IsKeyPressed(Keys.S))
            {
                System.Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");

                int janelaLargura = Size.X;
                int janelaAltura = Size.Y;
                Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
                Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

                objetoSelecionado = mundo.BuscarObjetoPontoDentro(sruPonto);

                if(objetoSelecionado != null)
                    objetoSelecionado.ToString();
            }
            if (input.IsKeyDown(Keys.Escape))
                Close();
            if (input.IsKeyPressed(Keys.Space) && objetoSelecionado != null)
            {
                objetoSelecionado.shaderCor = _shaderBranca;
                objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
                if (objetoSelecionado != null)
                    objetoSelecionado.shaderCor = _shaderAmarela;
            }
            if (input.IsKeyPressed(Keys.G))
                mundo.GrafocenaImprimir("");
            // #8
            if (input.IsKeyPressed(Keys.R))
                if (objetoSelecionado != null)
                    objetoSelecionado.shaderCor = _shaderVermelha;
            if (input.IsKeyPressed(Keys.G))
                if (objetoSelecionado != null)
                    objetoSelecionado.shaderCor = _shaderVerde;
            if (input.IsKeyPressed(Keys.B))
                if (objetoSelecionado != null)
                    objetoSelecionado.shaderCor = _shaderAzul;
            // #7
            if (input.IsKeyPressed(Keys.P) && objetoSelecionado != null)
            {
                System.Console.WriteLine(objetoSelecionado.ToString());

                if (objetoSelecionado != null)
                    if (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineLoop)
                        objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
                    else
                        objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
            }
            if (input.IsKeyPressed(Keys.M) && objetoSelecionado != null)
                objetoSelecionado.MatrizImprimir();
            //TODO: não está atualizando a BBox com as transformações geométricas
            if (input.IsKeyPressed(Keys.I) && objetoSelecionado != null)
                objetoSelecionado.MatrizAtribuirIdentidade();
            if (input.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
            if (input.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
            if (input.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
            if (input.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
                objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
            if (input.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
            if (input.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
            if (input.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
            if (input.IsKeyPressed(Keys.End) && objetoSelecionado != null)
                objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);
            if (input.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
                objetoSelecionado.MatrizRotacao(10);
            if (input.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
                objetoSelecionado.MatrizRotacao(-10);
            if (input.IsKeyPressed(Keys.D3) && objetoSelecionado != null)
                objetoSelecionado.MatrizRotacaoZBBox(10);
            if (input.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
                objetoSelecionado.MatrizRotacaoZBBox(-10);
            #endregion

            #region  Mouse
            // #2
            if (MouseState.IsButtonPressed(MouseButton.Left))
            {
                if (!isDrawingPolygon)
                {
                    vertices.Clear();
                    isDrawingPolygon = true;
                }

                int janelaLargura = Size.X;
                int janelaAltura = Size.Y;
                Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
                Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

                vertices.Add(sruPonto);
            }
            if (MouseState.IsButtonPressed(MouseButton.Left))
            {
                System.Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
                System.Console.WriteLine("__ Valores do Espaço de Tela");
                System.Console.WriteLine("Vector2 mousePosition: " + MousePosition);
                System.Console.WriteLine("Vector2i windowSize: " + Size);
            }
            // #4
            if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
            {
                System.Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");

                int janelaLargura = Size.X;
                int janelaAltura = Size.Y;
                Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
                Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

                objetoSelecionado.PontosAlterar(sruPonto, objetoSelecionado.PontosMaisPertoXY(sruPonto));
            }
            if (MouseState.IsButtonReleased(MouseButton.Right))
            {
                System.Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");
            }

            #endregion
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }
        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteProgram(_shaderBranca.Handle);
            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);
            GL.DeleteProgram(_shaderCiano.Handle);
            GL.DeleteProgram(_shaderMagenta.Handle);
            GL.DeleteProgram(_shaderAmarela.Handle);

            base.OnUnload();
        }

#if CG_Gizmo
        private void Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
            var transform = Matrix4.Identity;
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.SetMatrix4("transform", transform);
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.SetMatrix4("transform", transform);
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.SetMatrix4("transform", transform);
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }
#endif
    }
}

/*
                              +-------------------+
                              |       Mundo       |
                              +-------------------+
                              | - mundo: Objeto   |
                              | - rotuloNovo: char|
                              | - objetoSelecionado: Objeto|
                              | - _sruEixos: float[]|
                              | - _vertexBufferObject_sruEixos: int|
                              | - _vertexArrayObject_sruEixos: int|
                              | - _shaderBranca: Shader|
                              | - _shaderVermelha: Shader|
                              | - _shaderVerde: Shader|
                              | - _shaderAzul: Shader|
                              | - _shaderCiano: Shader|
                              | - _shaderMagenta: Shader|
                              | - _shaderAmarela: Shader|
                              | - vertices: List<Ponto4D>|
                              | - isDrawingPolygon: bool|
                              | - poligonoSelecionado: Poligono|
                              | - distanciaMinima: float|
                              | - verticeSelecionadoIndex: int|
                              +-------------------+
                              | + OnLoad()        |
                              | + OnRenderFrame() |
                              | + OnUpdateFrame() |
                              +-------------------+

                    +-------------------+
                    |      Objeto       |
                    +-------------------+
                    | - pai: Objeto     |
                    | - rotulo: char    |
                    | - filho: Objeto   |
                    | - irmao: Objeto   |
                    | - primitivaTipo: PrimitiveType|
                    | - primitivaTamanho: int|
                    | - pontos: List<Ponto4D>|
                    | - shaderCor: Shader|
                    | - matriz: Transformacao4D|
                    +-------------------+
                    | + Desenhar(Transformacao4D)|
                    | + Remover()       |
                    | + GrafocenaBuscaProximo(Objeto): Objeto|
                    | + BuscarObjetoPontoDentro(Ponto4D): Objeto|
                    | + GrafocenaImprimir(string)|
                    | + MatrizImprimir()|
                    | + MatrizAtribuirIdentidade()|
                    | + MatrizTranslacaoXYZ(double, double, double)|
                    | + MatrizEscalaXYZ(double, double, double)|
                    | + MatrizEscalaXYZBBox(double, double, double)|
                    | + MatrizRotacao(double)|
                    +-------------------+

   +--------------+
   |  Poligono    |
   +--------------+
   | - area: double|
   +--------------+
   | + CalcularArea()|
   +--------------+

    +--------------+
    |   Retangulo  |
    +--------------+
    | - largura: double|
    | - altura: double |
    +--------------+
    | + CalcularArea()|
    +--------------+

     +--------------+
     |   SegReta    |
     +--------------+
     | - pontoA: Ponto4D|
     | - pontoB: Ponto4D |
     +--------------+

      +--------------+
      |   Ponto      |
      +--------------+
      | - ponto: Ponto4D|
      +--------------+

       +--------------+
       |   Circulo    |
       +--------------+
       | - raio: double|
       | - centro: Ponto4D |
       +--------------+

        +--------------+
        |   Spline     |
        +--------------+
        | - pontosControle: List<Ponto4D>|
        +--------------+
*/