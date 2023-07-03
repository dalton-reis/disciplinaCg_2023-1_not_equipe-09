//https://github.com/mono/opentk/blob/main/Source/Examples/Shapes/Old/Cube.cs

#define CG_Debug
using CG_Biblioteca;
using OpenTK.Mathematics;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class Cubo : Objeto
  {
    Vector3[] vertices;
    int[] indices;
    Vector3[] normals;
    int[] colors;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;

  private char _rotulo;

    public Cubo(Objeto paiRef, ref char _rotulo) :
      this(paiRef, ref _rotulo, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5))
    { }

    public Cubo(Objeto paiRef, ref char _rotulo, Ponto4D ptoInfEsq, Ponto4D ptoSupDir) : base(paiRef, ref _rotulo)
    {
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");

      vertices = new Vector3[]
      {
        new Vector3(-1.0f, -1.0f,  1.0f),
        new Vector3( 1.0f, -1.0f,  1.0f),
        new Vector3( 1.0f,  1.0f,  1.0f),
        new Vector3(-1.0f,  1.0f,  1.0f),
        new Vector3(-1.0f, -1.0f, -1.0f),
        new Vector3( 1.0f, -1.0f, -1.0f),
        new Vector3( 1.0f,  1.0f, -1.0f),
        new Vector3(-1.0f,  1.0f, -1.0f)
      };

      indices = new int[]
      {
        0, 1, 2, 2, 3, 0, // front face
        3, 2, 6, 6, 7, 3, // top face
        7, 6, 5, 5, 4, 7, // back face
        4, 0, 3, 3, 7, 4, // left face
        0, 1, 5, 5, 4, 0, // bottom face  
        1, 5, 6, 6, 2, 1, // right face
      };

      normals = new Vector3[]
      {
        new Vector3(-1.0f, -1.0f,  1.0f),
        new Vector3( 1.0f, -1.0f,  1.0f),
        new Vector3( 1.0f,  1.0f,  1.0f),
        new Vector3(-1.0f,  1.0f,  1.0f),
        new Vector3(-1.0f, -1.0f, -1.0f),
        new Vector3( 1.0f, -1.0f, -1.0f),
        new Vector3( 1.0f,  1.0f, -1.0f),
        new Vector3(-1.0f,  1.0f, -1.0f),
      };

      // colors = new int[]
      // {
      //   ColorToRgba32(Color.DarkRed),
      //   ColorToRgba32(Color.DarkRed),
      //   ColorToRgba32(Color.Gold),
      //   ColorToRgba32(Color.Gold),
      //   ColorToRgba32(Color.DarkRed),
      //   ColorToRgba32(Color.DarkRed),
      //   ColorToRgba32(Color.Gold),
      //   ColorToRgba32(Color.Gold),
      // };


      // Construção dos quadrados que formam o cubo
      List<Ponto4D> pontosPoligonoQuadrado = new List<Ponto4D>();
      int pontosDoMesmoLado = -1;
      int corLado = 0;
      for (int i = 0; i <= 35; i++) 
      {
        pontosDoMesmoLado++;
        Vector3 vertice = vertices[indices[i]];
        pontosPoligonoQuadrado.Add(new Ponto4D(vertice.X, vertice.Y, vertice.Z));

        if (pontosDoMesmoLado == 5) 
        {
          pontosDoMesmoLado = -1;
          Objeto objeto = new Poligono(base.paiRef, ref this._rotulo, pontosPoligonoQuadrado);
          objeto.PrimitivaTipo = PrimitiveType.TriangleFan;
          if (corLado==0) {
            objeto.shaderCor = _shaderVermelha;
          } else if (corLado==0) {
            objeto.shaderCor = _shaderVerde;
          } else if (corLado==1) {
            objeto.shaderCor = _shaderAzul;
          } else if (corLado==2) {
            objeto.shaderCor = _shaderCiano;
          } else if (corLado==3) {
            objeto.shaderCor = _shaderVerde;
          } else if (corLado==4) {
            objeto.shaderCor = _shaderMagenta;
          } else if (corLado==5) {
            objeto.shaderCor = _shaderAmarela;
          }
          corLado++;
          
          pontosPoligonoQuadrado = new List<Ponto4D>();
        }
      }
      Atualizar();
    }

    public static int ColorToRgba32(Color c)
    {
      return (int)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
    }

    private void Atualizar()
    {

      base.ObjetoAtualizar();
    }
    
#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Cubo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}
