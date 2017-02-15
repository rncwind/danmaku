using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace moregameteststuff
{
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

        public gameobject(Texture2D texture, Vector2 position)
        {
            this.position = position;
            this.texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, position);
            spriteBatch.End();
        }
    }

    public class ship : gameobject
    {
        public int lives = 3;

        public ship(Texture2D texture, Vector2 position) : base(texture, position)
        {
        }

        public void move(GameTime gameTime)
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
        public bullet(Texture2D texture, Vector2 position) : base(texture, position)
        {
        }

        public void movebullet(List<bullet> bulletlist, powerup currentweapon)
        {
            foreach (bullet bullet in bulletlist)
            {
                for (int i = 0; i < bulletlist.Count; ++i)
                {
                    bulletlist[i].position.Y -= currentweapon.weaponvelocity;
                }
            }
        }

        public void bullettrash(List<bullet> bulletlist)
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

    public class enemy1 : gameobject
    {
        public int health = 20;
        public bool hit = false;
        public void killenemy(List<enemy1> enemy1list, int i)
        {
            enemy1list.RemoveAt(i);
        }

        public enemy1(Texture2D texture, Vector2 position) : base(texture, position)
        {
        }
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
        public bgsprite(Texture2D texture, Vector2 position) : base(texture,position)
        {
        }

        public void initbg(int screenheightpass, int screenwidth)
        {
            origin = new Vector2(texture.Width / 2, 0);
            screenpos = new Vector2(0, screenheight / 2);
            texsize = new Vector2(0, texture.Height);
            screenheight = screenheightpass;
        }

        public void bgloop()
        {
            screenpos.Y += 5;
            screenpos.Y = screenpos.Y % texture.Height;
        }

        new public void Draw(SpriteBatch spriteBatch)
        {
            if(screenpos.Y < screenheight)
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
}
