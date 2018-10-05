using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ShadowGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameObject tile = null;
        GameObject player = null;
        public static int count = 0;
        SpriteFont font;
        private Dictionary<string, Texture2D> texturesDictionary = null;
        
        private Color[] rectColor;
        private Color[,] TwoDColor;
        private bool[,] isLit;
        private int[,] map;
        private Texture2D background;
        private float alpha;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 1080;
            graphics.PreferredBackBufferWidth = 1920;
        
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            texturesDictionary = new Dictionary<string, Texture2D>();
            isLit = new bool[graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2];
            map = new int[graphics.PreferredBackBufferWidth/2, graphics.PreferredBackBufferHeight/2];

           

            //rectColor = new Color[graphics.PreferredBackBufferHeight * graphics.PreferredBackBufferWidth];
            rectColor = new Color[graphics.PreferredBackBufferWidth * graphics.PreferredBackBufferHeight];
            TwoDColor = new Color[graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight];
            background = new Texture2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            for (int i = 0; i < rectColor.Length; i++)
            {
                rectColor[i] = Color.LightSalmon;
            }
            background.SetData(rectColor);
            alpha = 1;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texturesDictionary.Add("GameObject", Content.Load<Texture2D>("Images\\SimpleTile_White"));
            font = Content.Load<SpriteFont>("fonts\\Arial");
            player = new GameObject(new Vector2(200, 200), new Vector2(10, 10), 0f, SpriteEffects.None, Color.Black, texturesDictionary["GameObject"]);
            tile = new GameObject(new Vector2(500,500), new Vector2(30,30), 0f, SpriteEffects.None, Color.Red, texturesDictionary["GameObject"]);
            for (int i = 0; i < 1920/2; i++)
            {
                for (int j = 0; j < 1080/2; j++)
                {
                    map[i, j] = 0;
                }
            }

            //map[500, 500] = 1;
            //map[500, 501] = 1;
            for (int i = 500/2; i < 530/2; i++)
            {
                for (int j = 500/2; j < 530/2; j++)
                {
                    map[i, j] = 1;
                }
            }
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            count = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                player.gameObjectPosition.Y -= 5;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                player.gameObjectPosition.Y += 5;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                player.gameObjectPosition.X -= 5;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                player.gameObjectPosition.X += 5;

            for (int i = 0; i < rectColor.Length; i++)
            {
                rectColor[i] = Color.Black;
            }
            for (int j = (int)player.gameObjectPosition.X/2 - 150; j < player.gameObjectPosition.X/2 + 150; j++)
                for (int k = (int)player.gameObjectPosition.Y/2 - 150; k < player.gameObjectPosition.Y/2 + 150; k++)
                {
                    if (j < 0)
                        j = 0;
                    if (j > graphics.PreferredBackBufferWidth/2)
                        j = graphics.PreferredBackBufferWidth/2;
                    if (k < 0)
                        k = 0;
                    if (k > graphics.PreferredBackBufferHeight/2)
                        k = graphics.PreferredBackBufferHeight/2;
                    isLit[j, k] = false;
                }
            ShadowCaster.ComputeFieldOfViewWithShadowCasting((int)player.gameObjectPosition.X/2, (int)player.gameObjectPosition.Y/2, 100, (x1, y1) => map[x1, y1] == 1, (x2, y2) => { isLit[x2, y2] = true; });

            for (int j = (int)player.gameObjectPosition.X/2 - 100; j < player.gameObjectPosition.X/2 + 100; j++)
                for (int k = (int)player.gameObjectPosition.Y/2 - 100; k < player.gameObjectPosition.Y/2 + 100; k++)
                {
                    if (isLit[j, k] == true)
                    {
                        TwoDColor[j*2, k*2] = Color.White;
                        rectColor[j*2  + k * 1920 * 2] = TwoDColor[j, k];
                    }
                }



            background.SetData(rectColor);

            base.Update(gameTime);
        }

        float calculateDistance(int x, int y)
        {
            return (float)Math.Sqrt((float)Math.Pow((float)x - (float)player.gameObjectPosition.X, 2) + Math.Pow((float)y - (float)player.gameObjectPosition.Y, 2));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            tile.draw(spriteBatch);
            player.draw(spriteBatch);

            spriteBatch.DrawString(font, count.ToString(), new Vector2(0, 0), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Height; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }
}
