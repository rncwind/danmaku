using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace mgtsrw
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1_rw : Game
    {
        GraphicsDeviceManager graphics;SpriteBatch spriteBatch;

        List<enemy> enemylist = new List<enemy>();
        List<bullet> bulletlist = new List<bullet>();

        Random rngh = new Random(1000);
        Random rngw = new Random(720);

        ship player;
        bgsprite background;

        Texture2D e1texture, playertex, bullettex, bosstex, bgtex;

        double enemydelta = 0, bulletdelta =0;
        int initlvscore = 0;

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

            //player init
            playertex = Content.Load<Texture2D>("Sprites/ikaruga");
            player = new ship(playertex, Vector2.Zero);
            player.position = new Vector2((graphics.PreferredBackBufferWidth - player.texture.Width) / 2, (graphics.PreferredBackBufferHeight / 2));

            //textures
            if (player.level == 1)
            {
                initlvscore = player.score;
                e1texture = Content.Load<Texture2D>("Sprites/honk");
                bullettex = Content.Load<Texture2D>("bulletn");
                bosstex = Content.Load<Texture2D>("Sprites/cacodeamon");
                bgtex = Content.Load<Texture2D>("Sprites/spacetex");
            }

            //background init
            background = new bgsprite(bgtex, Vector2.Zero);
            background.initbg(screenheightpass = Window.ClientBounds.Height, screenwidth = Window.ClientBounds.Width);

            

            //debug enemy
            enemylist.Add(new enemy(e1texture, Vector2.Zero, player));
        }

        public void levelswitch()
        {
            if (player.level == 2)
            {
                enemylist.Clear();
                bulletlist.Clear();
                e1texture = Content.Load<Texture2D>("Sprites/honk");
                bullettex = Content.Load<Texture2D>("Sprites/honk");
                bosstex = Content.Load<Texture2D>("Sprites/honk");
                bgtex = Content.Load<Texture2D>("Sprites/ikaruga");
                background = new bgsprite(bgtex, Vector2.Zero);
            }
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
            if (Keyboard.GetState().IsKeyDown(Keys.C)) //debug function, remove in prod
            {
                Vector2 enemypos = new Vector2(rngw.Next(0,(Window.ClientBounds.Width)), rngh.Next(0, (Window.ClientBounds.Height)));
                enemylist.Add(new enemy(e1texture, enemypos, player));
                player.level = 2;
                levelswitch();
            }
            Debug.Write(player.score);
            // TODO: Add your update logic here
            levelswitch();
            if (enemylist.Count > 0 && bulletlist.Count > 0)
                enemylist = enemylist[enemylist.Count - 1].destroyenemy(enemylist, bulletlist, player);
            player.move();
            base.Update(gameTime);
            enemydelta += gameTime.ElapsedGameTime.TotalMilliseconds;
            bulletdelta += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (bulletdelta >= 200 && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                bulletdelta = 0;
                player.addbullet(bulletlist, player.position, bullettex);
            }
            if (enemydelta >= 1000)
            {
                enemydelta = 0;
                Vector2 enemypos = new Vector2(rngw.Next(0, (Window.ClientBounds.Width - e1texture.Width)), rngh.Next(0, (Window.ClientBounds.Height/2)-e1texture.Height));
                enemylist.Add(new enemy(e1texture, enemypos, player));
            }
            if (player.score == (initlvscore + 1000))
            {
                player.level++;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            background.Draw(spriteBatch);
            foreach (enemy enemy in enemylist)
                enemy.Draw(spriteBatch);
            foreach (bullet bullet in bulletlist)
            {
                bullet.movebullet(bulletlist);
                bullet.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}