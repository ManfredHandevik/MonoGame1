using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace MonoGame1;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Texture2D xWingTexture;
    Vector2 xWingPosition;
    float speed = 300f;

    Texture2D enemyTexture;
    List<Enemy> enemies = new List<Enemy>();
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        xWingPosition = new Vector2(400, 300); 

        enemies.Add(new Enemy(new Vector2(100, -100)));
        enemies.Add(new Enemy(new Vector2(300, -300)));
        enemies.Add(new Enemy(new Vector2(600, -200)));

        base.Initialize();

    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        xWingTexture = Content.Load<Texture2D>("xwing");
        enemyTexture = Content.Load<Texture2D>("tiefighter");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        var kstate = Keyboard.GetState();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (kstate.IsKeyDown(Keys.Up)) xWingPosition.Y -= speed * dt;
        if (kstate.IsKeyDown(Keys.Down)) xWingPosition.Y += speed * dt;
        if (kstate.IsKeyDown(Keys.Left)) xWingPosition.X -= speed * dt;
        if (kstate.IsKeyDown(Keys.Right)) xWingPosition.X += speed * dt;


int screenWidth = _graphics.PreferredBackBufferWidth;
int screenHeight = _graphics.PreferredBackBufferHeight;


float marginX = xWingTexture.Width / 2f;
float marginY = xWingTexture.Height / 2f;


if (xWingPosition.X < -marginX) 
    xWingPosition.X = -marginX;
if (xWingPosition.X > screenWidth - marginX) 
    xWingPosition.X = screenWidth - marginX;

if (xWingPosition.Y < -marginY) 
    xWingPosition.Y = -marginY;
if (xWingPosition.Y > screenHeight - marginY) 
    xWingPosition.Y = screenHeight - marginY;

foreach (var enemy in enemies)
            {
                enemy.Update(gameTime);
                
                // Om en fiende åker utanför botten, flytta upp den till toppen igen
                if (enemy.Position.Y > _graphics.PreferredBackBufferHeight)
                {
                    enemy.Position.Y = -100;
                }
            }

        base.Update(gameTime);

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _spriteBatch.Begin();
        foreach (var enemy in enemies)
            {
                _spriteBatch.Draw(enemyTexture, enemy.Position, Color.White);
            }
        _spriteBatch.Draw(xWingTexture, xWingPosition, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

public class Enemy
{
    public Vector2 Position;
    public float Speed = 150f;

    public Enemy(Vector2 startPos)
    {
        Position = startPos;
    }

    public void Update(GameTime gameTime)
    {
        // Här kan vi få dem att röra sig nedåt automatiskt
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position.Y += Speed * dt;
    }
}