using System;
using System.Collections.Generic;
using Tao.FreeGlut;
using OpenGL;

namespace OpenGLTutorial1
{
    class Program
    {
        static int width = 1280, height = 720;
        static ShaderProgram program;
        static VBO<Vector3> squareVertexes;
        static VBO<int> squareElements;

        static VBO<Vector3> squareColor;

        static System.Diagnostics.Stopwatch watch;
        static float angle;
        static float move = 0;
        static float dirrection = 1;
        static List<Square> squares = new List<Square>();
        static void Main(string[] args)
        {
            // create an OpenGL window
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE
                | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("OpenGL Tutorial");

            // provide the Glut callbacks that are necessary for running this tutorial
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Gl.Enable(EnableCap.DepthTest);

            program = new ShaderProgram(VertexShader, FragmentShader);

            // set the view and projection matrix, which are static throughout this tutorial
            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

            // create a triangle
            squareVertexes = new VBO<Vector3>(new Vector3[] { new Vector3(-1, -1, 0),
                                                        new Vector3(-1, 1, 0),
                                                        new Vector3(1, 1, 0),
                                                        new Vector3(1, -1, 0)
                                                      });
            squareElements = new VBO<int>(new int[] { 0, 1, 2, 3 }, BufferTarget.ElementArrayBuffer);

            squares = Reader.readFromFile("input.txt");
           
            watch = System.Diagnostics.Stopwatch.StartNew();
       
            Glut.glutMainLoop();
        }
       

        static void OnDisplay() { }

        static void OnRenderFrame()
        {
            if (Math.Abs(move) > 1)
                dirrection = dirrection * (-1);


            watch.Stop();
            float deltaTime = (float)watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency;
            watch.Restart();

            // use the deltaTime to adjust the angle of the cube and pyramid
            angle += deltaTime;

            // set up the OpenGL viewport and clear both the color and depth bits
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit
                | ClearBufferMask.DepthBufferBit);


            // use our shader program
            Gl.UseProgram(program);
            Gl.BindBufferToShaderAttribute(squareVertexes, program, "vertexPosition");
            Gl.BindBuffer(squareElements);
            CheckCollisions();
            for (int i = 0; i < squares.Count; i++)
            {
                // Console.Write("Drawing square");
                squares[i].CheckBounds();
                squares[i].move(deltaTime);
                drawSquare(squares[i]);
            }
 

            Glut.glutSwapBuffers();
        }
        static void CheckCollisions()
        {
            List<Tuple<Square, Square>> collisions =
                new List<Tuple<Square,Square>>();
            for (int i = 0; i < squares.Count; i++)
                for (int j = 0; j < squares.Count; j++)
                {
                    if (squares[i].collide(squares[j])) {
                        bool found = false;
                        for (int k = 0; k < collisions.Count; k++)
                        {
                            bool ab = (collisions[k].Item1 == squares[i]
                                && collisions[k].Item2 == squares[j]);
                            bool ba = (collisions[k].Item1 == squares[j]
                                && collisions[k].Item2 == squares[i]);
                              
                            if (!ab && !ba)
                            {
                                Console.WriteLine("collision");
                                found = true;  
                            }
                        }
                        if (!found)
                            collisions.Add(
                                 new Tuple<Square, Square>(squares[i], squares[j]));

                    }

                }


            for (int i = 0; i < collisions.Count; i++)
            {
                if (collisions[i].Item1 != null)
                {
                    int index = squares.FindIndex(x => x == collisions[i].Item1);
                    if (index >= 0)
                        squares.RemoveAt(index);
                    index = squares.FindIndex(x => x == collisions[i].Item2);
                    if (index >= 0)
                        squares.RemoveAt(index);
                }
            }
        }
        static void drawSquare(Square square)
        {
            Matrix4 scaling = Matrix4.CreateScaling(square.size);
            Matrix4 translation = Matrix4.CreateTranslation(new Vector3(square.x, square.y, 0));
            program["model_matrix"].SetValue(scaling * translation);
            if (squareColor != null)
                squareColor.Dispose(); 
            squareColor= new VBO<Vector3>(new Vector3[] { square.color, square.color, square.color, square.color });
            Gl.BindBufferToShaderAttribute(squareColor, program, "vertexColor");
            Gl.DrawElements(BeginMode.Quads, squareElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

        }

        public static string VertexShader = @"
in vec3 vertexPosition;
in vec3 vertexColor;
out vec3 color;
uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;
void main(void)
{
    color = vertexColor;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        public static string FragmentShader = @"
#version 130
in vec3 color;
out vec4 fragment;
void main(void)
{
    fragment = vec4(color, 1);
}
";
    }

}