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
    ///     The Character game object will interact with the boxes in the level
    ///     This demonstrates basic rectangle collision resolution
    ///     The boxes do move when the character hits them
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

        ///     Update the character each frame.
        public void Update(Box[] boxes)
        {
            if (!Game.EndGame)
            {
                CollisionCheck(boxes);
                MoveCharacter();
            }
            DrawCharacter();
        }
        ///     Checking if a collision with a box occurs
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

        ///     Check and resolve a box collision. 
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
    }
}
