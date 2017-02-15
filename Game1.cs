﻿using Microsoft.Xna.Framework;
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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public List<enemy1> enemy1list = new List<enemy1>();
        public List<bullet> bulletlist = new List<bullet>();
        public KeyboardState oldstate;
        public double timesinceshot;
        ship player;
        bullet bullet;
        bgsprite background;
        powerup currentweapon = new powerup(texture: null, position: Vector2.Zero);
        public Game1() //constructor for the game, sets graphics things such as the height and width of the window, as well as where to load content from
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 1000;
        }

        public void spawnenemy()
        {
            Random rnd = new Random();
            Texture2D enemytex = Content.Load<Texture2D>("Sprites/honk");
            Vector2 enemypos;
            enemypos.Y = rnd.Next(0, (Window.ClientBounds.Height - enemytex.Height) / 2);
            enemypos.X = rnd.Next(0, (Window.ClientBounds.Width - enemytex.Width));
            enemy1list.Add(new enemy1(enemytex, enemypos));
            enemy1list[enemy1list.Count - 1].Draw(spriteBatch);
        }

        //moves the bullet
        public void fire(GameTime gameTime)
        {
            Texture2D bullettex = Content.Load<Texture2D>("Sprites/bullet");
            bulletlist.Add(new bullet(bullettex, player.position));
        }

        //performs actions other than moving
        void actions(GameTime gameTime)
        {
            // i love polling
            KeyboardState state = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (state.IsKeyDown(Keys.Space) && oldstate.IsKeyUp(Keys.Space))
            {
                if (timesinceshot > currentweapon.firerate)
                {
                    fire(gameTime);
                    timesinceshot = 0;
                }
            }
            bullet.bullettrash(bulletlist);

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

        public void checkbulletcollision()
        {
            int i = 0, it = 0;
            while (i < (bulletlist.Count))
            {
                while (it < (enemy1list.Count))
                {
                    if (enemy1list[it].hitbox.Intersects(bulletlist[i].hitbox))
                    {
                        enemy1list[it].hit = true;
                        killenemy();
                    }
                    it++;
                }
                i++;
            }
        }

        public void killenemy()
        {
            enemy1[] e1arr;
            e1arr = enemy1list.ToArray();
            int i = 0;
            foreach (enemy1 enemy in e1arr)
            {
                if (e1arr[i].hit)
                    enemy1list.RemoveAt(i);
                i++;
            }
        }
        
        /// init logic, monogame's default comments are big and boring.
        /// Calling base.Initialize will enumerate through any components, and initialize them as well.

        protected override void Initialize()
        {
            base.Initialize();
            /*
            enemy1list.Add(new enemy1());
            enemy1list[enemy1list.Count - 1].texture = Content.Load<Texture2D>("Sprites/honk");
            bulletlist.Add(new bullet());
            bulletlist[bulletlist.Count - 1].texture = Content.Load<Texture2D>("Sprites/bullet");
            */
            int screenheightpass = Window.ClientBounds.Height;
            int screenwidth = Window.ClientBounds.Width;
            player.position = new Vector2((Window.ClientBounds.Width - player.texture.Width) / 2, Window.ClientBounds.Height);
            Debug.WriteLine("Width: " + Window.ClientBounds.Width.ToString());
            Debug.WriteLine("Height: " + Window.ClientBounds.Height.ToString());
            background.initbg(screenheightpass, screenwidth);
            oldstate = Keyboard.GetState();
        }

        /// Loads game content
        protected override void LoadContent() //personally i found this override kind of useless as texturing on the fly was easier
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D playertex = Content.Load<Texture2D>("Sprites/ikaruga");
            Texture2D bullettex = Content.Load<Texture2D>("Sprites/bullet");
            Texture2D bgtex = Content.Load<Texture2D>("Sprites/spacetex");
            Vector2 bg2pos;
            player = new ship(playertex, Vector2.Zero);
            bullet = new bullet(bullettex, Vector2.Zero);
            background = new bgsprite(bgtex, Vector2.Zero);
            bg2pos.Y = (background.position.Y - background.texture.Height);
            bg2pos.X = 0;
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
            player.move(gameTime);
            actions(gameTime);
            checkbounds();
            checkbulletcollision();
            background.bgloop();
            bullet.movebullet(bulletlist, currentweapon);
            Draw(gameTime);
            timesinceshot += gameTime.ElapsedGameTime.TotalMilliseconds;
            

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
            background.Draw(spriteBatch);
            player.Draw(spriteBatch);
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
    * bullet velocity increases as bullets are on the screen. This is due to the way draws are updated, i could fix this but it would be an inefficient bodge
    * 
    */
}