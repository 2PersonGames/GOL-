using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace GOL.Objects
{
    class GameObject
    {
        //Position & movement variables
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public Vector2 Centre
        {
            get { return new Vector2(sprite.Width / 2f, sprite.Height / 2f); }
        }

        //Drawing variables
        protected float scale;
        public float Scale
        {
            get { return scale; }
            set 
            {
                scale = value; 
            }
        }
        protected Texture2D sprite;
        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        protected bool alive;
        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
        protected float rotation;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        //Sprite variables
        public float GetSpriteHeight()
        {
            return (float)sprite.Height * scale;
        }
        public float GetSpriteWidth()
        {
            return (float)sprite.Width * scale;
        }

        //Methods for instantiation
        public GameObject(Vector2 newPosition, float newScale)
        {
            position = newPosition;
            scale = newScale;

            //Automatically set values
            velocity = Vector2.Zero;
            alive = false;
            rotation = 0f;
        }

        public GameObject(Texture2D newSprite, Vector2 newPosition, float newScale)
        {
            sprite = newSprite;
            position = newPosition;
            scale = newScale;

            //Automatically set values
            velocity = Vector2.Zero;
            alive = false;
            rotation = 0f;
        }

        protected virtual void Kill()
        {
            Alive = false;
        }

        void UpdatePosition()
        {
            velocity.X = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 5f;
            velocity.Y = -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * 5f;
        }

        void CheckBoundaryCollision(Rectangle viewportRect)
        {
            int MaxX = viewportRect.Width + viewportRect.X - (int)(Centre.X * 3f);
            int MinX = viewportRect.X + (int)Centre.X;
            int MaxY = viewportRect.Height + viewportRect.Y - (int)(Centre.Y * 3f);
            int MinY = viewportRect.Y + (int)Centre.Y;

            if (position.X > MaxX)
            {
                velocity.X *= -1;
                position.X = MaxX;
            }
            else if (position.X < MinX)
            {
                velocity.X *= -1;
                position.X = MinX;
            }

            if (position.Y > MaxY)
            {
                velocity.Y *= -1;
                position.Y = MaxY;
            }
            else if (position.Y < MinY)
            {
                velocity.Y *= -1;
                position.Y = MinY;
            }
        }

        public void Update(Rectangle viewportRect)
        {
            UpdatePosition();
            Position += Velocity;
            CheckBoundaryCollision(viewportRect);
        }
    }
}
