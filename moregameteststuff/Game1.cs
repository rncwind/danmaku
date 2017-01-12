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
        
        List<bullet> bulletList = new List<bullet>();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public bullet genericbullet = new bullet();
        public Game1() //constructor for the game, sets graphics things such as the height and width of the window, as well as where to load content from
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
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
            public enemy1(Microsoft.Xna.Framework.Content.ContentManager game)
            {
                game.Load<Texture2D>("Sprites/honk.jpg");
            }
        }

        //moves the bullet
        public void fire() {
            bullet newbullet = new bullet();
            newbullet.bulletpos = player.position;
            if (bulletList.Count < 20)
            {
                bulletList.Add(newbullet);
            }
        }

        //controlls and stuff
        void move() {
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
            if (state.IsKeyDown(Keys.Space))
            {
                fire();
            }
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

        public void movebullet()
        {
            foreach (bullet newbullet in bulletList)
            {
                newbullet.bulletpos.Y += 10;
                if (Vector2.Distance(newbullet.bulletpos, player.position) > 100)
                    newbullet.isVisible = false;
            }
            for (int i = 0; i < bulletList.Count; i++)
            {
                if(!bulletList[i].isVisible)
                {
                    bulletList.RemoveAt(i);
                    i--;
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
            player.position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 100, graphics.GraphicsDevice.Viewport.Height / 2 - 75);
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
            move();
            checkbounds();
            movebullet();
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
            spriteBatch.Draw(player.texture, player.position);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}