using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celesteia.Resources;
using Celesteia.Graphics;
using System;
using Celesteia.Resources.Sprites;
using Celesteia.Game.Components.Items;

namespace Celesteia.Game.Planets {
    public class Chunk {
        public const int CHUNK_SIZE = 16;

        public Point TruePosition { get; private set; }
        private ChunkVector _position;
        public ChunkVector Position {
            get => _position;
            set { _position = value; TruePosition = _position.Resolve(); }
        }

        private BlockState[,] foreground;
        private BlockState[,] background;

        public Chunk(ChunkVector cv) {
            Position = cv;

            foreground = new BlockState[CHUNK_SIZE, CHUNK_SIZE];
            background = new BlockState[CHUNK_SIZE, CHUNK_SIZE];
        }

        public BlockState GetForeground(int x, int y) => foreground[x, y];
        public BlockState GetBackground(int x, int y) => background[x, y];

        public void SetForeground(int x, int y, byte id) {
            foreground[x, y].BlockID = id;
            foreground[x, y].BreakProgress = 0;
            UpdateDraws(x, y);
        }

        public void SetBackground(int x, int y, byte id) {
            background[x, y].BlockID = id;
            background[x, y].BreakProgress = 0;
            UpdateDraws(x, y);
        }

        public void UpdateDraws(int x, int y) {
            foreground[x, y].Draw = foreground[x, y].HasFrames();
            background[x, y].Draw = background[x, y].HasFrames() && foreground[x, y].Translucent;
        }

        private NamespacedKey? dropKey;
        public void AddBreakProgress(int x, int y, int power, bool wall, out ItemStack drops) {
            dropKey = null;
            drops = null;

            if (wall) {
                background[x, y].BreakProgress += power;
                if (background[x, y].BreakProgress > background[x, y].Type.Strength) {
                    dropKey = background[x, y].Type.DropKey;
                    SetBackground(x, y, 0);
                }
            } else {
                foreground[x, y].BreakProgress += power;
                if (foreground[x, y].BreakProgress > foreground[x, y].Type.Strength) {
                    dropKey = foreground[x, y].Type.DropKey;
                    SetForeground(x, y, 0);
                }
            }

            if (dropKey.HasValue) drops = new ItemStack(dropKey.Value, 1);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            for (int i = 0; i < CHUNK_SIZE; i++) {
                for (int j = 0; j < CHUNK_SIZE; j++) {
                    DrawAllAt(i, j, gameTime, spriteBatch);
                }
            }
        }

        Vector2 v;
        private void DrawAllAt(int x, int y, GameTime gameTime, SpriteBatch spriteBatch) {
            v.X = TruePosition.X + x;
            v.Y = TruePosition.Y + y;
            v.Floor();

            if (background[x, y].Draw) {
                DrawWallTile(background[x, y].Frames.GetFrame(0), spriteBatch, v);
                if (background[x, y].BreakProgress > 0) DrawWallTile(ResourceManager.Blocks.BreakAnimation.GetProgressFrame(
                    // Background block breaking progress.
                    background[x, y].BreakProgress / (float) background[x, y].Type.Strength
                ), spriteBatch, v);
            }
            if (foreground[x, y].Draw) {
                DrawTile(foreground[x, y].Frames.GetFrame(0), spriteBatch, v);
                if (foreground[x, y].BreakProgress > 0) DrawTile(ResourceManager.Blocks.BreakAnimation.GetProgressFrame(
                    // Foreground block breaking progress.
                    foreground[x, y].BreakProgress / (float) foreground[x, y].Type.Strength
                ), spriteBatch, v);
            }  
        }

        public void DrawTile(BlockFrame frame, SpriteBatch spriteBatch, Vector2 v) {
            frame.Draw(0, spriteBatch, v, Color.White, 0.4f);
        }

        public void DrawWallTile(BlockFrame frame, SpriteBatch spriteBatch, Vector2 v) {
            frame.Draw(0, spriteBatch, v, Color.DarkGray, 0.5f);
        }
    }

    public struct ChunkVector {
        public int X;
        public int Y;

        public ChunkVector(int x, int y) {
            X = x;
            Y = y;
        }

        public static ChunkVector FromVector2(Vector2 vector)
        {
            return new ChunkVector(
                (int)MathF.Floor(vector.X / Chunk.CHUNK_SIZE),
                (int)MathF.Floor(vector.Y / Chunk.CHUNK_SIZE)
            );
        }

        public static ChunkVector FromPoint(Point point)
        => new ChunkVector(point.X / Chunk.CHUNK_SIZE, point.Y/ Chunk.CHUNK_SIZE);

        public Point Resolve() {
            return new Point(X * Chunk.CHUNK_SIZE, Y * Chunk.CHUNK_SIZE);
        }

        public static int Distance(ChunkVector a, ChunkVector b) {
            return MathHelper.Max(Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));
        }

        public static bool operator ==(ChunkVector a, ChunkVector b) {
            return a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is ChunkVector) return ((ChunkVector)obj).X == this.X && ((ChunkVector)obj).Y == this.Y;
            return false;
        }

        public static bool operator !=(ChunkVector a, ChunkVector b) {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}