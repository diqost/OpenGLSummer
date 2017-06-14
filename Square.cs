using OpenGL;
using System;

namespace OpenGLTutorial1
{
    class Square
    {
        public float x, y;
        public Vector3 size;
        public Vector3 color;
        public Vector2 dirrection;
        static Random rnd = new Random();
        const int bounds = 2;
        public Square(Square parent1, Square parent2)
        {
             
        }
        public Square(float x, float y, float size, Vector3 color)
        {
            this.x = x;
            this.y = y;
            this.size = new Vector3(size, size, 1);
            this.color = color;
            this.dirrection = new Vector2(rnd.Next(-255, 256), rnd.Next(-255, 256)).Normalize() * 2;

        }
        public void move(float deltaTime)
        {
            x += dirrection.X * deltaTime;
            y += dirrection.Y * deltaTime;
        }
        public void CheckBounds()
        {
            if (Math.Abs(x) > bounds)
            {
                x = Math.Sign(x + size.X / 2) * bounds;
                dirrection.X *= -1;
            }

            if (Math.Abs(y) > bounds)
            {
                y = Math.Sign(y + size.Y / 2) * bounds;
                dirrection.Y *= -1;
            }
        }
        public bool collide(Square other)
        {
            Vector2 dist = 
                new Vector2(x - other.x, y - other.y);
            return dist.Length <= (size.X + other.size.X);
        }
    }
}
