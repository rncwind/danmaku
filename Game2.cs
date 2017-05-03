using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace moregameteststuff
{
    /// This is the main type for your game.
    public class Game2 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        bool newpatternspawn = false;

        List<enemy2> e2list = new List<enemy2>();

        ship player;
        bgsprite background;
        public Game2()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 1000;
        }

        public void patterning()
        {
            newpatternspawn = true;
            pattern patterninst = new pattern();
            Vector2[] vectorarr = new Vector2[10];
            char patternchar = 'v';
            vectorarr = patterninst.getpattern(patternchar);
            patternspawn(patternchar, vectorarr);
        }

        public void patternspawn(char patternchar, Vector2[] vectorarr)
        {
            if (patternchar == 'v')
            {
                for (int i = 0; i <= 4; i++)
                {
                    e2list.Add(new enemy2(Content.Load<Texture2D>("Sprites/cacodeamon"), vectorarr[i]));
                }
            }
            if (patternchar == 'x')
            {
                for (int i = 0; i <= 5; i++)
                    e2list.Add(new enemy2(Content.Load<Texture2D>("Sprites/cacodeamon"), vectorarr[i]));
            }
        }

        public void checkbounds()
        {
            if (player.position.X > Window.ClientBounds.Width - player.texture.Width)
            {
                player.position.X = Window.ClientBounds.Width - player.texture.Width;
            }
            if (player.position.Y > Window.ClientBounds.Height - player.texture.Height)
            {
                player.position.Y = Window.ClientBounds.Height - player.texture.Height;
            }

            if (player.position.X < 0)
            {
                player.position.X = 0;
            }
            if (player.position.Y < 0)
            {
                player.position.Y = 0;
            }
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
            player = new ship(Content.Load<Texture2D>("Sprites/ikaruga"), Vector2.Zero);
            background = new bgsprite(Content.Load<Texture2D>("Sprites/spacetex"), Vector2.Zero);
            int screenheightpass = Window.ClientBounds.Height;
            int screenwidth = Window.ClientBounds.Width;
            background.initbg(screenheightpass, screenwidth);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
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
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (newpatternspawn == true)
            {
                foreach (enemy2 enemy2 in e2list)
                {
                    for (int i = 0; i < e2list.Count; i++)
                    {
                        e2list[i].Draw(spriteBatch);
                    }
                }
            }
            background.Draw(spriteBatch);
            player.move();
            checkbounds();
            player.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
