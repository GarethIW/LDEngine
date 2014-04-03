using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LDEngine
{
    enum SpriteAnimState
    {
        Playing,
        Paused
    }

    enum SpriteAnimDirection
    {
        Forward,
        Reverse
    }

    class SpriteAnim
    {
        public Texture2D SpriteSheet;
        public Vector2 Offset;
        public int NumFrames;
        public int SheetRow;
        public int CellWidth;
        public int CellHeight;
        public int CurrentFrame;
        public double CurrentFrameTime;
        public double TargetFrameTime;
        public bool Loop;
        public bool PingPong;
        public SpriteAnimDirection CurrentDirection;
        public SpriteAnimState State;

        public SpriteAnim(Texture2D sheet, int row, int numframes, int width, int height, double frametime)
        {
            SpriteSheet = sheet;
            Offset = new Vector2(sheet.Width,sheet.Height)/2f;
            NumFrames = numframes;
            SheetRow = row;
            CellWidth = width;
            CellHeight = height;
            CurrentFrame = 0;
            CurrentFrameTime = 0;
            TargetFrameTime = frametime;
            Loop = true;
            PingPong = false;
            CurrentDirection = SpriteAnimDirection.Forward;
            State = SpriteAnimState.Paused;
        }
        public SpriteAnim(Texture2D sheet, int row, int numframes, int width, int height, double frametime, Vector2 offset)
            :this(sheet, row, numframes, width, height, frametime)
        {
            Offset = offset;
        }
        public SpriteAnim(Texture2D sheet, int row, int numframes, int width, int height, double frametime, Vector2 offset, bool loop, bool pingpong, int startframe)
            : this(sheet, row, numframes, width, height, frametime, offset)
        {
            CurrentFrame = startframe;
            Loop = loop;
            PingPong = pingpong;
        }

        public void Update(GameTime gameTime)
        {
            if (State != SpriteAnimState.Playing) return;

            CurrentFrameTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (CurrentFrameTime < TargetFrameTime) return;

            CurrentFrameTime = 0;

            switch(CurrentDirection)
            {
                case SpriteAnimDirection.Forward:
                    CurrentFrame++;
                    if (CurrentFrame == NumFrames)
                    {
                        if (PingPong && NumFrames > 1)
                        {
                            CurrentDirection = SpriteAnimDirection.Reverse;
                            CurrentFrame -= 2;
                        }
                        else CurrentFrame = 0;
                    }
                    break;
                case SpriteAnimDirection.Reverse:
                    CurrentFrame--;
                    if (CurrentFrame == -1)
                    {
                        if (PingPong && NumFrames > 1)
                        {
                            CurrentDirection = SpriteAnimDirection.Forward;
                            CurrentFrame = 1;
                        }
                        else CurrentFrame = NumFrames-1;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Play() { State = SpriteAnimState.Playing; }
        public void Pause() { State = SpriteAnimState.Paused; }
        public void Reset()
        {
            CurrentDirection = SpriteAnimDirection.Forward;
            CurrentFrame = 0;
            CurrentFrameTime = 0;
        }

        public void Draw(SpriteBatch sb, Vector2 pos)
        {
            sb.Draw(SpriteSheet,
                    pos,
                    new Rectangle(CurrentFrame * CellWidth, SheetRow * CellHeight, CellWidth, CellHeight),
                    Color.White,
                    0f,
                    Offset,
                    1f,
                    SpriteEffects.None,
                    1);
        }
        public void Draw(SpriteBatch sb, Vector2 pos, SpriteEffects effects)
        {
            sb.Draw(SpriteSheet,
                    pos,
                    new Rectangle(CurrentFrame * CellWidth, SheetRow * CellHeight, CellWidth, CellHeight),
                    Color.White,
                    0f,
                    Offset,
                    1f,
                    effects,
                    1);
        }
        public void Draw(SpriteBatch sb, Vector2 pos, SpriteEffects effects, float scale)
        {
            sb.Draw(SpriteSheet,
                    pos,
                    new Rectangle(CurrentFrame * CellWidth, SheetRow * CellHeight, CellWidth, CellHeight),
                    Color.White,
                    0f,
                    Offset,
                    scale,
                    effects,
                    1);
        }
        public void Draw(SpriteBatch sb, Vector2 pos, SpriteEffects effects, float scale, float rotation)
        {
            sb.Draw(SpriteSheet,
                    pos,
                    new Rectangle(CurrentFrame * CellWidth, SheetRow * CellHeight, CellWidth, CellHeight),
                    Color.White,
                    rotation,
                    Offset,
                    scale,
                    effects,
                    1);
        }
        public void Draw(SpriteBatch sb, Vector2 pos, SpriteEffects effects, float scale, float rotation, Color tint)
        {
            sb.Draw(SpriteSheet,
                    pos,
                    new Rectangle(CurrentFrame * CellWidth, SheetRow * CellHeight, CellWidth, CellHeight),
                    tint,
                    rotation,
                    Offset,
                    scale,
                    effects,
                    1);
        }
        
    }
}
