using System;
using System.Numerics;

namespace Game10003
{
    ///     The Box class represents a series of boxes that move across the screen with added variations,
    ///     bouncing back when hitting the screen edges. Boxes do not interact with each other.
    public class Box
    {
        // PUBLIC VARIABLES
        public Vector2 Position;
        public Vector2 Size;
        public bool IsPaddle;
        public bool Highlight = false;
        public bool Alive = true;
        public KeyboardInput DestroyKey;

        // PRIVATE VARIABLES
        private float movementSpeed = 400.0f; // pixels per second movement speed
        private Vector2 direction;
        private float verticalOscillationAmplitude = 20.0f; // amplitude for vertical movement
        private float oscillationFrequency = 2.0f; // frequency for oscillation
        private float currentOscillationPhase = 0.0f;
        private Color boxBaseColor;
        private Color boxHighlightColor = Color.Cyan;

        public Box(Vector2 startPosition, bool isPaddle = false)
        {
            this.IsPaddle = isPaddle;
            this.Position = startPosition;

            if (this.IsPaddle)
            {
                this.boxBaseColor = Color.Red;
                this.Size = new Vector2(200, 30);
            }
            else
            {
                // Assign a randomized color and size for the non-paddle boxes
                this.boxBaseColor = Random.Color();
                this.Size = new Vector2(Random.Float(50, 100), Random.Float(40, 60));
                this.direction = new Vector2(Random.Float(-1.0f, 1.0f), 0); // Randomized horizontal direction
            }
        }

        /// <summary>
        /// Updates the state of the Box in each frame.
        /// </summary>
        public void Update()
        {
            if (Alive && !Game.EndGame)
            {
                UpdatePosition();
                CheckCollision();
                RenderBox();
            }
        }

        /// <summary>
        /// Updates the position of the box, with vertical oscillation for non-paddles.
        /// </summary>
        private void UpdatePosition()
        {
            if (IsPaddle)
            {
                Position.X = Input.GetMouseX() - (Size.X / 2);
                Position.Y = Window.Height - 60;
            }
            else
            {
                Position += direction * movementSpeed * Time.DeltaTime;
                currentOscillationPhase += Time.DeltaTime * oscillationFrequency;
                Position.Y += (float)Math.Sin(currentOscillationPhase) * verticalOscillationAmplitude;
            }
        }

        /// <summary>
        /// Renders the box on the screen.
        /// </summary>
        private void RenderBox()
        {
            Draw.LineColor = boxBaseColor;
            Draw.LineSize = 1;
            Draw.FillColor = Highlight ? boxHighlightColor : boxBaseColor;
            Draw.Rectangle(Position, Size);
        }

        /// <summary>
        /// Checks for collisions with the screen boundaries and applies a slight bounce effect.
        /// </summary>
        private void CheckCollision()
        {
            if ((Position.X <= 0 && direction.X < 0) || (Position.X + Size.X >= Window.Size.X && direction.X > 0))
            {
                Position.X = Math.Clamp(Position.X, 0, Window.Width - Size.X);
                direction.X *= -1;
                movementSpeed *= 0.95f; // slight speed reduction to simulate a bounce effect
            }
            if ((Position.Y < 0 && direction.Y < 0) || (Position.Y + Size.Y >= Window.Size.Y && direction.Y > 0))
            {
                Position.Y = Math.Clamp(Position.Y, 0, Window.Height - Size.Y);
                direction.Y *= -1;
                movementSpeed *= 0.95f; // slight speed reduction to simulate a bounce effect
            }
        }
    }
}
