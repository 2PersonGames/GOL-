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

namespace GOL.Menus
{
    class MenuObject        
    {
        protected Objects.GameObject[] menuButtons;
        public Objects.GameObject[] MenuButtons
        {
            get { return menuButtons; }
            set { menuButtons = value; }
        }

        protected int currentButton;
        public int CurrentButton
        {
            get { return currentButton; }
            set { currentButton = value; }
        }
        protected float scale;

        protected bool alive;
        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        public MenuObject()
        {
        }

        //Works on all menus using linear buttons
        public int UpdateLinearMenu(GamePadState previousGamePadState)
        {
            int maxButtonSelection = menuButtons.Length - 1;

            if (previousGamePadState.ThumbSticks.Left.Y == 0)
            {
                //Update menu buttons
                if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > 0 || Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    //Button selection goes up
                    menuButtons[currentButton].Alive = false;
                    currentButton--;
                    if (currentButton < 0)
                    {
                        currentButton = maxButtonSelection;
                    }
                    menuButtons[currentButton].Alive = true;
                }
                else if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < 0 || Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    //Button selection goes down
                    menuButtons[currentButton].Alive = false;
                    currentButton++;
                    if (currentButton > maxButtonSelection)
                    {
                        currentButton = 0;
                    }
                    menuButtons[currentButton].Alive = true;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) && previousGamePadState.Buttons.A == ButtonState.Released || Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                return currentButton;
            }
            else
            {
                return 99;
            }
        }

        //Draws all buttons, highlighting active
        //button in red
        public void DrawMenu(SpriteBatch spriteBatch)
        {
            foreach (Objects.GameObject button in menuButtons)
            {
                if (button.Alive)
                {
                    spriteBatch.Draw(button.Sprite, button.Position, null, Color.Black, button.Rotation, button.Centre, button.Scale * 1.1f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(button.Sprite, button.Position, null, Color.DarkGray, button.Rotation, button.Centre, button.Scale, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }
    }
}
