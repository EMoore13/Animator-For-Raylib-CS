using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Numerics;
using TestGame.src;

namespace [INSERT NAMESPACE]
{
    class AnimatorExample
    {
        class Program
        {
            static void Main(string[] args)
            {
                // INIT WINDOW
                const int screenW = 1280;
                const int screenH = 768;
                InitWindow(screenW, screenH, "Animator Example");

                // Load Textures
                Texture2D ojaTexture = LoadTexture("spritesheets/[INSERT SPRITESHEET HERE].png");

                // Load Player Class
                Player p = new Player(ojaTexture, new Vector2(0f, 0f), new Rectangle(0, 0, 80, 54));

                SetTargetFPS(60);

                // GAME LOOP
                while (!WindowShouldClose())
                {
                    // Update
                    p.Update();

                    BeginDrawing();
                    ClearBackground(BLACK);

                    p.Draw();

                    EndDrawing();
                }

                CloseWindow();
            }
        }
    }
}
