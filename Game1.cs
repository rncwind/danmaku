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
        List<enemy2> e2list = new List<enemy2>();
        public KeyboardState oldstate;

        public double timesinceshot;
        double timesinceenemyspawn;
        bool l1bossfight = false;

        GUI hud;
        ship player;
        bullet bullet;
        bgsprite background;
        powerup currentweapon = new powerup(texture: null, position: Vector2.Zero);
        boss1 l1boss;
        
        Texture2D enemy2tex;
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
            Texture2D bullettex = Content.Load<Texture2D>("Sprites/bullet");
            bulletlist.Add(new bullet(bullettex, bulletpos));
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
                patterning();
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
            enemy1list = enemy1list.Except(collides).ToList();
        }

        /// init logic, monogame's default comments are big and boring.
        /// Calling base.Initialize will enumerate through any components, and initialize them as well.

        protected override void Initialize()
        {
            base.Initialize();
            int screenheightpass = Window.ClientBounds.Height;
            int screenwidth = Window.ClientBounds.Width;
            player.position = new Vector2((Window.ClientBounds.Width - player.texture.Width) / 2, Window.ClientBounds.Height);
            Debug.WriteLine("Width: " + Window.ClientBounds.Width.ToString());
            Debug.WriteLine("Height: " + Window.ClientBounds.Height.ToString());
            background.initbg(screenheightpass, screenwidth);
            oldstate = Keyboard.GetState();
            spawnenemy();
        }

        /// Loads game content
        protected override void LoadContent() //personally i found this override kind of useless as texturing on the fly was easier
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D playertex = Content.Load<Texture2D>("Sprites/ikaruga");
            Texture2D bullettex = Content.Load<Texture2D>("Sprites/bullet");
            Texture2D bgtex = Content.Load<Texture2D>("Sprites/spacetex");
            Texture2D enemy2tex = Content.Load<Texture2D>("Sprites/cacodeamon");

            SpriteFont spriteFont = Content.Load<SpriteFont>("Arial");
            player = new ship(playertex, Vector2.Zero);
            bullet = new bullet(bullettex, Vector2.Zero);
            background = new bgsprite(bgtex, Vector2.Zero);
            hud = new GUI();
            int scorepass = player.score;
            hud.Draw(spriteFont, spriteBatch, player);
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
            timesinceenemyspawn += gameTime.ElapsedGameTime.TotalMilliseconds;

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
            base.Update(gameTime);
        }

        /// This is called when the game should draw itself.
        protected override void Draw(GameTime gameTime)
        {
            SpriteFont spriteFont = Content.Load<SpriteFont>("Arial");
            GraphicsDevice.Clear(Color.CornflowerBlue);
            background.Draw(spriteBatch);
            player.Draw(spriteBatch);
            hud.Draw(spriteFont, spriteBatch, player);
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

            foreach (enemy2 enemy2 in e2list)
            {
                for (int i = 0; i < enemy1list.Count; i++)
                {
                    enemy1list[i].Draw(spriteBatch);
                }
            }

            if (l1bossfight == true)
            {
                l1boss.Draw(spriteBatch);
                if (l1boss.defeated == true)
                {
                    clearlevel();
                }
            }
            base.Draw(gameTime);
        }


        //LEVEL 2 BEGINS HERE
        public void patterning()
        {
            pattern patterninst = new pattern();
            Vector2[] vectorarr = new Vector2[10];
            char patternchar = 'v';
            vectorarr = patterninst.getpattern(patternchar);
            patternspawn(patternchar, vectorarr);
        }

        public void patternspawn(char patternchar, Vector2[] patternarr)
        {
            if (patternchar == 'v')
            {
                for (int i = 0; i <= 4; i++)
                    e2list.Add(new enemy2(Content.Load<Texture2D>("Sprites/cacodeamon"), patternarr[i]));
            }
            if (patternchar == 'x')
            {
                for (int i = 0; i <= 5; i++)
                    e2list.Add(new enemy2(Content.Load<Texture2D>("Sprites/cacodeamon"), patternarr[i]));
            }
        }



    public void gameover()
        {
            Debug.WriteLine("Player lost");
            highscore();
            Exit();
        }

        public void clearlevel()
        {
            enemy1list.Clear();
            bulletlist.Clear();
        }
    }
    /*Blizzard "Bugs" (they are features i swear)
    * bullet velocity increases as bullets are on the screen. This is due to the way draws are updated, i could fix this but it would be an inefficient bodge
    * 
    */
}

