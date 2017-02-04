using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace moregameteststuff
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public int bulletcounter = 0;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public List<enemy1> enemy1list = new List<enemy1>();
        public List<bullet> bulletlist = new List<bullet>();
        KeyboardState oldstate;
        public bullet genericbullet = new bullet();
        public Game1() //constructor for the game, sets graphics things such as the height and width of the window, as well as where to load content from
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 1000;
        }

        //class for all bullets
        public class bullet
        {
            public Texture2D bullettex;
            public Vector2 bulletpos;
            public bool isVisible;
        }

        //class for the player ship
        public class ship
        {
            public Texture2D texture;
            public Vector2 position;
            public int lives = 3;
        }

        public class enemy1
        {
            public Texture2D texture;
            public Vector2 Position;
            public int health;
        }

        public void spawnenemy()
        {
            Random rnd = new Random();
                enemy1list.Add(new enemy1());
                enemy1list[enemy1list.Count - 1].texture = Content.Load<Texture2D>("Sprites/honk");
                enemy1list[enemy1list.Count - 1].Position.X = rnd.Next(130, 360);
                enemy1list[enemy1list.Count - 1].Position.Y = 0;
                enemy1list[enemy1list.Count - 1].health = 50;
        }


        public void initbullets()
        {
            int i = 0;
            for (i = 0; i < bulletlist.Count; i++)
            {
                bulletlist[i] = genericbullet = new bullet();
                bulletlist[i].bullettex = Content.Load<Texture2D>("Sprites/bullet");
                bulletlist[i].bulletpos.X = 0;
                bulletlist[i].bulletpos.Y = 0;
            }
        }




        //moves the bullet
        public void fire() {
            bulletlist.Add(new bullet());
            bulletlist[bulletlist.Count -1].bulletpos.Y = player.position.Y;
            bulletlist[bulletlist.Count -1].bulletpos.X = player.position.X;
            bulletlist[bulletlist.Count - 1].bullettex = Content.Load<Texture2D>("Sprites/bullet");
        }

        //controlls and stuff
        void move(GameTime gameTime) {
            // i love polling
            KeyboardState state = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (state.IsKeyDown(Keys.D))
            {
                player.position.X += 5;
            }
            if (state.IsKeyDown(Keys.A))
            {
                player.position.X -= 5;
            }
            if (state.IsKeyDown(Keys.W))
            {
                player.position.Y -= 5;
            }
            if (state.IsKeyDown(Keys.S))
            {
                player.position.Y += 5;
            }
            if (state.IsKeyDown(Keys.Space) && oldstate.IsKeyUp(Keys.Space))
            {
                fire();
            }
            //test
            //the following inputs are debug commands for testing
            if (state.IsKeyDown(Keys.NumPad0))
            {
                spawnenemy();
            }
            if (state.IsKeyDown(Keys.H))
            {
                player.lives = player.lives - 1;
            }
            bullettrash();
        }

        //bounds checking so the player cant leave the screen
        public void checkbounds() {
            if (player.position.X > Window.ClientBounds.Width - player.texture.Width)
            {
                player.position.X = Window.ClientBounds.Width - player.texture.Width;
            }
            if (player.position.Y > Window.ClientBounds.Height - player.texture.Height)
            {
                player.position.Y = Window.ClientBounds.Height - player.texture.Height;
            }

            if (player.position.X <0)
            {
                player.position.X = 0;
            }
            if (player.position.Y < 0)
            {
                player.position.Y = 0;
            }
        }

        public void bullettrash()
        {
            foreach (bullet bullet in bulletlist.ToArray())
            {
                for (int i = 0; i < bulletlist.Count; i++)
                {
                    if (bulletlist[i].bulletpos.Y < -600)
                    {
                        bulletlist.RemoveAt(i);
                    }
                }
            }
        }

        public void movebullet()
        {
           foreach (bullet bullet in bulletlist)
            {
                for (int i = 0; i < bulletlist.Count; i++)
                {
                    bulletlist[i].bulletpos.Y -= 10;
                }
            }
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
        


        public ship player = new ship(); //creates the player

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
            base.Initialize();
            enemy1list.Add(new enemy1());
            enemy1list[enemy1list.Count -1].texture = Content.Load<Texture2D>("Sprites/honk");
            bulletlist.Add(new bullet());
            bulletlist[bulletlist.Count -1].bullettex = Content.Load<Texture2D>("Sprites/bullet");
            player.position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            oldstate = Keyboard.GetState();
        }

        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.texture = Content.Load<Texture2D>("Sprites/ikaruga");
            
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            move(gameTime);
            checkbounds();
            movebullet();
            //bullettrash();
            Draw(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            //initbullets();
            spriteBatch.Draw(player.texture, player.position);
            spriteBatch.Draw(enemy1list[0].texture, enemy1list[0].Position);
            foreach (bullet bullet in bulletlist)
            {
                for (int i=0; i < bulletlist.Count; i++)
                {
                    spriteBatch.Draw(bulletlist[i].bullettex, bulletlist[i].bulletpos);
                }
            }
            foreach (enemy1 enemy1 in enemy1list)
            {
                for (int i=0; i < enemy1list.Count; i++)
                {
                    spriteBatch.Draw(enemy1list[i].texture, enemy1list[i].Position);
                }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}