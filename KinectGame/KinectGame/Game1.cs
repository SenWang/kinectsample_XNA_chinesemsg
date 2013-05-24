using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace KinectGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        Rectangle videoDisplayRectangle;
        protected override void Initialize()
        {
            videoDisplayRectangle = new Rectangle(0, 0,GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            base.Initialize();
        }

        KinectSensor myKinect;
        SpriteFont message;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            myKinect = KinectSensor.KinectSensors[0];
            myKinect.ColorStream.Enable();
            myKinect.Start();
            message = Content.Load<SpriteFont>("Message");
        }

        void DataToVideoTexture(byte[] colorData,int Width,int Height)
        {
            kinectVideoTexture = new Texture2D(GraphicsDevice,Width, Height);

            Color[] bitmap = new Color[Width * Height];
            int sourceOffset = 0;
            for (int i = 0; i < bitmap.Length; i++)
            {
                bitmap[i] = new Color(colorData[sourceOffset + 2],
                                                         colorData[sourceOffset + 1],
                                                         colorData[sourceOffset],
                                                         255);
                sourceOffset += 4;
            }
            kinectVideoTexture.SetData(bitmap);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            byte[] colorData = null;
            using (ColorImageFrame colorFrame = myKinect.ColorStream.OpenNextFrame(0))
            {
                if (colorFrame != null)
                {
                    if (colorData == null)
                        colorData = new byte[colorFrame.Width * colorFrame.Height * 4];

                    colorFrame.CopyPixelDataTo(colorData);
                    DataToVideoTexture(colorData, colorFrame.Width, colorFrame.Height);
                }
            }

            base.Update(gameTime);
        }

        Texture2D kinectVideoTexture;
        string msg = "Kinect §@¥Î¤¤";
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            if (kinectVideoTexture != null)
            {
                spriteBatch.Draw(kinectVideoTexture, videoDisplayRectangle,
                                 Color.White);
            }

            spriteBatch.DrawString(message, msg, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
