using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Game10003
{
    /// <summary>
    ///     The Character game object will interact with the boxes in the level
    ///     This demonstrates basic rectangle collision resolution
    ///     The boxes do move when the character hits them
    /// </summary>
    public class Ball
    {
        private Vector2 position;
        private Vector2 ballSize;
        private Vector2 velocity;
        private Color ballColor;

        /// <summary>
        ///     Set up the character size, position, velocity, and color when the game object is created
        /// </summary>
        public Ball()
        {
            ballSize = new Vector2(50, 50);
            position = new Vector2(Window.Width / 2, 150);
            velocity = new Vector2(200, 200);
            ballColor = Color.Red;
        }

        /// <summary>
        ///     Update the character each frame.
        /// </summary>
        /// <param name="boxes">The full array of boxes that exists from the Game.cs class</param>
        public void Update(Box[] boxes)
        {
            if (!Game.EndGame)
            {
                CollisionCheck(boxes);
                MoveCharacter();
            }
            DrawCharacter();
        }

        /// <summary>
        ///     Checking if a collision with a box occurs
        /// </summary>
        /// <param name="box">A single Box game object to check against.</param>
        /// <returns></returns>
        private bool IsCollidingWithBox(Box box)
        {
            float characterLeft = position.X;
            float characterRight = position.X + ballSize.X;
            float characterTop = position.Y;
            float characterBottom = position.Y + ballSize.Y;

            float boxLeft = box.Position.X;
            float boxRight = box.Position.X + box.Size.X;
            float boxTop = box.Position.Y;
            float boxBottom = box.Position.Y + box.Size.Y;

            // check all four edges on both rectangles - if ALL the conditions are true,
            // then we know we have a collision
            return characterLeft <= boxRight && characterRight >= boxLeft &&
                characterTop <= boxBottom && characterBottom >= boxTop;
        }

        /// <summary>
        ///     Check and resolve a box collision.
        /// </summary>
        /// <param name="box">A single Box game object to check against.</param>
        private void CollisionWithBox(Box box)
        {
            // highlighting logic
            if (!box.IsPaddle)
            {
                if (Vector2.Distance(position, box.Position) < 100)
                {
                    box.Highlight = true;

                    if (Input.IsKeyboardKeyPressed(box.DestroyKey))
                    {
                        Game.Score++;
                        box.Alive = false;
                    }
                }
                else box.Highlight = false;
            }

            // step 1: check. are we colliding?
            if (IsCollidingWithBox(box))
            {
                // step 2: collision resolution

                // there's a little bit of code duplication here... we could fix this with a struct that we pass around,
                // if we wanted to
                float characterLeft = position.X;
                float characterRight = position.X + ballSize.X;
                float characterTop = position.Y;
                float characterBottom = position.Y + ballSize.Y;

                float boxLeft = box.Position.X;
                float boxRight = box.Position.X + box.Size.X;
                float boxTop = box.Position.Y;
                float boxBottom = box.Position.Y + box.Size.Y;

                // this number is a "threshold" that lets us check which edge we are closest to
                // it helps compensate when the character edge "overshoots" the box edge
                float colThreshold = 15.0f;

                // determine which side is colliding from the perpsective of the box
                bool colLeft = MathF.Abs(characterRight - boxLeft) < colThreshold;
                bool colRight = MathF.Abs(characterLeft - boxRight) < colThreshold;
                bool colTop = MathF.Abs(characterBottom - boxTop) < colThreshold;
                bool colBottom = MathF.Abs(characterTop - boxBottom) < colThreshold;

                // check how close each side is to the opposings side and resolve the collision accordingly
                if (colTop && velocity.Y > 0)
                {
                    position.Y = boxTop - ballSize.Y;
                    velocity.Y *= -1;
                }
                if (colBottom && velocity.Y < 0)
                {
                    position.Y = boxBottom;
                    velocity.Y *= -1;
                }
                if (colLeft && velocity.X > 0)
                {
                    position.X = boxLeft - ballSize.X;
                    velocity.X *= -1;
                }
                if (colRight && velocity.X < 0)
                {
                    position.X = boxRight;
                    velocity.X *= -1;
                }
            }
        }

        /// <summary>
        ///     Check collisions with the screen
        /// </summary>
        private void CollisionWithWindow()
        {
            if (position.X <= 0 && velocity.X < 0)
            {
                position.X = 0;
                velocity.X *= -1;
            }
            if (position.X + ballSize.X >= Window.Size.X && velocity.X > 0)
            {
                position.X = Window.Size.X - ballSize.X;
                velocity.X *= -1;
            }
            if (position.Y < 0 && velocity.Y < 0)
            {
                position.Y = 0;
                velocity.Y *= -1;
            }
            if (position.Y + ballSize.Y >= Window.Size.Y && velocity.Y > 0)
            {
                position.Y = Window.Size.Y - ballSize.Y;
                velocity.Y *= -1;

                Game.EndGame = true;
            }
        }

        /// <summary>
        ///     Check all collisions for the character. Iterate through all boxes, and check against the window as well.
        /// </summary>
        /// <param name="boxes">The full array of boxes that exists from the Game.cs class</param>
        private void CollisionCheck(Box[] boxes)
        {
            foreach (Box box in boxes)
            {
                if (!box.Alive) continue;

                CollisionWithBox(box);
            }

            CollisionWithWindow();
        }

        /// <summary>
        ///     Update the character's position
        /// </summary>
        private void MoveCharacter()
        {
            position += velocity * Time.DeltaTime;
        }

        /// <summary>
        ///     Render the character on screen.
        /// </summary>
        private void DrawCharacter()
        {
            Draw.LineColor = Color.Black;
            Draw.LineSize = 1;
            Draw.FillColor = ballColor;
            Draw.Rectangle(position, ballSize);

            Draw.FillColor = Color.LightGray;
            Draw.Circle(position + ballSize / 2, ballSize.X / 4);
        }
    }
}
