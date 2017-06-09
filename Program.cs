using System;
using Tao.FreeGlut;
using OpenGL;

namespace OpenGLTutorial1
{
    class Program
    {
        static int width = 1280, height = 720;
        static ShaderProgram program;
        static VBO<Vector3> triangle;
        static VBO<int> triangleElements;
        static VBO<Vector3> square;
        static VBO<int> squareElements;

        static VBO<Vector3> triangleColor;

        static System.Diagnostics.Stopwatch watch;
        static float angle;
        static float move = 0;
        static float dirrection = 1;

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
            triangle = new VBO<Vector3>(new Vector3[] { new Vector3(-1, 1, 1),
                                                        new Vector3(1, 1, 1),
                                                        new Vector3(1, 1, -1),
                                                        new Vector3(-1, 1, -1),
                                                        new Vector3(-1, -1, -1),
                                                        new Vector3(1, -1, -1),
                                                        new Vector3(1, -1, 1),
                                                        new Vector3(-1, -1, 1) });
            triangleElements = new VBO<int>(new int[] { 0, 1, 2, 3, 0, 7, 6, 1, 1,2,5,6,6,7,4,5,5,2,3,4,4,3,0,7 }, BufferTarget.ElementArrayBuffer);
           

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
            move = move + dirrection * deltaTime;

          // set up the OpenGL viewport and clear both the color and depth bits
          Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit
                | ClearBufferMask.DepthBufferBit);


            // use our shader program
            Gl.UseProgram(program);

 
            // transform the triangle
            program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3(0, 0, 0)));
            Gl.BindBufferToShaderAttribute(triangle, program, "vertexPosition");

            triangleColor = new VBO<Vector3>(new Vector3[] { new Vector3(0, 0, 1) , new Vector3(0, 1, 1 ), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1) });

            Gl.BindBufferToShaderAttribute(triangleColor, program, "vertexColor");
            Gl.BindBuffer(triangleElements);

            Gl.DrawElements(BeginMode.Quads, triangleElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            Glut.glutSwapBuffers();
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