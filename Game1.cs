using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Windows.Forms.Automation;

namespace MonoGame1;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    SpriteFont gameFont;
    Texture2D xWingTexture;
    Vector2 xWingPosition;
    float speed = 400f;
    Random random = new Random();
    float spawnTimer = 0f;
    float spawnInterval = 3f;

    Texture2D bulletTexture;
    List<Bullet> bullets = new List<Bullet>();
    KeyboardState prevKstate;
    Texture2D enemyTexture;
    List<Enemy> enemies = new List<Enemy>();
    int lives = 5;
    int score = 0;
    int highScore = 0;
    bool isGameOver = false;
        float invincibilityTimer = 0f;
        float blinkTimer = 0f;
        bool isVisible = true;
    public Game1()
    
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        xWingPosition = new Vector2(400, 300); 
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);

    gameFont = Content.Load<SpriteFont>("gamefont");
    gameFont = Content.Load<SpriteFont>("ScoreFont");

    xWingTexture = Content.Load<Texture2D>("xwing");
    enemyTexture = Content.Load<Texture2D>("tiefighter");

    bulletTexture = new Texture2D(GraphicsDevice, 4, 12);
    Color[] data = new Color[4 * 12];
    for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
    bulletTexture.SetData(data);
}
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (isGameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    lives = 5; 
                    score = 0;
                    isGameOver = false;
                    xWingPosition = new Vector2(400, 300);

                    foreach (var enemy in enemies)
                    {
                        enemy.Position.Y = -100; 
                    }
                    
                }
                return;
            }
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        spawnTimer -= dt;
                    if (spawnTimer <= 0)
                    {
                      float randomX = random.Next(0, _graphics.PreferredBackBufferWidth - 50);
                      enemies.Add(new Enemy(new Vector2(randomX, -100)));
                      spawnTimer = spawnInterval;
                    }
        var kstate = Keyboard.GetState();
        
        if (kstate.IsKeyDown(Keys.Up)) xWingPosition.Y -= speed * dt;
        if (kstate.IsKeyDown(Keys.Down)) xWingPosition.Y += speed * dt;
        if (kstate.IsKeyDown(Keys.Left)) xWingPosition.X -= speed * dt;
        if (kstate.IsKeyDown(Keys.Right)) xWingPosition.X += speed * dt;

if (kstate.IsKeyDown(Keys.Space) && !prevKstate.IsKeyDown(Keys.Space))
{
    bullets.Add(new Bullet(new Vector2(
        xWingPosition.X + xWingTexture.Width / 2f - 2,
        xWingPosition.Y)));
}
prevKstate = kstate;

if (kstate.IsKeyDown(Keys.F11) && !prevKstate.IsKeyDown(Keys.F11))
{
    _graphics.IsFullScreen = !_graphics.IsFullScreen;
    _graphics.ApplyChanges();
}

// Uppdatera skott och kolla kollision med fiender
foreach (var bullet in bullets)
{
    bullet.Update(dt);
    Rectangle bulletRect = new Rectangle((int)bullet.Position.X, (int)bullet.Position.Y, 4, 12);
    foreach (var enemy in enemies)
    {
        Rectangle enemyRect2 = new Rectangle(
            (int)enemy.Position.X, (int)enemy.Position.Y,
            (int)(enemyTexture.Width * 0.5f), (int)(enemyTexture.Height * 0.5f));
        if (bulletRect.Intersects(enemyRect2))
        {
            bullet.Active = false;
            enemy.Position.Y = -100;
            score += 10;
        }
    }
}
bullets.RemoveAll(b => !b.Active);


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

    spawnTimer -= dt;
    if (spawnTimer <= 0)
    {
        float randomX = random.Next(0, _graphics.PreferredBackBufferWidth - 50);
        enemies.Add(new Enemy(new Vector2(randomX, -100)));
        spawnTimer = spawnInterval;
    }

    int hitW = xWingTexture.Width / 2;
    int hitH = xWingTexture.Height / 3;
    Rectangle xWingRect = new Rectangle(
        (int)xWingPosition.X + (xWingTexture.Width - hitW) / 2,
         (int)xWingPosition.Y + (xWingTexture.Height - hitH) / 2,
        hitW,
        hitH
        );


foreach (var enemy in enemies)
            {
                enemy.Update(gameTime);
                
                // Om en fiende åker utanför botten, flytta upp den till toppen igen
                if (enemy.Position.Y > _graphics.PreferredBackBufferHeight)
                {
                    enemy.Position.Y = -100;
                }
                Rectangle enemyRect = new Rectangle(
                    (int)enemy.Position.X, 
                    (int)enemy.Position.Y, 
                    (int)(enemyTexture.Width * 0.5f), // Skala ner fiende till 50x50 pixlar
                    (int)(enemyTexture.Height * 0.5f)
                );
                if (xWingRect.Intersects(enemyRect) && invincibilityTimer <= 0)
                {
                   lives -= 1;
                     if (lives <= 0)
                      {
                         lives = 0; // Förhindrar negativt värde
                         if (score > highScore) highScore = score;
                        isGameOver = true;
                     }
                         invincibilityTimer = 1.5f;
                         blinkTimer = 0.1f;
                         isVisible = false;
                         enemy.Position.Y = -100;
}

                if(enemy.Position.Y > _graphics.PreferredBackBufferHeight)
                {
                    enemy.Position.Y = -100;
                }
                
            }
            
            if (invincibilityTimer > 0)
        {
            invincibilityTimer -= dt;
            blinkTimer -= dt;
            if (blinkTimer <= 0)
            {
                isVisible = !isVisible;
                blinkTimer = 0.1f; 
                
            }
        }
        else
        {
            invincibilityTimer = 0;
            isVisible = true;
        }

        base.Update(gameTime);

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White); 

    _spriteBatch.Begin();
if (!isGameOver)
{
    foreach (var enemy in enemies)
    {
        _spriteBatch.Draw(enemyTexture, new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y, 50, 50), Color.White);
    }

    foreach (var bullet in bullets)
        {
            _spriteBatch.Draw(bulletTexture, bullet.Position, Color.White);
        }


    if(isVisible)
    {
        _spriteBatch.Draw(xWingTexture, xWingPosition, Color.White);
    }
}
else
{
    
    if (gameFont != null)
    {
        string text= "GAME OVER!";
        string restartText = "Press Enter to Restart";
        _spriteBatch.DrawString(gameFont, text, new Vector2(300, 250), Color.Red);
        _spriteBatch.DrawString(gameFont, restartText, new Vector2(250, 300), Color.Red);
        _spriteBatch.DrawString(gameFont, "High Score: " + highScore, new Vector2(270, 350), Color.Red);
    }
}

if (gameFont != null)
        {
            _spriteBatch.DrawString(gameFont, "Lives: " + lives, new Vector2(10, 10), Color.Black);
            _spriteBatch.DrawString(gameFont, "Score: " + score, new Vector2(10, 40), Color.Black);
            _spriteBatch.DrawString(gameFont, "High Score: " + highScore, new Vector2(10, 70), Color.Black);
        }
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
       
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position.Y += Speed * dt;
   
    }
}
    public class Bullet
    {
    public Vector2 Position;
    public float Speed = 500f;
    public bool Active = true;

    public Bullet(Vector2 startPos)
    {
        Position = startPos;
    }

    public void Update(float dt)
    {
        Position.Y -= Speed * dt;
        if (Position.Y < -20) Active = false;
    }
    }
   