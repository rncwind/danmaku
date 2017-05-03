using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Text;
using System.Linq;
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
        List<enemy1bullet> e1bulletlist = new List<enemy1bullet>();

        public KeyboardState oldstate;

        public double timesinceshot;
        double timesinceenemyspawn;
        bool l1bossfight = false,prevstatehit = false;

        Texture2D enemytex;

        GUI hud;
        ship player;
        bullet bullet;
        enemy1 enemy1dummy;
        bgsprite background;
        powerup currentweapon = new powerup(texture: null, position: Vector2.Zero);
        boss1 l1boss;


        public Game1() //constructor for the game, sets graphics things such as the height and width of the window, as well as where to load content from
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 1000;
        }

        public void spawnenemy()
        {
            //bool invalid = false;
            Random rnd = new Random();
            
            Vector2 enemypos;
            enemypos.Y = rnd.Next(0, (Window.ClientBounds.Height - enemy1dummy.texture.Height) / 2);
            enemypos.X = rnd.Next(0, (Window.ClientBounds.Width - enemy1dummy.texture.Width));
            enemy1dummy.position = enemypos;
            enemy1list.Add(new enemy1(enemy1dummy.texture, enemy1dummy.position));
            enemy1list[enemy1list.Count - 1].Draw(spriteBatch);


            /*
            int i = 0;
            for (i = 0; i < bulletlist.Count; ++i)
            {
                if (enemy1list[enemy1list.Count - 1].hitbox.Intersects(bulletlist[i].hitbox) == true)
                    invalid = true;
            }

            if (invalid == true)
            {
                enemy1list[enemy1list.Count - 1].position.X = rnd.Next(0, (Window.ClientBounds.Width - enemytex.Width));
                enemy1list[enemy1list.Count - 1].position.Y = rnd.Next(0, (Window.ClientBounds.Height - enemytex.Height) / 2);
            }
            */
        }

        

        public void spawnboss()
        {
            Texture2D bosstex = Content.Load<Texture2D>("Sprites/cacodeamon");
            l1boss = new boss1(bosstex, new Vector2(360, 500));
            l1boss.Draw(spriteBatch);
            l1bossfight = true;
        }

        //moves the bullet
        public void fire(GameTime gameTime)
        {
            Vector2 bulletpos;
            bulletpos = player.position;
            bulletpos.X += 10;
            Texture2D bullettex = Content.Load<Texture2D>("bulletn");
            bulletlist.Add(new bullet(bullettex, bulletpos));
        }

        public void enemyfire(GameTime gameTime)
        {
            spawne1bullet(enemy1list[0].position);
        }

        //performs actions other than moving
        void actions(GameTime gameTime)
        {
            // i love polling
            KeyboardState state = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Debug.WriteLine(player.score);
                Exit();
            }
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
            if (state.IsKeyDown(Keys.NumPad2))
                highscore();
            if (state.IsKeyDown(Keys.NumPad3))
                spawnboss();
            if (state.IsKeyDown(Keys.NumPad4))
            {
                readinscores();
                highscore();
            }
            if (state.IsKeyDown(Keys.NumPad5))
                enemyfire(gameTime);
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

        public void highscore()
        {
            Int32 timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string playername = Microsoft.VisualBasic.Interaction.InputBox("Name?", "Input player name", "");
            System.IO.StreamWriter file = new System.IO.StreamWriter("scores.csv", true);
            file.WriteLine(playername + ',' + player.score + ',' + timestamp.ToString());
            file.Close();
        }

        public void readinscores() //if became production code replace with db backend
        {
            string[] fields = null;
            using (Microsoft.VisualBasic.FileIO.TextFieldParser csvParser = new Microsoft.VisualBasic.FileIO.TextFieldParser("scores.csv"))
            {
                csvParser.ReadLine(); //skips first line with the headers
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                while (!csvParser.EndOfData)
                {
                    fields = csvParser.ReadFields();
                }
            }
        }

        public void checkbulletcollision() //WE'LL MAKE OUR OWN COLLISION DETECTION, WITH LINQ AND HOOKERS
        {
            List<enemy1> collides = null;

            if (enemy1list.Count > 0 && bulletlist.Count > 0)
            {
                int i = 0;
                for (i = 0; i < bulletlist.Count; ++i)
                {
                    collides = enemy1list.Where(x => x.hitbox.Intersects(bulletlist[i].hitbox)).ToList();
                }
                killenemy(collides);
            }
        }

        public void killenemy(List<enemy1> collides)
        {
            player.score += (collides.Count * 10);
            enemy1list = enemy1list.Except(collides).ToList();
            collides.Clear();
        }
        
        public void updateifs()
        {
            if (timesinceenemyspawn > 500)
            {
                spawnenemy();

                timesinceenemyspawn = 0;
            }

            if (player.lives <= 0)
            {
                gameover();
            }

            if (player.score > 2000 && l1bossfight == false)
            {
                spawnboss();
            }

            if (l1bossfight == true)
            {
                int direction = 0;
                if (l1boss.health <= 0)
                {
                    l1boss.defeated = true;
                }
                else
                {
                    Random rnd = new Random();
                    direction = rnd.Next(0, 4);
                    if (direction == 0)
                        l1boss.position.X += rnd.Next(0, 30);
                    if (direction == 1)
                        l1boss.position.X -= rnd.Next(0, 30);
                    if (direction == 2)
                        l1boss.position.Y += rnd.Next(0, 30);
                    if (direction == 3)
                        l1boss.position.Y -= rnd.Next(0, 30);
                }
            }
        }

        public void spawne1bullet(Vector2 passvec)
        {
            Texture2D e1bullettex = Content.Load<Texture2D>("Sprites/enemybullet");
            e1bulletlist.Add(new enemy1bullet(passvec, e1bullettex));
        }

        public void checke1bcollision()
        {
            for (int i = 0; i < e1bulletlist.Count; ++i)
            {
                if (e1bulletlist[i].hitbox.Intersects(player.hitbox) && prevstatehit == false)
                {
                    prevstatehit = true;
                    player.lives -= 1;
                }
                if (e1bulletlist[i].hitbox.Intersects(player.hitbox) == false)
                    prevstatehit = false;
            }
        }

        /// init logic, monogame's default comments are big and boring.
        /// Calling base.Initialize will enumerate through any components, and initialize them as well.

        protected override void Initialize()
        {
            base.Initialize();
            int screenheightpass = Window.ClientBounds.Height;
            int screenwidth = Window.ClientBounds.Width;
            player.position = new Vector2((Window.ClientBounds.Width - player.texture.Width) / 2, Window.ClientBounds.Height);
            background.initbg(screenheightpass, screenwidth);
            oldstate = Keyboard.GetState();
            spawnenemy();
        }

        /// Loads game content
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D playertex = Content.Load<Texture2D>("Sprites/ikaruga");
            Texture2D bullettex = Content.Load<Texture2D>("bulletn");
            Texture2D bgtex = Content.Load<Texture2D>("Sprites/spacetex");
            Texture2D enemy2tex = Content.Load<Texture2D>("Sprites/cacodeamon");
            Texture2D e1bullettex = Content.Load<Texture2D>("Sprites/enemybullet");
            Texture2D enemytex = Content.Load<Texture2D>("Sprites/honk");

            SpriteFont spriteFont = Content.Load<SpriteFont>("Arial");
            player = new ship(playertex, Vector2.Zero);
            bullet = new bullet(bullettex, Vector2.Zero);
            background = new bgsprite(bgtex, Vector2.Zero);
            hud = new GUI();
            enemy1dummy = new enemy1(enemytex, Vector2.Zero);
            int scorepass = player.score;
            hud.Draw(spriteFont, spriteBatch, player);
        }

        /// UnloadContent will be called once per game and is the place to unload game-specific content.
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
        protected override void Update(GameTime gameTime)
        {
            player.move();
            actions(gameTime);
            checkbounds();
            checkbulletcollision();
            checke1bcollision();
            bullet.movebullet(bulletlist, currentweapon);
            Draw(gameTime);

            timesinceshot += gameTime.ElapsedGameTime.TotalMilliseconds;
            timesinceenemyspawn += gameTime.ElapsedGameTime.TotalMilliseconds;

            updateifs();
            base.Update(gameTime);
        }

        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            SpriteFont spriteFont = Content.Load<SpriteFont>("Arial");
            background.Draw(spriteBatch);
            
            player.Draw(spriteBatch);
            hud.Draw(spriteFont, spriteBatch, player);


            foreach (enemy1bullet e1bullet in e1bulletlist)
            {
                for (int i = 0; i < e1bulletlist.Count; i++)
                {
                    e1bulletlist[i].Draw(spriteBatch);
                }
            }

            foreach (bullet bullet in bulletlist)
            {
                for (int i = 0; i < bulletlist.Count; i++)
                {
                    bulletlist[i].Draw(spriteBatch);
                }
            }

            foreach (enemy1 enemydummy in enemy1list)
            {
                for (int i = 0; i < enemy1list.Count; i++)
                {
                    enemy1list[i].Draw(spriteBatch);
                }
            }

            foreach (enemy1bullet enemy1bullet in e1bulletlist)
            {
                for (int i =0; i < e1bulletlist.Count; i++)
                {
                    e1bulletlist[i].move();
                }
            }

            if (l1bossfight == true)
            {
                l1boss.Draw(spriteBatch);
                if (l1boss.defeated == true)
                {
                    clearlevel();
                    using (var game2 = new Game2())
                        game2.Run();
                }
            }
            base.Draw(gameTime);
        }



    public void gameover()
        {
            Debug.WriteLine("Player lost");
            highscore();
            clearlevel();
            Exit();
        }

        public void clearlevel()
        {
            enemy1list.Clear();
            bulletlist.Clear();
            UnloadContent();
        }
    }
    /*Blizzard "Bugs" (they are features i swear)
    * bullet velocity increases as bullets are on the screen. This is due to the way draws are updated, i could fix this but it would be an inefficient bodge
    * 
    */
}

