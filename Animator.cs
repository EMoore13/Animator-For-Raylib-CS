using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Numerics;

namespace TestGame.src
{
	class Animator
    {
		float FrameWidth;
		float FrameHeight;
		float TimeRemainingFramesCounter;
		int PlaybackPosition;
		int DelayFramesCounter;
		int Rows;
		int Columns;
		int Framerate;
		int CurrentRow;
		int CurrentColumn;
		int CurrentFrame;
		bool bFlipH;
		bool bFlipV;
		bool bCanLoop;
		bool bReverse;
		bool bContinuous;
		bool bPaused;
		bool bIsAnimationFinished;
		bool bHasStartedPlaying;
		Rectangle FrameRec;
		Texture2D Sprite;
		string Name;


		public Animator(string AnimatorName, int NumOfFramesPerRow, int NumOfRows, int Speed, bool bPlayInReverse = false, bool bContinuous = false, bool bLooping = true)
		{
			Name = AnimatorName;
			Framerate = Speed == 0 ? 1 : Speed;
			Columns = NumOfFramesPerRow;
			Rows = NumOfRows == 0 ? 1 : NumOfRows;
			bReverse = bPlayInReverse;
			bCanLoop = bLooping;
			this.bContinuous = bContinuous;
			PlaybackPosition = 0;
			DelayFramesCounter = 0;
			CurrentFrame = 0;
			CurrentRow = 0;
			CurrentColumn = 0;
			bFlipH = false;
			bFlipV = false;
		}

		public void AssignSprite(Texture2D Sprite)
		{
			this.Sprite = Sprite;

			Restart();
		}

		public void ChangeSprite(Texture2D NewSprite, int NumOfFramesPerRow, int NumOfRows, int Speed, float DelayInSeconds, bool bPlayInReverse, bool bContinuous, bool bLooping)
		{
			DelayFramesCounter++;

			if (GetFPS() >= 0)
			{
				if (DelayFramesCounter > DelayInSeconds * GetFPS())
				{
					Rows = NumOfRows == 0 ? 1 : NumOfRows;
					Columns = NumOfFramesPerRow;
					Framerate = Speed;
					bCanLoop = bLooping;
					this.bContinuous = bContinuous;
					bReverse = bPlayInReverse;
					PlaybackPosition = 0;
					DelayFramesCounter = 0;
					bIsAnimationFinished = false;
					bHasStartedPlaying = !bPaused;

					AssignSprite(NewSprite);
				}
			}
		}

		public void FlipSprite(bool bHorizontalFlip, bool bVerticalFlip)
		{
			bFlipH = bHorizontalFlip;
			bFlipV = !bFlipV;

			if (bHorizontalFlip && bVerticalFlip)
			{
				FrameRec.width *= -1;
				FrameRec.height *= -1;
			}
			else if (bHorizontalFlip)
			{
				FrameRec.width *= -1;
			}
			else if (bVerticalFlip)
			{
				FrameRec.height *= -1;
			}
		}

		public void SetLooping(bool bLooping)
		{
			bCanLoop = bLooping;
		}

		public void SetContinuous(bool bIsContinuous)
		{
			bContinuous = bIsContinuous;
		}

		public void ResetFrameRec()
		{
			FrameRec.width = bFlipH ? -(float)Sprite.width / Columns : (float)Sprite.width / Columns;
			FrameRec.height = bFlipV ? -(float)Sprite.height / Rows : (float)Sprite.height / Rows;
			FrameWidth = FrameRec.width;
			FrameHeight = FrameRec.height;
			FrameRec.x = bReverse && bContinuous ? Sprite.width - FrameWidth : 0;
			FrameRec.y = bReverse && bContinuous ? Sprite.height - FrameHeight : 0;

			CurrentFrame = bReverse ? Columns - 1 : 0;
			CurrentRow = bReverse ? Rows - 1 : 0;
			CurrentColumn = bReverse ? Columns - 1 : 0;
		}

		public void GoToRow(int RowNumber)
		{
			if (RowNumber >= Rows)
			{
				FrameRec.y = (Rows - 1) * FrameHeight;
				CurrentRow = Rows - 1;
				TimeRemainingFramesCounter = (float)GetTotalTimeInFrames();
			}
			else if (Rows >= 1)
			{
				FrameRec.y = RowNumber == 0 ? 0 : RowNumber * FrameHeight;
				CurrentRow = RowNumber;
				TimeRemainingFramesCounter = ((float)(GetTotalTimeInFrames() - (RowNumber * Columns + Columns)));
			}
		}

		public void GoToColumn(int ColumnNumber)
		{
			if (ColumnNumber >= Columns)
			{
				FrameRec.x = (Columns - 1) * FrameWidth;
				CurrentColumn = Columns - 1;
				CurrentFrame = Columns - 1;
				TimeRemainingFramesCounter = (float)(GetTotalTimeInFrames()) - CurrentRow * Columns;
			}
			else if (Columns >= 1)
			{
				FrameRec.x = ColumnNumber == 0 ? 0 : ColumnNumber * FrameWidth;
				CurrentColumn = ColumnNumber;
				CurrentFrame = ColumnNumber;
				TimeRemainingFramesCounter = (float)(GetTotalTimeInFrames()) - CurrentRow * Columns - ColumnNumber;
			}
		}

		public void GoToFirstRow()
		{
			GoToRow(0);

			// Update time remaining
			TimeRemainingFramesCounter = (float)(GetTotalTimeInFrames()) - CurrentColumn;
		}

		public void GoToFirstColumn()
		{
			if (!bIsAnimationFinished)
			{
				GoToColumn(0);

				// Update time remaining
				if (bContinuous)
					TimeRemainingFramesCounter = ((float)(GetTotalTimeInFrames())) - Columns * CurrentRow;
				else
					TimeRemainingFramesCounter = ((float)Columns);
			}
			else
			{
				GoToColumn(0);
				TimeRemainingFramesCounter = bContinuous ? (float)(GetTotalTimeInFrames()) - (float)(GetTotalTimeInFrames()) / Rows * CurrentRow : 0;
			}
		}

		public void GoToLastRow()
		{
			GoToRow(Rows - 1);

			// Update time remaining
			if (bContinuous)
				TimeRemainingFramesCounter = (float)(GetTotalTimeInFrames()) - CurrentColumn - Columns * (Rows - 1); 
			else
				TimeRemainingFramesCounter = (float)(Columns - CurrentColumn);
		}

		public void GoToLastColumn()
		{
			if (!bIsAnimationFinished)
			{
				GoToColumn(Columns - 1);

				// Update time remaining
				if (bContinuous)
				{
					if (!bReverse)
					{
						if (Columns * CurrentRow != 0)
							TimeRemainingFramesCounter = (float)(GetTotalTimeInFrames()) - Columns * CurrentRow + Columns;
						else
							TimeRemainingFramesCounter = (float)(GetTotalTimeInFrames()) - Columns;
					}
					else
					{
						TimeRemainingFramesCounter = (float)(Columns * CurrentRow + Columns);
					}

				}
				else
					TimeRemainingFramesCounter = bIsAnimationFinished ? 0.0f : (float)(Columns);
			}
			else
			{
				GoToColumn(Columns - 1);
				TimeRemainingFramesCounter = (float)(GetTotalTimeInFrames()) - CurrentRow * Columns - Columns;
			}
		}

		public void GoToFrame(int FrameNumber)
		{
			// Does frame exist in sprite-sheet
			if (FrameNumber < Columns * Rows)
			{
				GoToRow(FrameNumber / Columns);
				GoToColumn(FrameNumber % Columns);
			}
			else
				Console.WriteLine("ERROR from GoToFrame(): Frame %u does not exist! %s sprite has frames from %u to %u.\n", FrameNumber, Name, 0, Rows * Columns - 1);
		}

		public void GoToFirstFrame()
		{
			GoToColumn(0);
		}

		public void GoToLastFrame()
		{
			GoToColumn(Columns - 1);
		}

		public void GoToFirstFrameOfSpriteSheet()
		{
			GoToRow(0);
			GoToColumn(0);
		}

		public void GoToLastFrameOfSpriteSheet()
		{
			GoToRow(Rows - 1);
			GoToColumn(Columns - 1);
		}

		public void NextFrame()
		{
			// Only increment when animation is playing
			if (!bIsAnimationFinished)
			{
				CurrentFrame++;
				CurrentColumn++;
			}

			if (bCanLoop)
			{
				// Are we over the max columns
				if (CurrentFrame > Columns - 1)
				{
					// If we are continuous, Go to the next row in the sprite-sheet
					if (bContinuous)
					{
						NextRow();
						GoToFirstColumn();
					}
					// Otherwise, Go back to the start
					else
					{
						GoToFirstColumn();
					}
				}
			}
			else
			{
				// Are we over the max columns
				if (CurrentFrame > Columns - 1)
				{
					// If we are continuous, Go to the next row in the sprite-sheet
					if (bContinuous)
					{
						// Clamp values back down
						CurrentFrame = Columns - 1;
						CurrentColumn = Columns - 1;

						// Go to next row if we are not at the last frame
						if (!IsAtLastFrame())
						{
							NextRow();
							GoToFirstColumn();
						}
						else
							bIsAnimationFinished = true;

					}
					// Otherwise, Stay at the end
					else
					{
						bIsAnimationFinished = true;
						GoToLastColumn();
					}
				}
			}
		}

		public void PreviousFrame()
		{
			// Only decrement when animation is playing
			if (!bIsAnimationFinished)
			{
				CurrentFrame--;
				CurrentColumn--;
			}

			if (bCanLoop)
			{
				// Are we over the max columns OR equal to zero
				if (CurrentFrame == 0 || CurrentFrame > Columns)
				{
					// If we are continuous, Go to the previous row in the sprite-sheet
					if (bContinuous)
					{
						PreviousRow();
						GoToLastColumn();
					}
					// Otherwise, Go back to the last column
					else
					{
						GoToLastColumn();
					}
				}
			}
			else
			{
				// Are we over the max columns OR equal to zero
				if (CurrentFrame == 0 || CurrentFrame > Columns)
				{
					// If we are continuous, Go to the previous row in the sprite-sheet
					if (bContinuous)
					{
						// Clamp values back down
						CurrentFrame = 0;
						CurrentColumn = 0;

						// Go to previous row if we are not at the first frame
						if (!IsAtFirstFrame())
						{
							PreviousRow();
							GoToLastColumn();
						}
						else
							bIsAnimationFinished = true;
					}
					// Otherwise, Stay at the start
					else
					{
						bIsAnimationFinished = true;
						GoToFirstColumn();
					}
				}
			}
		}

		private float Lerp(float Start, float End, float Alpha)
		{
			return (1.0f - Alpha) * Start + Alpha * End;
		}

		public void NextRow()
		{
			FrameRec.y += FrameHeight;

			if (FrameRec.y >= Sprite.height)
			{
				// Go to start
				if (bCanLoop)
				{
					FrameRec.y = 0;
					CurrentRow = 0;
				}
				// Stay at end
				else
				{
					FrameRec.y = (float)(Sprite.height);
					CurrentRow = Rows - 1;
				}

				ResetTimer();
			}
			else
				CurrentRow++;

			// Update the time remaining
			TimeRemainingFramesCounter = GetTotalTimeInFrames() - ((float)(GetTotalTimeInFrames()) / Rows) * CurrentRow;
		}

		public void PreviousRow()
		{
			FrameRec.y -= FrameHeight;

			if (FrameRec.y < 0)
			{
				FrameRec.y = (float)(Sprite.height) - FrameHeight;
				CurrentRow = Rows - 1;
				ResetTimer();
			}
			else
				CurrentRow--;

			// Update the time remaining
			if (!bReverse)
				TimeRemainingFramesCounter = GetTotalTimeInFrames() - ((float)(GetTotalTimeInFrames()) / Rows) * CurrentRow;
		}

		public void NextColumn()
		{
			FrameRec.x += FrameWidth;

			if (FrameRec.x > Sprite.width)
			{
				FrameRec.x = 0;
				CurrentColumn = 0;
			}
			else
				CurrentColumn++;

			// Update the time remaining
			TimeRemainingFramesCounter -= 1;
		}

		public void PreviousColumn()
		{
			FrameRec.x -= FrameWidth;

			if (FrameRec.x < 0)
			{
				FrameRec.x = (float)(Sprite.width) - FrameWidth;
				CurrentColumn = Columns - 1;
			}
			else
				CurrentColumn--;

			// Update the time remaining
			TimeRemainingFramesCounter += 1;
		}

		public void Forward()
		{
			if (bReverse)
				bReverse = false;
		}

		public void Reverse(bool bToggle)
		{
			if (bToggle)
			{
				bReverse = !bReverse;
				TimeRemainingFramesCounter += GetTotalTimeInFrames() - TimeRemainingFramesCounter * 2;
				bIsAnimationFinished = false;
			}
			else
			{
				bReverse = true;
				TimeRemainingFramesCounter += GetTotalTimeInFrames() - TimeRemainingFramesCounter * 2;
				bIsAnimationFinished = false;
			}
		}

		public void Restart()
		{
			ResetFrameRec();
			ResetTimer();
			bHasStartedPlaying = true;
		}

		public int GetTotalFrames()
		{
			return Rows * Columns;
		}

		public int GetTotalRows()
		{
			return Rows;
		}

		public int GetTotalColumns()
		{
			return Columns;
		}

		public int GetCurrentFrame()
		{
			return CurrentRow * Columns + CurrentColumn;
		}

		public int GetCurrentRow()
		{
			return CurrentRow;
		}

		public int GetCurrentColumn()
		{
			return CurrentColumn;
		}

		public int GetTotalTimeInFrames()
		{
			return bContinuous ? Columns * Rows : Columns;
		}

		public float GetTotalTimeInSeconds()
		{
			return bContinuous ? (float)(Columns * Rows) / Framerate : (float)(Columns) / Framerate;
		}

		public float GetTimeRemainingInFrames()
		{
			return TimeRemainingFramesCounter;
		}

		public float GetTimeRemainingInSeconds()
		{
			return (float)(TimeRemainingFramesCounter); // Framerate;
		}

		public string GetName()
		{
			return Name;
		}

		private void CountdownInFrames()
		{
			if (TimeRemainingFramesCounter != 0.0f)
				TimeRemainingFramesCounter -= GetFrameTime() < 0.01f ? Framerate * GetFrameTime() : 0.0f;

			if (TimeRemainingFramesCounter <= 0.0f)
				TimeRemainingFramesCounter = 0.0f;
		}

		public void Play()
		{
			if (!bPaused)
			{
				PlaybackPosition++;

				// Update the time remaining
				if (!bIsAnimationFinished)
					CountdownInFrames();

				// Has 'X' amount of frames passed?
				if (PlaybackPosition > GetFPS() / Framerate)
				{
					// Reset playback position
					PlaybackPosition = 0;

					// Go to previous frame when reversing
					if (bReverse)
						PreviousFrame();
					// Go to next frame if not reversing
					else
						NextFrame();
				}

				// Only go to next frame if animation has not finished playing
				if (!bIsAnimationFinished)
					FrameRec.x = (float)(CurrentFrame) * FrameWidth;

				bHasStartedPlaying = false;
			}
		}

		private void LerpAnim(float Speed, bool bConstant)
		{
			PlaybackPosition++;
			if (PlaybackPosition > GetFPS() / Framerate)
			{
				PlaybackPosition = 0;

				if (bConstant)
					FrameRec.x += Speed * GetFrameTime();
				else
					FrameRec.x = Lerp(FrameRec.x, Sprite.width, Speed * GetFrameTime());
			}
		}

		public void Start()
		{
			UnPause();

			if (!bHasStartedPlaying)
				bHasStartedPlaying = true;
		}

		public void Stop()
		{
			PlaybackPosition = 0;
			CurrentColumn = 0;
			CurrentFrame = 0;
			CurrentRow = 0;
			bHasStartedPlaying = true;
			bIsAnimationFinished = true;

			ResetFrameRec();
			ResetTimer();
			Pause(bPaused);
		}

		public void UnPause()
		{
			bPaused = false;
			bHasStartedPlaying = true;
		}

		public void Pause(bool bToggle)
		{
			if (bToggle)
			{
				bPaused = !bPaused;
				bHasStartedPlaying = !bPaused;
			}
			else
			{
				bPaused = true;
				bHasStartedPlaying = false;
			}
		}

		public void SetFramerate(int NewFramerate)
		{
			Framerate = NewFramerate;
		}

		public bool IsAtFrame(int FrameNumber)
		{
			// Does frame exist in sprite-sheet
			if (FrameNumber < Columns * Rows)
			{
				int RowFrameNumberIsOn = FrameNumber / Columns;
				int ColumnFrameNumberIsOn = FrameNumber % Columns;

				return IsAtRow(RowFrameNumberIsOn) && IsAtColumn(ColumnFrameNumberIsOn);
			}

			Console.WriteLine("ERROR from IsAtFrame(): Frame %u does not exist! %s sprite has frames from %u to %u.\n", FrameNumber, Name, 0, Rows * Columns - 1);
			return false;
		}

		public bool IsAtRow(int RowNumber)
		{
			if (RowNumber < Rows)
				return RowNumber == CurrentRow;

			Console.WriteLine("ERROR from IsAtRow(): Row does not exist!\n");
			return false;
		}

		public bool IsAtColumn(int ColumnNumber)
		{
			if (ColumnNumber < Columns)
				return ColumnNumber == CurrentColumn;

			Console.WriteLine("ERROR from IsAtColumn(): Column does not exist!\n");
			return false;
		}

		public bool IsAtFirstFrameOfSpriteSheet()
		{
			return IsAtFirstRow() && IsAtFirstColumn();
		}

		public bool IsAtLastFrameOfSpriteSheet()
		{
			return IsAtLastRow() && IsAtLastColumn();
		}

		public bool IsAtFirstRow()
		{
			return CurrentRow == 0;
		}

		public bool IsAtFirstColumn()
		{
			return CurrentColumn == 0;
		}

		public bool IsAtFirstFrame()
		{
			return bContinuous ? IsAtFirstRow() && IsAtFirstColumn() : IsAtFirstColumn();
		}

		public bool IsAtLastFrame()
		{
			return bContinuous ? IsAtLastRow() && IsAtLastColumn() : IsAtLastColumn();
		}

		private void ResetTimer()
		{
			TimeRemainingFramesCounter = (float)(GetTotalTimeInFrames());
		}

		public Rectangle GetFrameRec()
		{
			return FrameRec;
		}

		public Texture2D GetSprite()
		{
			return Sprite;
		}

		public bool IsAtLastRow()
		{
			return CurrentRow == Rows - 1;
		}

		public bool IsAtLastColumn()
		{
			return CurrentColumn == Columns - 1;
		}

		public bool IsStartedPlaying()
		{
			if (IsAtFirstFrame())
			{
				ResetTimer();
				return true;
			}

			return bHasStartedPlaying;
		}

		public bool IsFinishedPlaying()
		{
			if (IsAtLastFrame())
			{
				ResetTimer();
				return true;
			}

			if (!bCanLoop)
				return bIsAnimationFinished;

			return bIsAnimationFinished;
		}

		public bool IsPlaying()
		{
			if (bCanLoop)
				return !bPaused;

			if (!bCanLoop && bContinuous)
				return !bIsAnimationFinished;

			return !bIsAnimationFinished;
		}
	}
}
