using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
namespace OpenGLTutorial1
{
    class Reader
    {
        static List<Square> readFromFile(string filename)
        {
            List<Square> result = new List<Square>();
           
            var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                streamReader.ReadLine();
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] args = line.Split(' ');
                    
                    int x = int.Parse(args[0]);
                    int y = int.Parse(args[1]);
                    int size = int.Parse(args[2]);
                    float r = int.Parse(args[3]) / 255f;
                    float g = int.Parse(args[4]) / 255f;
                    float b = int.Parse(args[5]) / 255f;
                     
                    result.Add(new Square(x,y,size,new Vector3(r,g,b)));
                }
            }
            return result;
        }
    }
}
