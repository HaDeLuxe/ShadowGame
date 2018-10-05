using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowGame
{
     class  GameObject
    {
        public GameObject(Vector2 gameObjectPosition, Vector2 gameObjectSize, float gameObjectRotation, SpriteEffects gameObjectSpriteEffect, Color gameObjectColor, Texture2D gameObjectTexture)
        {
            this.gameObjectPosition = gameObjectPosition;
            this.gameObjectSize = gameObjectSize;
            this.gameObjectRotation = gameObjectRotation;
            this.gameObjectSpriteEffect = gameObjectSpriteEffect;
            this.gameObjectColor = gameObjectColor;
            this.gameObjectTexture = gameObjectTexture;
        }

        public Vector2 gameObjectPosition = new Vector2(0,0);
        public Vector2 gameObjectSize { get; set; } = new Vector2(1, 1);
        private float gameObjectRotation { get; set; } = 0f;

        SpriteEffects gameObjectSpriteEffect { get; set; } = SpriteEffects.None;

        Color gameObjectColor { get; set; } = Color.White;

        Texture2D gameObjectTexture { get; set; } = null;

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameObjectTexture, gameObjectPosition, null, gameObjectColor, gameObjectRotation, Vector2.Zero, gameObjectSize, gameObjectSpriteEffect, 0);
        }
    }
}
