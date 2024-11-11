// Include code libraries you need below (use the namespace).
using System;
using System.Numerics;

// The namespace your code is in.
namespace Game10003
{
    ///     this is a simple game of Breakout but with a twist!
    public class Game
    {
        // Initialize the box positions, the Box game objects, and the Character game object
        Vector2[] boxPositions = {

            // Box Positions
            new(100, 100), new(400, 250), new(150, 350), new(500, 450),

            // Paddle Position
            new(800 - 100, 600 - 60) };

        KeyboardInput[] destroyKeys = {
            KeyboardInput.W, // 0 (The first array always starts with 0 and never 1!)
            KeyboardInput.A, // 1
            KeyboardInput.S, // 2
            KeyboardInput.D  // 3
        };

        Box[] boxes = new Box[5];
        Ball myCharacter;
        float gameTimer = 0.0f;

        // static Game variables that can be accessed anywhere
        public static int Score = 0;
        public static bool EndGame = false;
        public static bool WinGame = false;

        ///     Setup runs once before the game loop begins.
        public void Setup()
        {
            Window.TargetFPS = 60;
            Window.SetTitle("The Breakout Game");
            Window.SetSize(800, 600);

            // initialize random number for key
            int keyIndex = Random.Integer(0, 3);

            // initialize each Box game object
            for (int i = 0; i < boxes.Length; i++)
            {
                if (i <= 3)
                {
                    // these are ordinary boxes
                    boxes[i] = new Box(boxPositions[i]); // "allocate" a new game object

                    boxes[i].DestroyKey = destroyKeys[keyIndex];

                    keyIndex++;
                    if (keyIndex >= destroyKeys.Length) keyIndex = 0;

                }
                else if (i == 4)
                {
                    // this is a paddle
                    boxes[i] = new Box(boxPositions[i], true);
                }
            }

            // set up the character object
            myCharacter = new Ball();
        }

        ///     Update runs every frame.
        public void Update()
        {
            Window.ClearBackground(Color.White);

            // update all the boxes
            int deadCounter = 0;
            foreach (Box box in boxes)
            {
                box.Update();
                if (!box.Alive) deadCounter++;
            }

            if (deadCounter == 4) Game.WinGame = true;

            if (Game.WinGame) Game.EndGame = true;

            // update the character - it needs to know about all the boxes!
            myCharacter.Update(boxes);

            if (Game.EndGame)
            {
                if (Game.WinGame)
                {
                    string multiLineText = "YEAH BRO YOU DID IT,\n \nI AM REALLY PROUD OF YOU!";
                    Text.Draw(multiLineText, new Vector2(Window.Width / 2 - 50, Window.Height / 2));

                }
                else
                {
                    Text.Draw($"ARE YA WINNING SON?", new Vector2(Window.Width / 2 - 50, Window.Height / 2));
                }
            }
            else gameTimer = Time.SecondsElapsed;

            Text.Draw($"Score: {Game.Score}", new Vector2(10, 10));
            Text.Draw($"Time: {gameTimer:F1} s", new Vector2(10, 50));
        }
    }
}