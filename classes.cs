﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace moregameteststuff
{
    public class gameobject : Game
    {
        
        public Texture2D texture;
        public Vector2 position;
        public Rectangle hitbox //getter for creating and assigning a hitbox for new objects
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            }
        }

        public gameobject(Texture2D texture, Vector2 position) //constructor for gameobjects
        {
            this.position = position;
            this.texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch) //draw fuction that allows objects to draw themselves indipendently.
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, position);
            spriteBatch.End();
        }
    }

    public class ship : gameobject
    {
        public int lives = 3;
        public int score;
        public ship(Texture2D texture, Vector2 position) : base(texture, position) //constructor for the ship
        {
        }

        public void move() //function to allow for player movement
        {
            // i love polling
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.D))
            {
                this.position.X += 5;
            }
            if (state.IsKeyDown(Keys.A))
            {
                this.position.X -= 5;
            }
            if (state.IsKeyDown(Keys.W))
            {
                this.position.Y -= 5;
            }
            if (state.IsKeyDown(Keys.S))
            {
                this.position.Y += 5;
            }
        }
    }

    //class for all bullets
    public class bullet : gameobject
    {
        public bullet(Texture2D texture, Vector2 position) : base(texture, position)//constructor
        {
        }

        public void movebullet(List<bullet> bulletlist, powerup currentweapon)//function that moves all bullets that currently exist and are not disposed of
        {
            foreach (bullet bullet in bulletlist)
            {
                for (int i = 0; i < bulletlist.Count; ++i)
                {
                    bulletlist[i].position.Y -= currentweapon.weaponvelocity;
                }
            }
        }

        public void bullettrash(List<bullet> bulletlist)//disposes of bullets that are too far off the screen.
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
    }

    public class enemy1bullet : gameobject//class for default enemy bullets
    {
        public enemy1bullet(Vector2 position,Texture2D texture) : base(texture,position)//constructor
        {
        }

        public void move()//allows for bullet movement each tick
        {
            this.position.Y += 1;
        }

    }

    public class enemy1 : gameobject//default enemy
    {
        public int health = 20;
        public bool hit = false;

        public enemy1(Texture2D texture, Vector2 position) : base(texture, position)//constructor
        {
            
        }
    }

    public class enemy2 : gameobject
    {
        public int health = 50;
        public double timesincefired = 0;

        public enemy2(Texture2D texture, Vector2 position) : base(texture,position)
        {
        }
    }

    public class boss1 : gameobject
    {
        public bool defeated = false;
        public boss1(Texture2D texture, Vector2 position) : base(texture, position)
        {
        }
    
        public int health = 200;
    }

    public class powerup : gameobject
    {
        public int weaponvelocity = 10;
        public int firerate = 250;
        public int lifechange = 0;
        public int damagebuff = 5;

        public powerup(Texture2D texture, Vector2 position) : base(texture, position)
        {
        }
    }

    public class bgsprite : gameobject //the scrolling all works through some black magic.
    {
        private int screenheight;
        private Vector2 screenpos, origin, texsize;
        public bgsprite(Texture2D texture, Vector2 position) : base(texture,position)//constructor
        {
        }

        public void initbg(int screenheightpass, int screenwidth)//init operations that setup the bh
        {
            origin = new Vector2(texture.Width / 2, 0);
            screenpos = new Vector2(0, screenheight / 2);
            texsize = new Vector2(0, texture.Height);
            screenheight = screenheightpass;
        }

        new public void Draw(SpriteBatch spriteBatch)
        {
            screenpos.Y += 5;//incriments the position of the texture
            screenpos.Y = screenpos.Y % texture.Height;//modulos the position of the texture by its height and assigns the position the resultant value
            if (screenpos.Y < screenheight)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, screenpos);
                spriteBatch.End();
            }
            spriteBatch.Begin();
            spriteBatch.Draw(texture, screenpos - texsize);
            spriteBatch.End();
        }
    }

    public class GUI
    {
        public void Draw(SpriteFont spriteFont, SpriteBatch spriteBatch, ship player)//draws the GUI elements such as score and player lives.
        {
            string scorestr = "Score: " + player.score;
            string lifestr = "Lives: " + player.lives;
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, scorestr, new Vector2(0,0),Color.HotPink);
            spriteBatch.DrawString(spriteFont, lifestr, new Vector2(660, 0), Color.HotPink);
            spriteBatch.End();
        }
    }

    public class pattern//a class to generate enemy spawn patterns based on latin characters
    {
        public Vector2[] getpattern(char patternchar)
        {
            if (patternchar == 'v')
            {
                Vector2[] vectorarr = new Vector2[10];
                vectorarr[0] = new Vector2(0, 0);
                vectorarr[1] = new Vector2(30, 30);
                vectorarr[2] = new Vector2(60, 60);
                vectorarr[3] = new Vector2(90, 30);
                vectorarr[4] = new Vector2(120, 0);
                return vectorarr;
            }
            if (patternchar == 'x')
            {
                Vector2[] vectorarr = new Vector2[10];
                vectorarr[0] = new Vector2(0, 0);
                vectorarr[1] = new Vector2(30, 30);
                vectorarr[2] = new Vector2(60, 60);
                vectorarr[4] = new Vector2(0, 60);
                vectorarr[5] = new Vector2(60, 0);
                return vectorarr;
            }
            else
            {
                Vector2[] vectorarr = new Vector2[10];
                vectorarr[0] = new Vector2(0, 0);
                return vectorarr;
            }
        }
    }
}
