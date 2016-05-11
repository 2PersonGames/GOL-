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
    class LoadedMenu : MenuObject
    {
        public LoadedMenu(Texture2D customGameButton, Texture2D glidersGameButton, Texture2D block1, Texture2D block2,
                          Texture2D block3, Texture2D leaveButton, Rectangle viewportRect, float newScale)
        {
            scale = newScale;

            //Instantiate buttons
            menuButtons = new Objects.GameObject[6];
            menuButtons[0] = new Objects.GameObject(customGameButton, new Vector2(viewportRect.Width * 0.25f, viewportRect.Height * 0.15f), scale * 0.80f);
            menuButtons[0].Alive = true;
            menuButtons[1] = new Objects.GameObject(glidersGameButton, new Vector2(viewportRect.Width * 0.25f, viewportRect.Height * 0.25f), scale * 0.80f);
            menuButtons[2] = new Objects.GameObject(block1, new Vector2(viewportRect.Width * 0.25f, viewportRect.Height * 0.35f), scale * 0.80f);
            menuButtons[3] = new Objects.GameObject(block2, new Vector2(viewportRect.Width * 0.25f, viewportRect.Height * 0.45f), scale * 0.80f);
            menuButtons[4] = new Objects.GameObject(block3, new Vector2(viewportRect.Width * 0.25f, viewportRect.Height * 0.55f), scale * 0.80f);
            menuButtons[5] = new Objects.GameObject(leaveButton, new Vector2(viewportRect.Width * 0.25f, viewportRect.Height * 0.83f), scale * 0.80f);

            alive = false;
            currentButton = 0;
        }
    }
}
