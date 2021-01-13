using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Numerics;
using [ANIMATOR NAMESPACE];

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

                Texture2D characterTexture = LoadTexture(filePath + "spritesheets/CharSheetEdit.png");
                Rectangle characterFrameRec = new Rectangle(0, 0, 84, 64);

                Vector2 characterPos = new Vector2(0, 0);

                Animator characterAnimator = new Animator("Test", 24, 1, 12);
                characterAnimator.AssignSprite(characterTexture);

                SetTargetFPS(60);

                // GAME LOOP
                while (!WindowShouldClose())
                {
                    // Update
                    characterAnimator.Play();

                    BeginDrawing();
                    ClearBackground(BLACK);

                    DrawTextureRec(characterAnimator.GetSprite(), characterAnimator.GetFrameRec(), characterPos, WHITE);

                    EndDrawing();
                }

                CloseWindow();
            }
        }
    }
}
