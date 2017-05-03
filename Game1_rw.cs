using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace mgtsrw
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1_rw : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<enemy1> enemy1list = new List<enemy1>();
        List<bullet> bulletlist = new List<bullet>();

        ship player;
        bgsprite background;

        Texture2D e1texture, playertex, bullettex, bosstex;


        public Game1_rw()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 1000;
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
            //called before game loop for 1 time init work, idk if i'll use it
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //misc and momogame stuff
            int screenheightpass;
            int screenwidth;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //textures
            e1texture = Content.Load<Texture2D>("Sprites/honk");
            bullettex = Content.Load<Texture2D>("bulletn");
            playertex = Content.Load<Texture2D>("Sprites/ikaruga");
            bosstex = Content.Load<Texture2D>("Sprites/cacodeamon");
            Texture2D bgtex = Content.Load<Texture2D>("Sprites/spacetex");

            //background init
            background = new bgsprite(bgtex, Vector2.Zero);
            background.initbg(screenheightpass = Window.ClientBounds.Height, screenwidth = Window.ClientBounds.Width);

            //player init
            player = new ship(playertex, Vector2.Zero);
            player.position = new Vector2((graphics.PreferredBackBufferWidth - player.texture.Width) / 2, (graphics.PreferredBackBufferHeight / 2));

            //debug enemy
            enemy1list.Add(new enemy1(e1texture, Vector2.Zero));
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.G))
                enemy1list.Add(new enemy1(e1texture, Vector2.Zero));
            // TODO: Add your update logic here
            base.Update(gameTime);
            player.move();
            if (enemy1list.Count > 0 && bulletlist.Count > 0)
                enemy1list = enemy1list[enemy1list.Count -1].destroyenemy(enemy1list, bulletlist);
            player.addbullet(bulletlist, player.position, bullettex);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            background.Draw(spriteBatch);
            for (int i = 0; i < enemy1list.Count; i++)
                enemy1list[i].Draw(spriteBatch);
            for (int i = 0; i < bulletlist.Count; i++)
            {
                bulletlist[i].Draw(spriteBatch);
                bulletlist[i].movebullet(bulletlist);
            }
            player.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
