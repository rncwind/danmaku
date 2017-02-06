using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace moregameteststuff
{
    public class Game1 : Game
    {
        public Microsoft.Xna.Framework.Content.ContentManager content;
        public ship player = new ship(); //creates the player
        public int bulletcounter = 0;
        public byte currentgun = 0;
        public int firerate = 0;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public List<enemy1> enemy1list = new List<enemy1>();
        public List<bullet> bulletlist = new List<bullet>();
        KeyboardState oldstate;
        public double timesinceshot;
        public bullet genericbullet = new bullet();
        public powerup currentweapon = new powerup();
        public Game1() //constructor for the game, sets graphics things such as the height and width of the window, as well as where to load content from
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 1000;
        }

        public class gameobject
        {

            public Texture2D texture;
            public Vector2 position;
            public Rectangle hitbox
            {
                get
                {
                    return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, position);
                spriteBatch.End();
            }
        }

        //class for all bullets
        public class bullet : gameobject
        {
            //wow, this class is boring lol
        }

        //class for the player ship
        public class ship : gameobject
        {
            public int lives = 3;
        }

        public class enemy1 : gameobject
        {
            public int health = 20;
        }

        public class powerup : gameobject
        {
            public int weaponvelocity = 10;
            public int firerate = 250;
            public int lifechange = 0;
            public int damagebuff = 5;
        }

        public void spawnenemy()
        {
            Random rnd = new Random();
            enemy1list.Add(new enemy1());
            enemy1list[enemy1list.Count - 1].texture = Content.Load<Texture2D>("Sprites/honk");
            enemy1list[enemy1list.Count - 1].position.X = rnd.Next(0, 360);
            enemy1list[enemy1list.Count - 1].position.Y = 0;
            enemy1list[enemy1list.Count - 1].health = 50;
            enemy1list[enemy1list.Count - 1].Draw(spriteBatch);
        }


        public void initbullets()
        {
            int i = 0;
            for (i = 0; i < bulletlist.Count; i++)
            {
                bulletlist[i] = genericbullet = new bullet();
                bulletlist[i].texture = Content.Load<Texture2D>("Sprites/bullet");
                bulletlist[i].position.X = 0;
                bulletlist[i].position.Y = 0;
            }
        }




        //moves the bullet
        public void fire(GameTime gameTime)
        {
            bulletlist.Add(new bullet());
            bulletlist[bulletlist.Count - 1].position.Y = player.position.Y;
            bulletlist[bulletlist.Count - 1].position.X = player.position.X;
            bulletlist[bulletlist.Count - 1].texture = Content.Load<Texture2D>("Sprites/bullet");
        }

        //controlls and stuff
        void move(GameTime gameTime)
        {
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
                if (timesinceshot > currentweapon.firerate)
                {
                    fire(gameTime);
                    timesinceshot = 0;
                }
            }
            bullettrash();

            //the following inputs are debug commands for testing
            if (state.IsKeyDown(Keys.NumPad0)) //spawns new enemy
            {
                spawnenemy();
            }
            if (state.IsKeyDown(Keys.H)) //removes life from player
            {
                player.lives = player.lives - 1;
            }
            if (state.IsKeyDown(Keys.NumPad1)) //debug cheat rapid fire key
            {
                timesinceshot = (currentweapon.firerate + 1);
            }
        }

        //bounds checking so the player cant leave the screen
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

        public void bullettrash()
        {
            foreach (bullet bullet in bulletlist.ToArray())
            {
                for (int i = 0; i < bulletlist.Count; i++)
                {
                    if (bulletlist[i].position.Y < -600)
                    {
                        bulletlist.RemoveAt(i);
                    }
                }
            }
        }

        public void movebullet()
        {
            foreach (bullet bullet in bulletlist.ToArray())
            {
                for (int i = 0; i < bulletlist.Count; i++)
                {
                    bulletlist[i].position.Y -= currentweapon.weaponvelocity;
                }
            }
        }


        /// init logic, monogame's default comments are big and boring.
        /// Calling base.Initialize will enumerate through any components, and initialize them as well.

        protected override void Initialize()
        {
            base.Initialize();
            enemy1list.Add(new enemy1());
            enemy1list[enemy1list.Count - 1].texture = Content.Load<Texture2D>("Sprites/honk");
            bulletlist.Add(new bullet());
            bulletlist[bulletlist.Count - 1].texture = Content.Load<Texture2D>("Sprites/bullet");
            player.position = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);
            oldstate = Keyboard.GetState();
        }

        /// Loads game content
        protected override void LoadContent() //personally i found this override kind of useless as texturing on the fly was easier
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.texture = Content.Load<Texture2D>("Sprites/ikaruga");

        }

        /// UnloadContent will be called once per game and is the place to unload game-specific content.
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            move(gameTime);
            checkbounds();
            movebullet();
            timesinceshot += gameTime.ElapsedGameTime.TotalMilliseconds;
            Draw(gameTime);

            if (player.lives <= 0)
            {
                gameover();
            }
            base.Update(gameTime);
        }

        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            player.Draw(spriteBatch);
            enemy1list[0].Draw(spriteBatch);
            foreach (bullet bullet in bulletlist)
            {
                for (int i = 0; i < bulletlist.Count; i++)
                {
                    bulletlist[i].Draw(spriteBatch);
                }
            }


            foreach (enemy1 enemy1 in enemy1list)
            {
                for (int i = 0; i < enemy1list.Count; i++)
                {
                    enemy1list[i].Draw(spriteBatch);
                }
            }
            base.Draw(gameTime);
        }

        public void gameover()
        {
            Debug.WriteLine("Player lost");
            Exit();
        }
    }
    /*Blizzard "Bugs" (they are features i swear)
    * bullet velocity increases as bullets are on the screen. i kind of like this as it allows for "inverse slowdown" for more skillful players
    * 
    */
}

